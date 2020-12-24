using System;

namespace DronePhotImageizer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Exploring EXIF Image Metadata");


            DisplayImageGPSMetadata display = new DisplayImageGPSMetadata();
        }
    }
}
