using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CommonImageProcessClassLibrary;

namespace DronePhotoImageizer.ConsoleApp
{
    class WorkoutDronePhotoGridJagedArray
    {

        //count the number of  rows and columns 
        //based on the number up and downs in gps values


        string home = @"E:\__WORKING_DATA_RESIZED";





        public WorkoutDronePhotoGridJagedArray()
        {
            string[] files = System.IO.Directory.GetFiles(home);

            float currentlat;
            float hightestlat = 0;
            float lowestlat = 0;

            float currentlong;
            float highestlong = 0;
            float lowestlong = 0;

            List<float> lats = new List<float>();
            List<float> longs = new List<float>();


            foreach (var item in files)
            {
                //var filepath = System.IO.Path.Combine(home, fileEnd);
                Console.WriteLine(item);
                Image image = new Bitmap(item);


                currentlat = (float)ImageMetadataReader.GetLatitude(image);
                currentlong = (float)ImageMetadataReader.GetLongitude(image);




                //set the lowest lat first and highest long because this dataset 
                // lowestlat = currentlat;
                //highestlong = currentlong;


                lats.Add(currentlat);
                longs.Add(currentlong);

                if (lowestlat == 0)
                {
                    lowestlat = currentlat;
                }

                if (lowestlong == 0)
                {
                    lowestlong = currentlong;
                }

                if (currentlat > hightestlat)
                {
                    hightestlat = currentlat;
                }
                else if (currentlat < lowestlat)
                {
                    lowestlat = currentlat;
                }


                if (Math.Abs(currentlong) > Math.Abs(highestlong))
                {
                    highestlong = currentlong;
                }
                else if (Math.Abs(currentlong) < Math.Abs(lowestlong))
                {
                    lowestlong = currentlong;
                }

                Console.Write("Long: ");
                Console.WriteLine(ImageMetadataReader.GetLongitude(image).ToString());

                Console.Write("Lat: ");
                Console.WriteLine(ImageMetadataReader.GetLatitude(image).ToString());
                Console.Write("Long: ");
                Console.WriteLine(ImageMetadataReader.GetLongitude(image).ToString());



            }

            Console.Write("highest lat: ");
            Console.WriteLine(hightestlat);
            Console.Write("lowest lat: ");
            Console.WriteLine(lowestlat);


            Console.Write("hightest long: ");
            Console.WriteLine(highestlong);
            Console.Write("lowest long: ");
            Console.WriteLine(lowestlong);




            Console.WriteLine(lats.Max().ToString());
            Console.WriteLine(longs.Max().ToString());
            Console.WriteLine(lats.Min().ToString());
            Console.WriteLine(longs.Min().ToString());

            Console.WriteLine(lats.Max() - lats.Min());
            Console.WriteLine(longs.Max() - longs.Min());
        }
        
    }
}
