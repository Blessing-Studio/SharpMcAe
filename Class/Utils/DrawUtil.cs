using SharpMcAe.Class.Calculators;
using System.Drawing;

namespace SharpMcAe.Class.Utils {
    public class DrawUtil {
        /// <summary>
        /// 转换坐标格式
        /// </summary>
        /// <param name="points">转换类别</param>
        /// <returns></returns>
        public List<List<double>> ArrayTran(params List<double>[] points) {
            var outList = new List<List<double>>();
            if (points.Length == 3 && points.All(p => p.Count == 1)) {
                outList.Add(new List<double> { points[0][0], points[1][0], points[2][0] });
            } else if (points.Length == 1) {
                outList = points[0].Select(p => new List<double> { p }).ToList();
            } else {
                outList = points.ToList();
            }
            return outList;
        }

        /// <summary>
        /// 求两点距离
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <returns></returns>
        public double GetDistance(List<double> p1, List<double> p2) {
            return Math.Sqrt(Math.Pow(p2[0] - p1[0], 2) + Math.Pow(p2[1] - p1[1], 2) + Math.Pow(p2[2] - p1[2], 2));
        }

        /// <summary>
        /// 整体移动点列表
        /// </summary>
        /// <param name="points">点列表</param>
        /// <param name="x">x方向移动量</param>
        /// <param name="y">y方向移动量</param>
        /// <param name="z">z方向移动量</param>
        /// <returns></returns>
        public List<List<double>> Move(List<List<double>> points, double x, double y, double z) {
            return points.Select(p => new List<double> { p[0] + x, p[1] + y, p[2] + z }).ToList();
        }

        /// <summary>
        /// 获取点列中点
        /// </summary>
        /// <param name="pointsList">点列表</param>
        /// <returns></returns>
        public List<double> GetMidpoint(List<List<double>> pointsList) {
            var x = pointsList.Average(p => p[0]);
            var y = pointsList.Average(p => p[1]);
            var z = pointsList.Average(p => p[2]);
            return new List<double> { x, y, z };
        }

        /// <summary>
        /// 单位化向量
        /// </summary>
        /// <param name="p1">向量起点</param>
        /// <param name="p2">向量终点</param>
        /// <returns></returns>
        public List<double> VecUnit(List<double> p1, List<double> p2) {
            var dx = p2[0] - p1[0];
            var dy = p2[1] - p1[1];
            var dz = p2[2] - p1[2];
            var d = GetDistance(p1, p2);
            return new List<double> { dx / d, dy / d, dz / d };
        }

