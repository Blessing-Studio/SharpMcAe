namespace SharpMcAe.Class.Calculators
{
    /// <summary>
    /// 向量计算器
    /// </summary>
    public static class VectorCalculator
    {
        /// <summary>
        /// 向量计算 - 交叉乘积
        /// </summary>
        /// <param name="vector1">向量1</param>
        /// <param name="vector2">向量2</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<double> Cross(this List<double> vector1, List<double> vector2)
        {
            if (vector1.Count != 3 || vector2.Count != 3)
            {
                throw new ArgumentException("Both vectors must have exactly 3 elements");
            }

            var crossProduct = new List<double> {
               vector1[1] * vector2[2] - vector1[2] * vector2[1],
               vector1[2] * vector2[0] - vector1[0] * vector2[2],
               vector1[0] * vector2[1] - vector1[1] * vector2[0]
            };

            return crossProduct;
        }

        /// <summary>
        /// 向量计算 - 内积
        /// </summary>
        /// <param name="vector1">向量1</param>
        /// <param name="vector2">向量2</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<double> Inner(this List<List<double>> a, List<List<double>> b)
        {
            if (a.Count != b.Count)
            {
                throw new ArgumentException("Both vectors must have the same number of elements.");
            }

            return a.Zip(b, (ai, bi) => ai.Zip(bi, (aji, bji) => aji * bji).Sum())
                .ToList();
        }

        /// <summary>
        /// 向量计算 - 矩阵乘积
        /// </summary>
        /// <param name="vector1">向量1</param>
        /// <param name="vector2">向量2</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<double> MatMul(this List<double> a, List<double> b)
        {
            if (a.Count != b.Count)
            {
                throw new ArgumentException("Both vectors must have the same number of elements");
            }

            return a.Zip(b, (ai, bi) => ai * bi)
                .ToList();
        }
    }
}
