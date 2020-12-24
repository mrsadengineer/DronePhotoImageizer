using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using CommonImageProcessClassLibrary;

namespace DronePhotoImageizer.ConsoleApp
{
    class PrintImageGPSInfo
    {

        string home = @"E:\__WORKING_DATA_RESIZED";
        string fileEnd = @"00061.jpg";

        //second instance of using project reference from common class library
        public PrintImageGPSInfo()
        {
            var filepath = System.IO.Path.Combine(home, fileEnd);
            Console.WriteLine(filepath);
            Image image = new Bitmap(filepath);
            Console.Write("Lat: ");
            Console.WriteLine(ImageMetadataReader.GetLatitude(image).ToString());
            Console.Write("Long: ");
            Console.WriteLine(ImageMetadataReader.GetLongitude(image).ToString());
        }
    }
}
