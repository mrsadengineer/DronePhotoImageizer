using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing.Imaging;
using System.Drawing;

using CommonImageProcessClassLibrary;

namespace DronePhotoImageizer.ConsoleApp
{
    class ListGPSCoordinatesInDirectory
    {

        string home = @"E:\__WORKING_DATA_RESIZED";



        //first instance of using class library with project solution
        public ListGPSCoordinatesInDirectory()
        {
            string[] files = System.IO.Directory.GetFiles(home);
            foreach (var item in files)
            {
                //var filepath = System.IO.Path.Combine(home, fileEnd);
                Console.WriteLine(item);
                Image image = new Bitmap(item);
                Console.Write("Lat: ");
                Console.WriteLine(ImageMetadataReader.GetLatitude(image).ToString());
                Console.Write("Long: ");
                Console.WriteLine(ImageMetadataReader.GetLongitude(image).ToString());
            }
        }
    }
}
