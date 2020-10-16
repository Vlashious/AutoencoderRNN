using System;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace MRZuIS1
{
    class NeuralNetwork
    {
        private readonly Rect[] _rects;
        private double LearningRate { get; }
        private double CompressionRate { get; }
        private double Error { get; }
        private Matrix<double> _firstLayerWeights;
        private Matrix<double> _secondLayerWeights;
        private int LearningSteps { get; set; }

        /// <summary>
        /// Builds the neural network, which will process the given rects of data.
        /// </summary>
        /// <param name="rects">Rects of data to process.</param>
        public NeuralNetwork(Rect[] rects)
        {
            _rects = rects;
            Console.WriteLine("Enter the learning rate and error through comma: ");
            var data = Console.ReadLine().Split(",");
            LearningRate = double.Parse(data[0]);
            Error = double.Parse(data[1]);

            Console.WriteLine("Enter the size of the bottleneck: ");
            var hiddenLayerSize = int.Parse(Console.ReadLine());

            _firstLayerWeights = Matrix<double>.Build.Random(_rects[0].MatrixR.ColumnCount, hiddenLayerSize, new ContinuousUniform(-0.1, 0.1));
            _secondLayerWeights = _firstLayerWeights.Transpose();

            CompressionRate = ((double)_rects[0].MatrixR.ColumnCount * (double)_rects[0].MatrixR.RowCount * _rects.Length) / (((double)_rects[0].MatrixR.ColumnCount * (double)_rects[0].MatrixR.RowCount + _rects.Length) * hiddenLayerSize + 2);
        }

        /// <summary>
        /// Train the neural network until the error is lower than the specified error cap.
        /// </summary>
        public void Train()
        {
            var curError = double.MaxValue;
            // var error = _error * _rects.Length;
            while (curError > Error || double.IsNaN(curError))
            {
                curError = 0;
                LearningSteps++;
                Parallel.For(0, _rects.Length, (i) =>
                {
                    ProcessRect(_rects[i]);
                });
                curError = _rects.AsParallel().Select((r) => ProcessRectGetError(r)).Sum(); ;
                Console.WriteLine($"Error for sample is: {curError} of {Error}");
            }
            Console.WriteLine($"Training is over!");
            Console.WriteLine($"Compression rate: {CompressionRate}");
            Console.WriteLine($"Learning steps: {LearningSteps}");
            Console.WriteLine($"Allowed error: {Error}");
            Console.WriteLine($"Learning rate: {LearningRate}");
            LearningSteps = 0;
        }

        /// <summary>
        /// Resize matrices of rects from 1xN to NxM.
        /// </summary>
        /// <returns>Resized matrices.</returns>
        public Rect[] GetCompressedRects()
        {
            var matrixBuilder = Matrix<double>.Build;
            foreach (var rect in _rects)
            {
                var processedVectorR = matrixBuilder.Dense(ImageToRects.N, ImageToRects.M, ProcessVector(rect.MatrixR).ToColumnMajorArray());
                var processedVectorG = matrixBuilder.Dense(ImageToRects.N, ImageToRects.M, ProcessVector(rect.MatrixG).ToColumnMajorArray());
                var processedVectorB = matrixBuilder.Dense(ImageToRects.N, ImageToRects.M, ProcessVector(rect.MatrixB).ToColumnMajorArray());

                rect.MatrixR = processedVectorR;
                rect.MatrixG = processedVectorG;
                rect.MatrixB = processedVectorB;
            }

            return _rects;
        }

        /// <summary>
        /// Process one rect without calculating the error.
        /// </summary>
        /// <param name="rect">The rect to process.</param>
        /// <returns></returns>
        private void ProcessRect(Rect rect)
        {
            ProcessVector(rect.MatrixR);
            ProcessVector(rect.MatrixG);
            ProcessVector(rect.MatrixB);
        }

        /// <summary>
        /// Processes one rect and outputs the error for the rect.
        /// </summary>
        /// <param name="rect">The rect to process</param>
        /// <returns>Error for the rect.</returns>
        private double ProcessRectGetError(Rect rect)
        {
            var error = ProcessVectorGetDelta(rect.MatrixR);
            error += ProcessVectorGetDelta(rect.MatrixG);
            error += ProcessVectorGetDelta(rect.MatrixB);

            return error;
        }
        /// <summary>
        /// Process the input vector.
        /// </summary>
        /// <param name="vector">Vector of values to process.</param>
        /// <returns>Square mean error between input and output vectors.</returns>
        private double ProcessVectorGetDelta(Matrix<double> vector)
        {
            var y = vector * _firstLayerWeights;
            var x1 = y * _secondLayerWeights;
            var delta = x1 - vector;

            var vec = Vector<double>.Build.DenseOfArray(delta.ToRowMajorArray());
            vec = vec.Map(f => f * f);
            return vec.Sum() / vec.Count;
        }

        /// <summary>
        /// Processed the vector without calculating the error.
        /// </summary>
        /// <param name="vector">The vector to process.</param>
        /// <returns>Processed vector.</returns>
        private Matrix<double> ProcessVector(Matrix<double> vector)
        {
            var y = vector * _firstLayerWeights;
            var x1 = y * _secondLayerWeights;
            var delta = x1 - vector;

            _firstLayerWeights -= LearningRate * vector.Transpose() * delta * _secondLayerWeights.Transpose();
            _secondLayerWeights -= LearningRate * y.Transpose() * delta;

            return x1;
        }
    }
}