        /// <summary>
        /// 通过旋转矩阵绕单位向量旋转点
        /// </summary>
        /// <param name="u">旋转轴单位向量 x</param>
        /// <param name="v">旋转轴单位向量 y</param>
        /// <param name="w">旋转轴单位向量 z</param>
        /// <param name="a">角度</param>
        /// <param name="x">被旋转的的点 x</param>
        /// <param name="y">被旋转的的点 y</param>
        /// <param name="z">被旋转的的点 z</param>
        /// <returns></returns>
        public List<double> Rotate(double u, double v, double w, double a, double x, double y, double z) {
            a = Math.PI * a / 180.0; // convert to radians
            var matrix = new double[,] {
            { Math.Cos(a) + u * u * (1 - Math.Cos(a)), u * v * (1 - Math.Cos(a)) - w * Math.Sin(a), u * w * (1 - Math.Cos(a)) + v * Math.Sin(a) },
            { v * u * (1 - Math.Cos(a)) + w * Math.Sin(a), Math.Cos(a) + v * v * (1 - Math.Cos(a)), v * w * (1 - Math.Cos(a)) - u * Math.Sin(a) },
            { w * u * (1 - Math.Cos(a)) - v * Math.Sin(a), w * v * (1 - Math.Cos(a)) + u * Math.Sin(a), Math.Cos(a) + w * w * (1 - Math.Cos(a)) }
        };
            var result = new List<double> { matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] * z,
                                    matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] * z,
                                    matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2] * z };
            return result.ToList();
        }

        /// <summary>
        /// 通过旋转矩阵绕任意向量旋转点
        /// </summary>
        /// <param name="u1">旋转轴向量 x1</param>
        /// <param name="v1">旋转轴向量 y1</param>
        /// <param name="w1">旋转轴向量 z1</param>
        /// <param name="u2">旋转轴向量 x2</param>
        /// <param name="v2">旋转轴向量 y2</param>
        /// <param name="w2">旋转轴向量 z2</param>
        /// <param name="degree">角度</param>
        /// <param name="x">被旋转的的点 x</param>
        /// <param name="y">被旋转的的点 y</param>
        /// <param name="z">被旋转的的点 z</param>
        /// <returns></returns>
        public List<double> RotateByVec(double u1, double v1, double w1, double u2, double v2, double w2, double degree, double x, double y, double z) {
            var unitVec = VecUnit(new List<double> { u1, v1, w1 }, new List<double> { u2, v2, w2 });
            x -= u1; y -= v1; z -= w1;
            var rotatedPoint = Rotate(unitVec[0], unitVec[1], unitVec[2], degree, x, y, z);
            rotatedPoint[0] += u1; rotatedPoint[1] += v1; rotatedPoint[2] += w1;
            return rotatedPoint;
        }

        /// <summary>
        /// 按向量旋转点
        /// </summary>
        /// <param name="u1">旋转轴向量 x1</param>
        /// <param name="v1">旋转轴向量 y1</param>
        /// <param name="w1">旋转轴向量 z1</param>
        /// <param name="u2">旋转轴向量 x2</param>
        /// <param name="v2">旋转轴向量 y2</param>
        /// <param name="w2">旋转轴向量 z2</param>
        /// <param name="degree">角度</param>
        /// <param name="points">点</param>
        /// <returns></returns>
        public List<List<double>> RotatePointsByVec(double u1, double v1, double w1, double u2, double v2, double w2, double degree, List<List<double>> points) {
            var rPoints = new List<List<double>>();
            foreach (var p in points) {
                var rotatedPoint = RotateByVec(u1, v1, w1, u2, v2, w2, degree, p[0], p[1], p[2]);
                rPoints.Add(rotatedPoint);
            }
            return rPoints;
        }

        /// <summary>
        /// 坐标变换
        /// </summary>
        /// <param name="e1">基坐标1</param>
        /// <param name="e2">基坐标2</param>
        /// <param name="points">点</param>
        /// <returns></returns>
        public List<List<double>> CoordinateTransformation(List<double> e1, List<double> e2, List<List<double>> points) {
            var pr = new List<List<double>>();
            e1 = VecUnit(new List<double> { 0, 0, 0 }, e1);
            e2 = VecUnit(new List<double> { 0, 0, 0 }, e2);
            if (e1.Zip(e2, (a, b) => a * b).Sum() != 0) {
                Console.WriteLine("Error:坐标构建失败");
                return pr;
            }
            
            var e3 = e1.Cross(e2);
            var a1 = new List<List<double>> { new List<double> { 1, 0, 0 }, new List<double> { 0, 1, 0 }, new List<double> { 0, 0, 1 } };
            var A = a1.Inner(new List<List<double>> { e1, e2, e3 });

            foreach (var p in points) {
                pr.Add(A.MatMul(p));
            }

            return pr;
        }

        /// <summary>
        /// 给出两点生成直线
        /// </summary>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public List<List<double>> Line(List<double> p1, List<double> p2, double step) {
            List<List<double>> points = new List<List<double>>();
            List<double> diff = new() { p2[0] - p1[0], p2[1] - p1[1], p2[2] - p1[2] };
            double dist = GetDistance(p1, p2);
            int count = (int)(dist / step);
            List<double> increment = new() { diff[0] / count, diff[1] / count, diff[2] / count };
            for (int i = 0; i <= count; i++) {
                points.Add(new List<double> { p1[0] + increment[0] * i, p1[1] + increment[1] * i, p1[2] + increment[2] * i });
            }
            return points;
        }

        /// <summary>
        /// 首位相连多个点
        /// </summary>
        /// <param name="pointList">点列表</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public List<List<double>> LineLink(List<List<double>> pointList, double step) {
            List<List<double>> points = new List<List<double>>();
            for (int i = 0; i < pointList.Count; i++) {
                List<double> p1 = pointList[i];
                List<double> p2 = (i == pointList.Count - 1) ? pointList[0] : pointList[i + 1];
                points.AddRange(Line(p1, p2, step));
            }
            return points;
        }

        /// <summary>
        /// 一个点连接多个点
        /// </summary>
        /// <param name="pointList">点列表</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public List<List<double>> LineLinkOneToN(double x, double y, double z, List<List<double>> pointsList, int step) {
            // 一个点连接多个点
            List<List<double>> points = new List<List<double>>();
            foreach (var p in pointsList) {
                points.AddRange(Line(new List<double> { x, y, z }, p, step));
            }
            return points;
        }

        /// <summary>
        /// 给出经过三点求抛物线参数
        /// </summary>
        /// <returns></returns>
        public List<double> SolveParabola(double x1, double x2, double x3, double y1, double y2, double y3) {
            // 使用Cramer's rule解决线性方程组
            double a = ((y2 - y3) * (x1 - x2) - (y1 - y2) * (x2 - x3)) / ((x1 - x2) * (x2 * x2 - x3 * x3) - (x1 * x1 - x2 * x2) * (x2 - x3));
            double b = ((y1 - y2) - a * (x1 * x1 - x2 * x2)) / (x1 - x2);
            double c = y1 - a * x1 * x1 - b * x1;

            return new() { a, b, c };
        }

        /// <summary>
        /// 抛物线求出第三个点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public List<double> GetThirdPointParabola(List<double> p1, List<double> p2) {
            double dy = p2[1] - p1[1];
            double d = GetDistance(new() { p1[0], 0, p1[2] }, new() { p2[0], 0, p2[2] });

            double d3 = d / 2;
            double y3 = (p2[1] * (d - dy) / 2 / d + p1[1] * (d + dy) / 2 / d + p2[1] + (d - dy) / 2) / 2;
            double xzRatio = (p2[0] - p1[0]) / (p2[2] - p1[2]);
            double x3 = (xzRatio > 0) ? (p2[0] * (d - dy) / 2 / d + p1[0] * (d + dy) / 2 / d) : (p2[2] * (d - dy) / 2 / d + p1[2] * (d + dy) / 2 / d);
            double z3 = (!xzRatio.Equals(0)) ? x3 / xzRatio : ((p2[2] * (d - dy) / 2 / d + p1[2] * (d + dy) / 2 / d));

            return new() { x3, y3, z3 };
        }

        /// <summary>
        /// 给出两点生成竖直平面内抛物线,xz平面内投影疏密均匀
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<List<double>> Parabola(List<double> p1, List<double> p2, double step) {
            List<List<double>> points = new List<List<double>>();

            List<double> thirdPoint = GetThirdPointParabola(p1, p2);

            List<double> abc = SolveParabola(0, GetDistance(new() { p1[0], 0, p1[2] }, new() { thirdPoint[0], 0, thirdPoint[2] }), GetDistance(new() { p1[0], 0, p1[2] }, new() { p2[0], 0, p2[2] }), 0, thirdPoint[1] - p1[1], p2[1] - p1[1]);

            double a = abc[0];
            double b = abc[1];
            double c = abc[2];

            double dist = GetDistance(new() { p1[0], 0, p1[2] }, new() { p2[0], 0, p2[2] });
            int count = (int)(dist / step);
            List<double> diff = new() { p2[0] - p1[0], p2[1] - p1[1], p2[2] - p1[2] };
            List<double> increment = new() { diff[0] / count, diff[1] / count, diff[2] / count };

            for (int i = 0; i <= count; i++) {
                double px = step * i;
                double py = a * px * px + b * px + c;
                points.Add(new() { p1[0] + increment[0] * i, p1[1] + py, p1[2] + increment[2] * i });
            }

            return points;
        }

        /// <summary>
        /// 给出圆心，半径，单位法向量，角度，求空间圆上一点
        /// </summary>
        /// <param name="x0">圆心x</param>
        /// <param name="y0">圆心y</param>
        /// <param name="z0">圆心z</param>
        /// <param name="r">半径</param>
        /// <param name="n">列表，单位化的法向量</param>
        /// <param name="t">角度</param>
        /// <returns></returns>
        public List<double> CircleVecPoint(double x0, double y0, double z0, double r, List<double> n, double t) {
            List<double> j = new List<double> { 1, 0, 0 };
            List<double> k = new List<double> { 0, 1, 0 };
            List<double> a = n.Cross(j);
            if (a[0] == 0 && a[1] == 0 && a[2] == 0) {
                a = n.Cross(k);
            }
            List<double> b = n.Cross(a);
            List<double> aUnit = VecUnit(new List<double> { 0, 0, 0 }, a);
            List<double> bUnit = VecUnit(new List<double> { 0, 0, 0 }, b);
            double x = x0 + r * Math.Cos(t) * aUnit[0] + r * Math.Sin(t) * bUnit[0];
            double y = y0 + r * Math.Cos(t) * aUnit[1] + r * Math.Sin(t) * bUnit[1];
            double z = z0 + r * Math.Cos(t) * aUnit[2] + r * Math.Sin(t) * bUnit[2];
            return new List<double> { x, y, z };
        }

        /// <summary>
        /// 给出圆心，半径，单位法向量，角度，求空间圆
        /// </summary>
        /// <returns></returns>
        public List<List<double>> CircleVec(double x0, double y0, double z0, double r, List<double> n, double step) {
            List<List<double>> points = new List<List<double>>();
            double d = Math.PI * 2 * r;
            int count = (int)(d / step);
            for (int i = 0; i <= count; i++) {
                double t = (360.0 * (i / (double)count)) * (Math.PI / 180.0);
                points.Add(CircleVecPoint(x0, y0, z0, r, n, t));
            }
            return points;
        }

        /// <summary>
        /// 给出圆心，半径，生成xz平面上的圆
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="r"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<List<double>> Circle(double x, double y, double z, double r, double step) {
            return CircleVec(x, y, z, r, new List<double> { 0, 1, 0 }, step);
        }

        /// <summary>
        /// 给出圆心，长轴及短轴长度，生成xz平面椭圆
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y"></param>
        /// <param name="z0"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<List<double>> Ellipse(double x0, double y, double z0, double a, double b, double step) {
            List<List<double>> points = new List<List<double>>();
            double l = 2 * Math.PI * b + 4 * (a - b);
            int count = (int)(l / step);
            for (int i = 0; i <= count; i++) {
                double x = x0 + a * Math.Cos(2 * Math.PI * (i / (double)count));
                double z = z0 + b * Math.Sin(2 * Math.PI * (i / (double)count));
                points.Add(new List<double> { x, y, z });
            }
            return points;
        }

        /// <summary>
        /// 给出一个顶点和长宽，生成xz平面矩形
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<List<double>> Rectangle(double x, double y, double z, double a, double b, double step) {
            var points = new List<List<double>>();
            points.AddRange(Line(new() { x - a / 2, y, z + b / 2 }, new() { x + a / 2, y, z + b / 2 }, step));
            points.AddRange(Line(new() { x + a / 2, y, z + b / 2 }, new() { x + a / 2, y, z - b / 2 }, step));
            points.AddRange(Line(new() { x + a / 2, y, z - b / 2 }, new() { x - a / 2, y, z - b / 2 }, step));
            points.AddRange(Line(new() { x - a / 2, y, z - b / 2 }, new() { x - a / 2, y, z + b / 2 }, step));
            return points;
        }

        /// <summary>
        /// 给出顶点，边长，生成xz平面正方形
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="step"></param>
        public void Square(double x, double y, double z, double a, double step) {
            var points = Rectangle(x, y, z, a, a, step);
        }

        /// <summary>
        /// 给出中心，边长，生成xz平面正三角形
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="step"></param>
        public void Delta(double x, double y, double z, double a, double step) {
            var points = new List<List<double>>();
            var r = a / (2 * Math.Sin(Math.PI / 3));
            points.AddRange(Line(new() { x - a / 2, y, z - r * Math.Sin(Math.PI / 6) }, new() { x, y, z + r }, step));
            points.AddRange(Line(new() { x, y, z + r }, new() { x + a / 2, y, z - r * Math.Sin(Math.PI / 6) }, step));
            points.AddRange(Line(new() { x + a / 2, y, z - r * Math.Sin(Math.PI / 6) }, new() { x - a / 2, y, z - r * Math.Sin(Math.PI / 6) }, step));
        }

        /// <summary>
        /// 给出中心，边数，半径，生成xz平面正多边形的各个顶点
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="n"></param>
        /// <param name="r"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<List<double>> PolygonApex(double x0, double y0, double z0, int n, double r, int step) {
            // 生成xz平面正多边形的各个顶点
            List<List<double>> apexes = new List<List<double>>();
            double ex_angle = 2 * Math.PI / n;
            List<double> a1 = new List<double> { x0 - r * Math.Cos(ex_angle / 2), y0, z0 + r * Math.Sin(ex_angle / 2) };
            apexes.Add(a1);
            for (int t = 1; t < n; t++) {
                List<double> ap = RotateByVec(x0, y0, z0, x0, y0 + 1, z0, t * 360 / n, a1[0], a1[1], a1[2]);
                apexes.Add(ap);
            }
            return apexes;
        }

        /// <summary>
        /// 给出中心，边数，半径，生成xz平面正多边形
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="n"></param>
        /// <param name="r"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<List<double>> Polygon(double x0, double y0, double z0, int n, double r, int step) {
            // 生成xz平面正多边形
            List<List<double>> apexes = PolygonApex(x0, y0, z0, n, r, step);
            List<List<double>> points = LineLink(apexes, step);
            return points;
        }

        /// <summary>
        /// 给出中心，边数，半径，高度，生成底面为正多边形的棱锥
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="n"></param>
        /// <param name="r"></param>
        /// <param name="h"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<List<double>> RegularPyramid(double x0, double y0, double z0, int n, double r, double h, int step) {
            // 生成底面为正多边形的棱锥
            List<List<double>> apexes = PolygonApex(x0, y0, z0, n, r, step);
            List<List<double>> points = LineLink(apexes, step);

            // 棱锥顶点
            List<double> ah = new List<double> { x0, y0 + h, z0 };
            points.AddRange(LineLinkOneToN(ah[0], ah[1], ah[2], apexes, step));

            return points;
        }

        /// <summary>
        /// 生成半径不变，轨迹支持自定义的螺线
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="r"></param>
        /// <param name="step"></param>
        /// <param name="degree"></param>
        /// <param name="path_type"></param>
        /// <param name="custom_points"></param>
        /// <param name="add"></param>
        /// <param name="deg_d"></param>
        public void Helix(List<double> p1, List<double> p2, double r, double step, double degree = 0, string path_type = "line", List<List<double>> custom_points = null, bool add = true, int deg_d = 3) {
            var points = new List<List<double>>();
            var lp = new List<List<double>>();
            if (path_type == "line") {
                lp = Line(p1, p2, step);
            } else if (path_type == "parabola") {
                lp = Parabola(p1, p2, step);
            } else if (path_type == "custom") {
                lp = custom_points;
            }
            var cp1 = new List<List<double>>();
            for (int i = 0; i < lp.Count - 1; i++) {
                var x0 = lp[i][0];
                var y0 = lp[i][1];
                var z0 = lp[i][2];
                var n = VecUnit(lp[i], lp[i + 1]);
                var cp = CircleVecPoint(x0, y0, z0, r, n, (Math.PI / 180) * degree);
                cp1.Add(cp);
                degree += deg_d;
            }
            if (add) {
                points.AddRange(lp);
            }
            points.AddRange(cp1);
        }

        /// <summary>
        /// 生成长方体
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="step"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public List<List<double>> Cuboid(double x0, double y0, double z0, List<double> n1, List<double> n2, double step, double a = 1, double b = 1, double c = 1) {
            // 生成长方体
            a /= 2;
            b /= 2;
            c /= 2;

            List<List<double>> relativeP = new();
            relativeP.AddRange(Line(new() { a, b, c }, new() { a, b, -c }, step));
            relativeP.AddRange(Line(new() { a, b, c }, new() { -a, b, c }, step));
            relativeP.AddRange(Line(new() { -a, b, c }, new() { -a, b, -c }, step));
            relativeP.AddRange(Line(new() { a, b, -c }, new() { -a, b, -c }, step));
            relativeP.AddRange(Line(new() { a, b, c }, new() { a, -b, c }, step));
            relativeP.AddRange(Line(new() { -a, b, c }, new() { -a, -b, c }, step));
            relativeP.AddRange(Line(new() { a, b, -c }, new() { a, -b, -c }, step));
            relativeP.AddRange(Line(new() { -a, b, -c }, new() { -a, -b, -c }, step));
            relativeP.AddRange(Line(new() { a, -b, c }, new() { a, -b, -c }, step));
            relativeP.AddRange(Line(new() { a, -b, c }, new() { -a, -b, c }, step));
            relativeP.AddRange(Line(new() { -a, -b, c }, new() { -a, -b, -c }, step));
            relativeP.AddRange(Line(new() { a, -b, -c }, new() { -a, -b, -c }, step));

            var p = CoordinateTransformation(n1, n2, relativeP);
            var points = new List<List<double>>();

            foreach (var itm in p)
                points.Add(new List<double> { x0, y0, z0 }.Zip(itm, (x, y) => x + y).ToList());

            return points;
        }

        /// <summary>
        /// 生成正方体
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="step"></param>
        /// <param name="a"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public List<List<double>> Cube(double x0, double y0, double z0, double step = 0.1, double a = 1, List<double> n1 = null, List<double> n2 = null) {
            //生成正方体
            if (n1 == null) n1 = new List<double> { 1.0, 0.0, 0.0 };
            if (n2 == null) n2 = new List<double> { 0.0, 1.0, 0.0 };
            return Cuboid(x0, y0, z0, n1, n2, step, a, a, a);
        }
    }
}
