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

        static int n = 1_000_000_000;

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

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                for (int i = 0; i < n; i++)
                {
                    x[i] = reader.ReadDouble();

                    if (i % (n / 3) == 0)
                        Console.WriteLine($"Загружено {i} значений");
                }
            }

            Console.WriteLine("Начало сортировки массива");
            Array.Sort(x);
            Console.WriteLine("Сортировка массива окончена");

            double mid = (x[n / 2] + x[n / 2 + 1]) / 2;
            Console.WriteLine($"Медиана: { mid }");

            File.WriteAllText(filename + "_median.txt", mid.ToString());
        }

        static void CalculateMean(string filename)
        {
            double[] x = new double[n];

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                for (int i = 0; i < n; i++)
                {
                    x[i] = reader.ReadDouble();

                    if (i % (n / 3) == 0)
                        Console.WriteLine($"Загружено {i} значений");
                }
            }

            double M = 0;
            for (int i = 0; i < n; i++)
                M += x[i];

            M /= n;
            Console.WriteLine($"Среднее арифметическое: { M }");

            File.WriteAllText(filename + "_mean.txt", M.ToString());
        }

        static void Main(string[] args)
        {
            GenerateTrashAsymmetric("C:/repos/data/random_trash_symmetric_1");

            CalculateMean("C:/repos/data/random_trash_symmetric_1");
            CalculateMedian("C:/repos/data/random_trash_symmetric_1");
        }
    }
}
