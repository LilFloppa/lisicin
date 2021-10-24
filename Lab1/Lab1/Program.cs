using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;


namespace Lab1
{
    class Program
    {
        static Random random = new Random();

        static double nu = 0.05;
        static double b1 = 1.14;
        static double b4 = 2 * b1 * Math.Tan(b1);
        static double b2 = b4 / (2 + b4);
        static double b3 = nu * b4 * Math.Exp(b4) / 2;

        static int n = 50;

        static Func<double, double> f = (double x) =>
        {
            if (Math.Abs(x) <= 1.0)
            {
                double cos = Math.Cos(b1 * x);
                cos *= cos;
                return b2 * cos;
            }
            else
                return b3 * Math.Exp(-b4 * Math.Abs(x));
        };

        static double GenerateUniform()
        {
            double x = random.NextDouble();

            while (x == 0.0)
                x = random.NextDouble();

            return x;
        }
        static double GenerateRandomValue(double tetta, double lambda)
        {
            double r1 = GenerateUniform();

            if (r1 < nu)
            {
                double r4 = GenerateUniform();
                if (r1 < nu / 2)
                    return lambda * (1 - Math.Log(r4) / b4) + tetta;
                else
                    return lambda * (Math.Log(r4) / b4 - 1) + tetta;
            }
            else
            {
                double r2 = GenerateUniform();
                double r3 = GenerateUniform();
                double x = 2 * r2 - 1;

                double cos = Math.Cos(b1 * x);
                cos *= cos;

                while (r3 > cos)
                {
                    r2 = GenerateUniform();
                    r3 = GenerateUniform();
                    x = 2 * r2 - 1;

                    cos = Math.Cos(b1 * x);
                    cos *= cos;
                }

                return lambda * x + tetta;
            }
        }
        static double GenerateTrashValue(double tetta1, double tetta2, double lambda1, double lambda2)
        {
            double eps = 0.4;
            double r = GenerateUniform();

            if (r > 1 - eps)
                return GenerateRandomValue(tetta2, lambda2);
            else
                return GenerateRandomValue(tetta1, lambda1);
        }
        static void GenerateTrash()
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite("C:/repos/data/random_trash_symmetric")))
            {
                for (int i = 0; i < n; i++)
                {
                    writer.Write(GenerateTrashValue(0.0, 0.0, 1.0, 2.5));

                    if (i % 100_000_000 == 0)
                        Console.WriteLine(i);
                }
            }
        }
        static void GenerateTrashAsymmetric(string filename)
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename)))
            {
                for (int i = 0; i < n; i++)
                {
                    writer.Write(GenerateTrashValue(0.0, 1.325, 1.0, 2.5));

                    if (i % (n / 3) == 0)
                        Console.WriteLine($"Сгенерировано { i } значений");
                }
            }
        }
        static void CalculateMedian(string filename)
        {
            double[] x = new double[n];
            for (int i = 0; i < n; i++)
                x[i] = GenerateRandomValue(0.0, 1.0);

            Array.Sort(x);
            double mid = (x[n / 2] + x[n / 2 + 1]) / 2;

            File.WriteAllText(filename, mid.ToString());
        }
        static void CalculateMean(string filename)
        {
            double[] x = new double[n];
            for (int i = 0; i < n; i++)
                x[i] = GenerateRandomValue(0.0, 1.0);

            double M = 0;
            for (int i = 0; i < n; i++)
                M += x[i];

            M /= n;
            File.WriteAllText(filename, M.ToString());
        }
        static void MaxLikelihoodEstimation(string filename)
        {
            double lambda = 2.5;
            double[] x = new double[n];
            for (int i = 0; i < n; i++)
                x[i] = GenerateRandomValue(0.0, 1.0);

            Func<double, double> p = (double t) =>
            {
                double result = 0.0;
                for (int i = 0; i < n; i++)
                    result += -Math.Log(f((x[i] - t) / lambda));

                return result;
            };

            File.WriteAllText(filename, Optimization.GoldenRatio(p, -5.0, 5.0, 1.0e-9).ToString());
        }
        static void Радикальная(string filename, double delta)
        {
            double lambda = 2.5;
            double[] x = new double[n];
            for (int i = 0; i < n; i++)
                x[i] = GenerateRandomValue(0.0, 1.0);

            Func<double, double> p = (double t) =>
            {
                double result = 0.0;
                for (int i = 0; i < n; i++)
                    result += -Math.Pow(f((x[i] - t) / lambda), delta) / Math.Pow(f(0), delta);

                return result;
            };

            File.WriteAllText(filename, Optimization.GoldenRatio(p, -5.0, 5.0, 1.0e-9).ToString());
        }

        static void Main(string[] args)
        {
            CalculateMean("C:/repos/licisin/data/trash_sym_mean_1.txt");
            CalculateMedian("C:/repos/licisin/data/trash_sym_median_1.txt");
            MaxLikelihoodEstimation("C:/repos/licisin/data/trash_sym_max_likelihood_1.txt");
            Радикальная("C:/repos/licisin/data/trash_sym_radical_1_0_1.txt", 1.0);
            Радикальная("C:/repos/licisin/data/trash_sym_radical_0_5_1.txt", 0.5);
            Радикальная("C:/repos/licisin/data/trash_sym_radical_0_1_1.txt", 0.1);
        }
    }
}
