using System;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace MRZuIS1
{
    class ImageToRects
    {
        public static int N, M;
        private readonly Bitmap _image;
        public static (Point bP, Point eP)[] RectCoordinates { get; private set; }

        /// <summary>
        /// Creates an Image to Rects converter, which takes a file path.
        /// </summary>
        /// <param name="filepath">Path to the file.</param>
        public ImageToRects(string filepath)
        {
            GetInput(out N, out M);
            _image = new Bitmap(filepath);
        }

        /// <summary>
        /// Convert the given image to rects of size n * m.
        /// </summary>
        /// <returns>An array of rects for further processing.</returns>
        public Rect[] ConvertToRects()
        {
            var matrices = GetMatrices(GetRegions(_image));
            Rect[] rects = new Rect[matrices.Length / 3];
            var rectIndex = 0;
            for (int i = 0; i < matrices.Length; i++, rectIndex++)
            {
                rects[rectIndex] = new Rect(matrices[i++], matrices[i++], matrices[i]);
            }
            return rects;
        }

        /// <summary>
        /// Builds matrices of Red, Green and Blue components of the given pixel for all of the image blocks.
        /// </summary>
        /// <param name="ps">An array of the starting and ending points of the blocks that define their boundaries on the canvas.</param>
        /// <returns>An array of matrices in the following order: Red, Green, Blue, Red, Green, Blue and so on.</returns>
        private Matrix<double>[] GetMatrices((Point beginPoint, Point endPoint)[] ps)
        {
            var matrices = new Matrix<double>[ps.Length * 3];
            var matrixIndex = 0;
            for (int i = 0; i < ps.Length; i++, matrixIndex++)
            {
                var matricesRGB = GetTwoDimensionalArrayPixelRGB(ps[i]);
                matrices[matrixIndex++] = Matrix<double>.Build.DenseOfArray(matricesRGB.R);
                matrices[matrixIndex++] = Matrix<double>.Build.DenseOfArray(matricesRGB.G);
                matrices[matrixIndex] = Matrix<double>.Build.DenseOfArray(matricesRGB.B);
            }

            return matrices;
        }

        /// <summary>
        /// Builds a tuple containing arrays for the Red, Green and Blue components of the pixels for specific boundary.
        /// </summary>
        /// <param name="p">A tuple, which defines the boundary of the one specific rect.</param>
        /// <returns>Tuple of arrays for the Red, Blue and Green components of the pixels.</returns>
        private (double[,] R, double[,] G, double[,] B) GetTwoDimensionalArrayPixelRGB((Point beginPoint, Point endPoint) p)
        {
            var rowIndex = 0;
            var colIndex = 0;
            var arrayR = new double[N, M];
            var arrayG = new double[N, M];
            var arrayB = new double[N, M];
            for (int i = p.beginPoint.Y; i < p.endPoint.Y; i++, rowIndex++)
            {
                for (int j = p.beginPoint.X; j < p.endPoint.X; j++, colIndex++)
                {
                    var pixel = _image.GetPixel(i, j);
                    arrayR[rowIndex, colIndex] = (2 * pixel.R / 255f) - 1;
                    arrayG[rowIndex, colIndex] = (2 * pixel.G / 255f) - 1;
                    arrayB[rowIndex, colIndex] = (2 * pixel.B / 255f) - 1;
                }
                colIndex = 0;
            }

            return (arrayR, arrayG, arrayB);
        }

        /// <summary>
        /// Get the N and M parameters for the rects to build.
        /// </summary>
        /// <param name="n">Height of the rect.</param>
        /// <param name="m">Width of the rect.</param>
        private void GetInput(out int n, out int m)
        {
            Console.WriteLine("Enter n, m through comma: ");
            var data = Console.ReadLine().Split(",");
            n = int.Parse(data[0]);
            m = int.Parse(data[1]);
        }

        /// <summary>
        /// Calculates the boundaries of the rects. Overlapping is possible.
        /// </summary>
        /// <param name="image">Image to process.</param>
        /// <returns>An array of the starting and ending points which define the boundaries of the rects.</returns>
        private (Point beginPoint, Point endPoint)[] GetRegions(Bitmap image)
        {
            int widthStepsNum, heightStepsNum;
            if (image.Width % M == 0)
            {
                widthStepsNum = image.Width / M;
            }
            else widthStepsNum = image.Width / M + 1;
            if (image.Height % N == 0)
            {
                heightStepsNum = image.Height / N;
            }
            else heightStepsNum = image.Height / N + 1;
            var regions = new (Point beginPoint, Point endPoint)[widthStepsNum * heightStepsNum];
            int index = 0;

            for (int j = 0; j < heightStepsNum; j++)
            {
                for (int i = 0; i < widthStepsNum; i++)
                {
                    if ((j + 1) * N <= image.Height && (i + 1) * M <= image.Width)
                    {
                        regions[index].beginPoint = new Point(i * M, j * N);
                        regions[index++].endPoint = new Point((i + 1) * M, (j + 1) * N);
                        continue;
                    }
                    if ((j + 1) * N > image.Height && (i + 1) * M <= image.Width)
                    {
                        regions[index].beginPoint = new Point(i * M, image.Height - N);
                        regions[index++].endPoint = new Point((i + 1) * M, image.Height);
                        continue;
                    }
                    if ((j + 1) * N > image.Height && (i + 1) * M > image.Width)
                    {
                        regions[index].beginPoint = new Point(image.Width - M, image.Height - N);
                        regions[index++].endPoint = new Point(image.Width, image.Height);
                        continue;
                    }
                    if ((j + 1) * N <= image.Height && (i + 1) * M > image.Width)
                    {
                        regions[index].beginPoint = new Point(image.Width - M, j * N);
                        regions[index++].endPoint = new Point(image.Width, (j + 1) * N);
                    }
                }
            }
            RectCoordinates = regions;
            return regions;
        }
    }
}