using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MRZuIS1
{
    class RectsToImage
    {
        private readonly Rect[] _rects;
        private readonly Bitmap _bitmap;
        private readonly (Point bP, Point eP)[] _ps;
        private readonly string _filePath;

        /// <summary>
        /// Converts processed rects back to image format.
        /// </summary>
        /// <param name="rects">Process rects to restore.</param>
        /// <param name="filePath">Path to the new image.</param>
        public RectsToImage(Rect[] rects, string filePath)
        {
            _rects = rects;
            _bitmap = new Bitmap(256, 256);
            _ps = ImageToRects.RectCoordinates;
            _filePath = filePath;
        }

        /// <summary>
        /// Builds the image from the given rects.
        /// </summary>
        public void ConstructImage()
        {
            var index = 0;
            foreach (var ps in _ps)
            {
                var rowIndex = 0;
                var colIndex = 0;
                for (int i = ps.bP.Y; i < ps.eP.Y; i++, rowIndex++)
                {
                    for (int j = ps.bP.X; j < ps.eP.X; j++, colIndex++)
                    {
                        var red = Math.Clamp(255 * (_rects[index].MatrixR.At(rowIndex, colIndex) + 1) / 2, 0, 255);
                        var green = Math.Clamp(255 * (_rects[index].MatrixG.At(rowIndex, colIndex) + 1) / 2, 0, 255);
                        var blue = Math.Clamp(255 * (_rects[index].MatrixB.At(rowIndex, colIndex) + 1) / 2, 0, 255);
                        var color = Color.FromArgb((int)red, (int)green, (int)blue);
                        _bitmap.SetPixel(i, j, color);
                    }
                    colIndex = 0;
                }
                index++;
            }

            Image image = new Bitmap(_bitmap);
            image.Save(_filePath, ImageFormat.Png);
        }
    }
}