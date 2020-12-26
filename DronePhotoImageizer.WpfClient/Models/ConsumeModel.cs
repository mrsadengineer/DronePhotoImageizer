// This file was auto-generated by ML.NET Model Builder. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ML;


namespace DronePhotoImageizer.WpfClient.Models
{
    public class ConsumeModel
    {
        // For more info on consuming ML.NET models, visit https://aka.ms/model-builder-consume
        // Method for consuming model in your app


        public enum ClassicationModelEnum{
            classtwo,
            classsix
}


        public static ModelOutput Predict(ModelInput input, ClassicationModelEnum cme)
        {

            // Create new MLContext
            MLContext mlContext = new MLContext();

            // Load model & create prediction engine
            string curdir = System.IO.Directory.GetCurrentDirectory();
            string modelnamepath;
            if (cme == ClassicationModelEnum.classtwo)
            {
                Console.WriteLine("classifying with 2 classes");
                modelnamepath = "ATS_ADICModel_Class2.zip";
            }
            else if (cme == ClassicationModelEnum.classsix)
            {
                Console.WriteLine("classifying with 6 classes");
                modelnamepath = "ATS_ADICModel_Class6.zip";
            }
            else
            {
                Console.WriteLine("there exception here or soemthing");
                modelnamepath = " ";
            }

            //            string modelPath = @"C:\Users\Sammy\AppData\Local\Temp\MLVSTools\ATS_DroneToolsML\ATS_DroneToolsML.Model\MLModel.zip";






            string modelPath = System.IO.Path.Combine(curdir, modelnamepath);
            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            Console.WriteLine($"number of columns is ======= {modelInputSchema.Count.ToString()}");
            foreach (var item in modelInputSchema)
            {
                Console.WriteLine($"name of column is ========={item.Name}");
            }

           // Console.WriteLine($"count is ======= {modelInputSchema.}");

            // Use model to make prediction on input data
            ModelOutput result = predEngine.Predict(input);
            return result;
        }


        public static ModelOutput Predict(ModelInput input, string modelFilePath)
        {
            MLContext mlContext = new MLContext();
            string modelPath = modelFilePath;

            //// Load model & create prediction engine
            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            Console.WriteLine($"number of columns is ======= {modelInputSchema.Count.ToString()}");
            foreach (var item in modelInputSchema)
            {
                Console.WriteLine($"name of column is ========={item.Name}");
            }

            // Console.WriteLine($"count is ======= {modelInputSchema.}");

            // Use model to make prediction on input data
            ModelOutput result = predEngine.Predict(input);
            return result;
        }
    }
}
