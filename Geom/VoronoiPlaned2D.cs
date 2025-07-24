using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Godot;
using Microsoft.VisualBasic;

public class VoironoiPlaned2D : IPolySource
{
    public class Cell : IPolygon
    {
        public readonly Vector2 Position;
        public List<CellEdge> Edges = [];

        public Cell(Vector2 position)
        {
            Position = position;
        }

        public IEnumerable<Line> GetLines()
        {
            foreach (var e in Edges)
            {
                yield return e.Edge;
            }
        }

        public IEnumerable<Vector2> GetVerts(bool closed = false)
        {
            // edges must remain sorted...
            foreach (var e in Edges)
            {
                yield return e.Edge.Start;
            }

            if (closed)
            {
                yield return Edges.First().Edge.Start;
            }
        }
    }

    // when an edge is part of the bounds, rather than another cell, Neighbour is null
    public class CellEdge
    {
        public Cell Neighbour;
        public Line Edge;

        public CellEdge(Cell neighbour, Line edge)
        {
            Neighbour = neighbour;
            Edge = edge;
        }
    }

    Rect2 Bounds;

    public readonly List<Cell> Cells = [];

    public VoironoiPlaned2D(Rect2 bounds)
    {
        Bounds = bounds;
    }

    public Cell AddPoint(Vector2 point)
    {
        Cell ret = CreateBoundsCell(point);

        foreach (var cell in Cells)
        {
            // it is not the general case that edges are connected End->Start
            // for at the moment, however, we are newly generated and currently that is the case,
            // which this algorithm replies on
            //
            // so this assert confirms that passes of this loop did't break that
            var prev_edge = ret.Edges.Last().Edge;
            foreach (var edge in ret.Edges.Select(x => x.Edge))
            {
                Debug.Assert(prev_edge.End == edge.Start);

                prev_edge = edge;
            }

            // Edge-Clipping setup:
            //              +------------------>+
            //              ^      (D)          |    ^
            //              |                   |    |
            //              |               (A) |  (out)
            //              |                   |    |
            //        ------|-------------------|--------- clip-line, infinite >
            //              |                   |
            //              | (C)               |
            //              |             (B)   v
            //              +<------------------+
            //
            // Edge-Clipping Result:
            //                   (Discarded)
            //              +------------------>+
            //
            //
            //
            //
            //              +     --- Q ---     +
            //              ^                   |
            // (ClippedEnd) | (C)           (A) | (ClippedStart)
            //              |      (B)          v
            //              +<------------------+
            //                   (Unchanged)
            //
            // e.g. in this case:
            // - A is truncated at the start
            // - B is unchanged
            // - C is truncated at the end
            // - D is discarded
            // - and the new edge "Q" must be synthesized
            // -- it can replace zero, one, or more existing edges, so it is not a new version of D)
            // -- this is achieved by keeping the end-point of the last unclipped edge
            // -- for ClippedEnd, this happens within a single edge

            // an infinite ray situated halfway between the two points
            Ray ray = CreateMidPointRay(point, cell.Position);

            bool clipped = ClipCell(ret, cell, ray);

            if (clipped)
            {
                ClipCell(cell, ret, ray.Reversed());
            }
        }


        Cells.Add(ret);

        return ret;
    }

