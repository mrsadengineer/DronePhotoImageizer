using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DronePhotoImageizer.WpfClient.Models
{
    public class CustomTwoClassificationImagePredictionResults
    {
        //public string predictionId { get; set; }
        private string _predictionId;

        public string PredictionId
        {
            get { return _predictionId; }
            set { _predictionId = value; }
        }

        //public string impageOriginalPath { get; set; }
        private string _imageOriginalPath;

        public string ImageOriginalPath
        {
            get { return _imageOriginalPath; }
            set { _imageOriginalPath = value;
            
            }
        }


        //public string ModleOutputPrediction { get; set; }
        private string _modelOutputPrediction;

        public string ModelOutputPrediction

        {
            get { return _modelOutputPrediction; }
            set { _modelOutputPrediction = value; }
        }


        //public string ModelOutputScore { get; set; }
        private string _modelOutputscore;

        public string ModelOutputscore
        {
            get { return _modelOutputscore; }
            set { _modelOutputscore = value; }
        }




    }
}
