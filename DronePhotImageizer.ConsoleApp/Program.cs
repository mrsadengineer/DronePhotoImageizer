using System;

namespace DronePhotoImageizer.ConsoleApp
{
    class Program
    {

        public const string imageHomeDirectory = @"E:\__WORKING_DATA_RESIZED";

        

        static void Main(string[] args)
        {
            Console.WriteLine("Exploring EXIF Image Metadata");


            //DisplayImageGPSMetadata display = new DisplayImageGPSMetadata();

            //PrintImageMetadataList pri = new PrintImageMetadataList();

            // ListGPSCoordinatesInDirectory lgcd = new ListGPSCoordinatesInDirectory();

            //   PrintImageGPSInfo pigi = new PrintImageGPSInfo();

            WorkoutDronePhotoGridJagedArray wdpdja = new WorkoutDronePhotoGridJagedArray();


            Console.WriteLine("end");
            Console.ReadKey();
        }
    }
}
