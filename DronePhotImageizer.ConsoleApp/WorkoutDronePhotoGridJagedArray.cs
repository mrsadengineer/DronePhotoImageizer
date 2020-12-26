using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;



using CommonImageProcessClassLibrary;

namespace DronePhotoImageizer.ConsoleApp
{
    class WorkoutDronePhotoGridJagedArray
    {

        //count the number of  rows and columns 
        //based on the number up and downs in gps values

        //maybe create List<int[]> or something similiar.


        string home = @"E:\__WORKING_DATA_RESIZED";



            /*
            //float hightestlat = 0;
            //float lowestlat = 0;

            //float highestlong = 0;
            //float lowestlong = 0;
            float lastColumnHighLat = 0;
            float lastColumnLowLat = 0;
            //List<float> lats = new List<float>();
            //List<float> longs = new List<float>();
            */


        public WorkoutDronePhotoGridJagedArray()
        {


            string[] files = System.IO.Directory.GetFiles(home);
            float currentlat;
            float currentlong;
            bool isLatMovingUp = true;

            List<int> numbofColums = new List<int>();
            float previousItemLat = 0;



            int columncount = 0;
            int totalCount = 0;
            int totalColumnCount = 0;



            bool isFirst = true;

            foreach (var item in files)
            {

                Console.WriteLine(item);

                Image image = new Bitmap(item);
                currentlat = (float)ImageMetadataReader.GetLatitude(image);
                currentlong = (float)ImageMetadataReader.GetLongitude(image);


    

                //counting columns in in rows. final number of list and int is dimension of 




                //assuming first is at the lowest latitude degree. Each higher latitude image is added to the first row.
                //After shift, each latitued that is getting smaller will be in that row. It goes back when the 
                //lat goes the other way. It goes on like this like a zigzag. 
                if (isFirst)
                {
                    previousItemLat = currentlat;
                    isFirst = false;
                }
                if (isLatMovingUp)
                {
                    if (previousItemLat > currentlat)
                    {
                        Console.WriteLine(item);
                        numbofColums.Add(columncount);
                        isLatMovingUp = false;
                        //lastColumnHighLat = currentlat;
                        columncount = 0;
                        totalColumnCount++;
                    }

                }
                //counting columns moving other direction in row
                else if (previousItemLat < currentlat)
                {
                    isLatMovingUp = true;
                    numbofColums.Add(columncount);
                    columncount = 0;
                    //lastColumnLowLat = currentlat;
                    totalColumnCount++;
                }

                previousItemLat = currentlat;
                columncount++;//adding column to working row
                totalCount++;

            }
            // Console.ReadKey();
            //print conclusiong : total 42 ranges between 58-60;

            Console.WriteLine("");
            Console.WriteLine("########################################");
            for (int i = 0; i < numbofColums.Count; i++)
            {
                Console.Write("ROW: ");
                Console.Write(i.ToString());
                Console.Write(" COLUMN COUNT: ");
                Console.WriteLine(numbofColums[i]);
            }

            Console.WriteLine("########################################");
            Console.WriteLine("");


        }



    private void ListImagesAndGPSInfoWithMinAndMax()
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

#region

//if (lowestlat == 0)
//{
//    lowestlat = currentlat;
//}

//if (lowestlong == 0)
//{
//    lowestlong = currentlong;
//}

//if (currentlat > hightestlat)
//{
//    hightestlat = currentlat;
//}
//else if (currentlat < lowestlat)
//{
//    lowestlat = currentlat;
//}

//if (Math.Abs(currentlong) > Math.Abs(highestlong))
//{
//    highestlong = currentlong;
//}
//else if (Math.Abs(currentlong) < Math.Abs(lowestlong))
//{
//    lowestlong = currentlong;
//}

#endregion