// Author is Uładzimir Śniežka, student of the 821701 group.
// Used libraries are Math.NET (https://github.com/mathnet/mathnet-numerics) and System.Drawing.Common (https://github.com/dotnet/standard)

namespace MRZuIS1
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageToRects test1 = new ImageToRects(args[0]);
            NeuralNetwork network = new NeuralNetwork(test1.ConvertToRects());
            network.Train();
            RectsToImage image = new RectsToImage(network.GetCompressedRects(), args[1]);
            image.ConstructImage();
        }
    }
}
