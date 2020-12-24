using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;

namespace DronePhotoImageizer.ConsoleApp
{

    class PrintImageMetadataList
    {

        ASCIIEncoding encodings = new ASCIIEncoding();
        string home = @"E:\__WORKING_DATA_RESIZED";
        string fileEnd = @"00061.jpg";

       
        public PrintImageMetadataList()
        {
            var filepath = System.IO.Path.Combine(home, fileEnd);
            Console.WriteLine(filepath);
            Bitmap image = new Bitmap(filepath);
            PropertyItem[] propItems = image.PropertyItems;

            int count = 0;
            foreach (PropertyItem item in propItems)
            {
                Console.WriteLine(count.ToString());

                //this is printing out the hexadecimal id type
                Console.Write("0x");
                Console.WriteLine(item.Id.ToString("x"));


                Console.WriteLine(" ");

                //  Console.WriteLine(item.Id.ToString());

                Console.WriteLine(encodings.GetString(item.Value));
                count++;
            }

        }




    }
}
