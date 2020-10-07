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
