using System.Linq;
using MathNet.Numerics.LinearAlgebra;

public class SimultaneousEqns
{
    public static double[] Solve(double[,] coeffs, double[] consts)
    {
        Matrix<double> coeffs_m = Matrix<double>.Build.DenseOfArray(coeffs);
        Vector<double> consts_m = Vector<double>.Build.DenseOfArray(consts);

        var coeffs_inv = coeffs_m.Inverse();

        if (coeffs_inv == null)
        {
            return null;
        }

        return (coeffs_inv * consts_m).ToArray();
    }
}