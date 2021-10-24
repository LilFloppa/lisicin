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

        static int n = 100;

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
        static void CalculateMedian(double[] x, string filename)
        {
            double[] y = new double[n];
            for (int i = 0; i < n; i++)
                y[i] = x[i];

            Array.Sort(y);
            double mid = (y[n / 2] + y[n / 2 + 1]) / 2;

            File.WriteAllText(filename, mid.ToString("0.000"));
        }
        static void CalculateMean(double[] x, string filename)
        {
            double M = 0;
            for (int i = 0; i < n; i++)
                M += x[i];

            M /= n;
            File.WriteAllText(filename, M.ToString("0.000"));
        }
        static void MaxLikelihoodEstimation(double[] x, double lambda, string filename)
        {
            Func<double, double> p = (double t) =>
            {
                double result = 0.0;
                for (int i = 0; i < n; i++)
                    result += -Math.Log(f((x[i] - t) / lambda));

                return result;
            };

            File.WriteAllText(filename, Optimization.GoldenRatio(p, -5.0, 5.0, 1.0e-9).ToString("0.000"));
        }
        static void Радикальная(double[] x, double lambda, string filename, double delta)
        {
            Func<double, double> p = (double t) =>
            {
                double result = 0.0;
                for (int i = 0; i < n; i++)
                    result += -Math.Pow(f((x[i] - t) / lambda), delta) / Math.Pow(f(0), delta);

                return result;
            };

            File.WriteAllText(filename, Optimization.GoldenRatio(p, -5.0, 5.0, 1.0e-9).ToString("0.000"));
        }

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() as System.Globalization.CultureInfo ?? throw new InvalidCastException();
            culture.NumberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            double[] x = new double[n];

            for (int i = 0; i < n; i++)
                x[i] = GenerateRandomValue(0.0, 1.0);

            double lambda = 1.0;

            CalculateMean(x, "C:/repos/licisin/data/clean_mean.txt");
            CalculateMedian(x, "C:/repos/licisin/data/clean_median.txt");
            MaxLikelihoodEstimation(x, lambda, "C:/repos/licisin/data/clean_max_likelihood.txt");
            Радикальная(x, lambda, "C:/repos/licisin/data/clean_radical_1_0.txt", 1.0);
            Радикальная(x, lambda, "C:/repos/licisin/data/clean_radical_0_5.txt", 0.5);
            Радикальная(x, lambda, "C:/repos/licisin/data/clean_radical_0_1.txt", 0.1);

            for (int i = 0; i < n; i++)
                x[i] = GenerateTrashValue(0.0, 0.0, 1.0, 2.5);

            lambda = 2.5;

            CalculateMean(x, "C:/repos/licisin/data/trash_sym_mean.txt");
            CalculateMedian(x, "C:/repos/licisin/data/trash_sym_median.txt");
            MaxLikelihoodEstimation(x, lambda, "C:/repos/licisin/data/trash_sym_max_likelihood.txt");
            Радикальная(x, lambda, "C:/repos/licisin/data/trash_sym_radical_1_0.txt", 1.0);
            Радикальная(x, lambda, "C:/repos/licisin/data/trash_sym_radical_0_5.txt", 0.5);
            Радикальная(x, lambda, "C:/repos/licisin/data/trash_sym_radical_0_1.txt", 0.1);

            for (int i = 0; i < n; i++)
                x[i] = GenerateTrashValue(0.0, 1.325, 1.0, 2.5);

            lambda = 2.5;

            CalculateMean(x, "C:/repos/licisin/data/trash_ass_mean.txt");
            CalculateMedian(x, "C:/repos/licisin/data/trash_ass_median.txt");
            MaxLikelihoodEstimation(x, lambda, "C:/repos/licisin/data/trash_ass_max_likelihood.txt");
            Радикальная(x, lambda, "C:/repos/licisin/data/trash_ass_radical_1_0.txt", 1.0);
            Радикальная(x, lambda, "C:/repos/licisin/data/trash_ass_radical_0_5.txt", 0.5);
            Радикальная(x, lambda, "C:/repos/licisin/data/trash_ass_radical_0_1.txt", 0.1);
        }
    }
}
