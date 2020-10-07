using MathNet.Numerics.LinearAlgebra;

namespace MRZuIS1
{
    class Rect
    {
        /// <summary>
        /// 1xN Matrix for the Red component of the pixel.
        /// </summary>
        public Matrix<double> MatrixR { get; set; }
        /// <summary>
        /// 1xN Matrix for the Green component of the pixel.
        /// </summary>
        public Matrix<double> MatrixG { get; set; }
        /// <summary>
        /// 1xN Matrix for the Blue component of the pixel.
        /// </summary>
        public Matrix<double> MatrixB { get; set; }

        /// <summary>
        /// Creates the rect to store data about block. All matrices will be resized to 1xN.
        /// </summary>
        /// <param name="r">Matrix containing Red component of the pixel.</param>
        /// <param name="g">Matrix containing Green component of the pixel.</param>
        /// <param name="b">Matrix containing Blue component of the pixel.</param>
        public Rect(Matrix<double> r, Matrix<double> g, Matrix<double> b)
        {
            var matrixBuilder = Matrix<double>.Build;

            MatrixR = matrixBuilder.Dense(1, r.ColumnCount * r.RowCount, r.ToColumnMajorArray());
            MatrixG = matrixBuilder.Dense(1, r.ColumnCount * r.RowCount, g.ToColumnMajorArray());
            MatrixB = matrixBuilder.Dense(1, r.ColumnCount * r.RowCount, b.ToColumnMajorArray());
        }
    }
}