    private static bool ClipCell(Cell to_clip, Cell new_neighbour, Ray ray)
    {
        bool any_changed = false;

        // there must be an easier way to do this, but for the moment run an extra loop
        // to find the fall-off-the-end values of was_in_at_end_of_edge and last_end;

        List<(Ray.ClipResult, Cell, Line)> temp_clips = [];

        foreach (var curr in to_clip.Edges)
        {
            var (result, new_line) = ray.ClipLine(curr.Edge);
            temp_clips.Add((result, curr.Neighbour, new_line));
        }

        // we'll build a list of the new _Start_ point of each edge with "cell"
        List<(Cell, Vector2)> temp_points = [];

        bool was_in_at_end_of_prev_edge = true;     // we look at this before setting it, but "true" stops us trying to use last_end before that is set
        bool is_in_at_start_of_edge = false;
        Vector2? last_end = null;

        temp_clips = temp_clips.Concat(temp_clips)
            .SkipWhile(x => x.Item1 == Ray.ClipResult.Discarded)    // <-- all other values set last_end
            .Take(temp_clips.Count)
            .ToList();

        // cyclically permute the list so we set last_end before trying to use it...
        foreach (var (result, neighbour, new_edge) in temp_clips)
        {
            is_in_at_start_of_edge = result != Ray.ClipResult.Discarded;

            if (is_in_at_start_of_edge && !was_in_at_end_of_prev_edge)
            {
                // when we move back into "in" from "out"(from clipping to not clipping)
                // we need to connect the _end_ of the last edge to the start of the new one
                // and this is the new edge for "cell"
                temp_points.Add((new_neighbour, last_end.Value));
            }

            was_in_at_end_of_prev_edge = result == Ray.ClipResult.Unchanged || result == Ray.ClipResult.ClippedStart;
            if (result != Ray.ClipResult.Discarded)
            {
                last_end = new_edge.End;
            }

            switch (result)
            {
                case Ray.ClipResult.ClippedEnd:
                    // generate two points and hence two edges:

                    // the remainder of the edge with the original neighbour, still starting at the same (unclipped) start
                    temp_points.Add((neighbour, new_edge.Start));

                    any_changed = true;
                    break;
                case Ray.ClipResult.Discarded:
                    // discarded, disappears
                    any_changed = true;
                    break;
                case Ray.ClipResult.ClippedStart:
                    // Start is moved, now terminates the new edge "Q", but still starts the same edge
                    any_changed = true;

                    temp_points.Add((neighbour, new_edge.Start));
                    break;
                case Ray.ClipResult.Unchanged:
                    // Unchanged, just pass through
                    temp_points.Add((neighbour, new_edge.Start));
                    break;
            }
        }

        // cover the case were the usage of last_end is after the last/before the first loop iteration
        is_in_at_start_of_edge = temp_clips.First().Item1 != Ray.ClipResult.Discarded;
        if (is_in_at_start_of_edge && !was_in_at_end_of_prev_edge)
        {
            temp_points.Add((new_neighbour, last_end.Value));
        }

        if (any_changed)
        {
            // clear old edges
            to_clip.Edges = [];

            var (prev_cell, prev_point) = temp_points.Last();

            foreach (var (cell, point) in temp_points)
            {
                to_clip.Edges.Add(new(prev_cell, new(prev_point, point)));

                prev_cell = cell;
                prev_point = point;
            }
        }

        return any_changed;
    }

    private Ray CreateMidPointRay(Vector2 p_in, Vector2 p_out)
    {
        var mid_point = (p_in + p_out) / 2;
        var dir = (p_out - p_in).Normalized();
        float dist_from_origin = mid_point.Dot(dir);

        return new Ray(dir, dist_from_origin);
    }

    Vector2? GetBoundsCorner(int i)
    {
        switch (i)
        {
            case 0:
                return Bounds.Position;
            case 1:
                return new Vector2(Bounds.End.X, Bounds.Position.Y);
            case 2:
                return Bounds.End;
            case 3:
                return new Vector2(Bounds.Position.X, Bounds.End.Y);
        }

        return null;
    }

    private Cell CreateBoundsCell(Vector2 point)
    {
        Cell ret = new(point);

        int last = 3;
        for (int i = 0; i < 4; i++)
        {
            ret.Edges.Add(new CellEdge(null, new(GetBoundsCorner(last).Value, GetBoundsCorner(i).Value)));
            last = i;
        }

        return ret;
    }

    public int PolyCount()
    {
        return Cells.Count;
    }

    public IEnumerable<IPolygon> GetPolys()
    {
        foreach (var c in Cells)
        {
            yield return c;
        }
    }
}