using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DronePhotoImageizer.WpfClient.Models
{
    public class CustomTwoClassificationImagePredictionResults : INotifyPropertyChanged
    {
        //public string predictionId { get; set; }
        private string _predictionId;
        //public string impageOriginalPath { get; set; }
        private string _imageOriginalPath;
        //public string ModleOutputPrediction { get; set; }
        private string _modelOutputPrediction;
        //public string ModelOutputScore { get; set; }
        private string _modelOutputscore;







        public string PredictionId
        {
            get { return _predictionId; }
            set { _predictionId = value; 
                RaisePropertyChangedEvent("PredictionId"); }
        }


        public string ImageOriginalPath
        {
            get { return _imageOriginalPath; }
            set
            {
                _imageOriginalPath = value;
                RaisePropertyChangedEvent("ImageOriginalPath");
            }
        }

        public string ModelOutputPrediction

        {
            get { return _modelOutputPrediction; }
            set { _modelOutputPrediction = value; 
                RaisePropertyChangedEvent("ModelOutputPrediction"); }
        }

        public string ModelOutputscore
        {
            get { return _modelOutputscore; }
            set
            {                _modelOutputscore = value;
                RaisePropertyChangedEvent("ModelOutputscore");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
