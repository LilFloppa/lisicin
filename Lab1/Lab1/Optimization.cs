using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public static class Optimization
    {
        public static double GoldenRatio(Func<double, double> f, double a, double b, double eps)
		{
			int n = 0;
			double diff = b - a;
			double prevDiff, ratio;
			double x1 = a + 0.381966011 * diff;
			double x2 = b - 0.381966011 * diff;

			double f1 = f(x1);
			double f2 = f(x2);

			while (Math.Abs(diff) > eps)
			{
				if (f1 < f2)
				{
					b = x2;
					x2 = x1;
					x1 = a + 0.381966011 * (b - a);
					f2 = f1;
					f1 = f(x1);
				}
				else
				{
					a = x1;
					x1 = x2;
					x2 = b - 0.381966011 * (b - a);
					f1 = f2;
					f2 = f(x2);
				}

				prevDiff = diff;
				diff = b - a;

				ratio = prevDiff / diff;
				n++;
			}

			return (x1 + x2) / 2;
		}
	}
}
