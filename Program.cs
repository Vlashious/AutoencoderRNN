// Author is Uładzimir Śniežka, student of the 821701 group.
// Used libraries are Math.NET (https://github.com/mathnet/mathnet-numerics) and System.Drawing.Common (https://github.com/dotnet/standard)
using System;

namespace MRZuIS1
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageToRects test1 = new ImageToRects("./Images/test1.jpg");
            NeuralNetwork network = new NeuralNetwork(test1.ConvertToRects());
            network.Train();
            RectsToImage image = new RectsToImage(network.GetCompressedRects(), "./Images/test1c.png");
            image.ConstructImage();
        }
    }
}
