using SharpMcAe.Class.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharpMcAe.Class.Calculators {
    /// <summary>
    /// 贝塞尔曲线计算器
    /// </summary>
    public class BezierCalculator {
        private static DrawUtil Util = new DrawUtil();

        public double Bezier3x(double t, double p0, double p1, double p2, double p3) {
            return p0 * Math.Pow(1 - t, 3) + 3 * p1 * t * Math.Pow(1 - t, 2) + 3 * p2 * Math.Pow(t, 2) * (1 - t) + p3 * Math.Pow(t, 3);
        }

        public double Bezier3xGetC1(double i, double i_p1, double i_n1, double a = 0.25) {
            return i + a * (i_p1 - i_n1);
        }

        public double Bezier3xGetC2(double i_p1, double i_p2, double i, double a = 0.25) {
            return i_p1 - a * (i_p2 - i);
        }

        public List<int> BezierGetCountList(List<List<double>> pointsList, double step) {
            List<int> countList = new List<int>();
            for (int n = 0; n < pointsList.Count - 1; n++) {
                List<double> pointN = pointsList[n];
                List<double> pointNPlusOne = pointsList[n + 1];
                double d = Util.GetDistance(pointN, pointNPlusOne);
                int count = (int)(d / step);
                countList.Add(count);
            }
            return countList;
        }

        public List<double> Bezier3xGetPoints(int n, double p0, double p1, double p2, double p3) {
            double t = 0;
            List<double> bPoints = new List<double>();
            for (int i = 0; i <= n; i++) {
                bPoints.Add(Bezier3x(t, p0, p1, p2, p3));
                t += 1.0 / n;
            }
            return bPoints;
        }

        public List<List<double>> BezierLink(List<List<double>> points, List<int> counts, int take = 0) {
            List<List<double>> bPoints = new List<List<double>>();
            if (points.Count < 4) {
                Console.WriteLine("Error: Bezier3x requires at least four points");
                return bPoints;
            }

            for (int n = 0; n < points.Count - 1; n++) {
                double i = points[n][take];
                double i_p1 = points[n + 1][take];
                double i_p2 = (n == points.Count - 2) ? points[points.Count - 1][take] : points[n + 2][take];
                double i_n1 = (n == 0) ? points[0][take] : points[n - 1][take];

                double c1 = Bezier3xGetC1(i, i_p1, i_n1);
                double c2 = Bezier3xGetC2(i_p1, i_p2, i);

                bPoints.Add(Bezier3xGetPoints(counts[n], points[n][take], c1, c2, points[n + 1][take]));
            }

            return bPoints;
        }

        public List<List<List<double>>> Bezier3xXYZPoints(List<List<double>> pointsList, double step) {
            List<List<List<double>>> pointsLists = new();

            List<int> counts = BezierGetCountList(pointsList, step);

            List<List<List<double>>> xLists = new();
            foreach (var item in BezierLink(pointsList, counts, take: 0)) {
                xLists.Add(new List<List<double>> { item });
            }

            List<List<List<double>>> yLists = new();
            foreach (var item in BezierLink(pointsList, counts, take: 1)) {
                yLists.Add(new List<List<double>> { item });
            }

            List<List<List<double>>> zLists = new();
            foreach (var item in BezierLink(pointsList, counts, take: 2)) {
                zLists.Add(new List<List<double>> { item });
            }

            for (int listIndex = 0; listIndex < xLists.Count; listIndex++) {
                var xList = xLists[listIndex][0];
                var yList = yLists[listIndex][0];
                var zList = zLists[listIndex][0];

                List<List<double>> pointsTemp = new();
                for (int pointIndex = 0; pointIndex < xList.Count; pointIndex++) {
                    pointsTemp.Add(new List<double> { xList[pointIndex], yList[pointIndex], zList[pointIndex] });
                }

                pointsLists.Add(pointsTemp);
            }

            return pointsLists;
        }

        public List<List<double>> Bezier3xXYZ(List<List<double>> pointsList, double step) {
            List<List<double>> points = new List<List<double>>();

            List<List<List<double>>> bezierLists = Bezier3xXYZPoints(pointsList, step);

            foreach (var pointsSubList in bezierLists) {
                points.AddRange(pointsSubList);
            }

            return points;
        }
    }
}
