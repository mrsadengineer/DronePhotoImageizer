using System;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

using System.Threading;

using System.Windows.Input;

using DronePhotoImageizer.WpfClient.MVVMFramework;
using DronePhotoImageizer.WpfClient.Models;


namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class ClassifyImageByTwoViewModel : ObservableObject
    {


        private string targetDirectoryPath;
        private string _inputDirText;
        private string _outputDirText;

        private int _imageClassificationCount;
        private string[] filesToProcess;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private ObservableCollection<CustomTwoClassificationImagePredictionResults> _predictedResults;

        public ClassifyImageByTwoViewModel()
        {
            _imageClassificationCount = 0;
            PredictionResults = new ObservableCollection<CustomTwoClassificationImagePredictionResults>();
            worker.DoWork += startClassifying;
            worker.RunWorkerCompleted += startClassifyingCompleted;

        }




        public ObservableCollection<CustomTwoClassificationImagePredictionResults> PredictionResults

        {
            get { return _predictedResults; }
            set
            {
                _predictedResults = value;
                RaisePropertyChangedEvent("PredictionResults");
            }
        }


        public string InputDirText
        {
            get { return _inputDirText; }
            set
            {
                _inputDirText = value;
                RaisePropertyChangedEvent("InputDirText");
            }
        }
        public string OutputDirText
        {
            get { return _outputDirText; }
            set
            {
                _outputDirText = value;
                RaisePropertyChangedEvent("OutputDirText");
            }
        }
        #region commons

        private void SetInputDirectoryMethod()
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();
            //    dialog.InitialDirectory = inputdir_classify_txtbx.Text; // Use current value for initial dir
            dialog.InitialDirectory = InputDirText; // Use current value for initial dir

            //  sourceDirectory = inputdir_classify_txtbx.Text;
            //   sourceDirectory = InputDirText;


            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                InputDirText = path;
            }
        }

        private void SetOutputDirectoryMethod()
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.InitialDirectory = OutputDirText; // Use current value for initial dir






            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                OutputDirText = path;
                targetDirectoryPath = OutputDirText;


            }
        }

        #endregion
        #region commands

        public ICommand ClassifyAndCopy
        {
            get { return new DelegateCommand(GetImageFilesAndBeginWorker); }
        }

        public ICommand SetInputDirectory
        {
            get { return new DelegateCommand(SetInputDirectoryMethod); }
        }

        public ICommand SetOutputDirectory
        {
            get { return new DelegateCommand(SetOutputDirectoryMethod); }
        }

        #endregion

        

        #region predictions


        private void startClassifying(object sender, DoWorkEventArgs e)
        {

            PredictionResults = new ObservableCollection<CustomTwoClassificationImagePredictionResults>();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(targetDirectoryPath);




            var inventoryDir = System.IO.Path.Combine(targetDirectoryPath, "inventory");
            Console.WriteLine(inventoryDir);


            Console.WriteLine(inventoryDir);
            var infrastructureDir = System.IO.Path.Combine(targetDirectoryPath, "infrastructure");
            Console.WriteLine(infrastructureDir);



            if (!System.IO.Directory.Exists(inventoryDir))
            {
                System.IO.Directory.CreateDirectory(inventoryDir);
            }


            if (!System.IO.Directory.Exists(infrastructureDir))
            {
                System.IO.Directory.CreateDirectory(infrastructureDir);
            }


            foreach (var item in filesToProcess)
            {
                _imageClassificationCount++;
                Console.WriteLine(_imageClassificationCount); //parse to model input
                ModelInput mip = new ModelInput();
                mip.Label = "none";
                mip.ImageSource = item;
                ModelOutput mop = ConsumeModel.Predict(mip, ConsumeModel.ClassicationModelEnum.classtwo);





                string toprintDebugConsole = $"prediction class: {mop.Prediction}|| score: {mop.Score.FirstOrDefault()}";

                Console.WriteLine(toprintDebugConsole);
                Console.WriteLine(mop.Prediction.ToString());
                Console.WriteLine(mop.Score.FirstOrDefault());
                Console.WriteLine("##########################");
                Console.WriteLine(item);
                var filename = System.IO.Path.GetFileName(item);
                Console.WriteLine(filename);
                string disclass = System.IO.Path.Combine(targetDirectoryPath, mop.Prediction);
                Console.WriteLine(disclass);
                var destfile = System.IO.Path.Combine(disclass, filename);
                Console.WriteLine(destfile);
                System.IO.File.Copy(item, destfile, true);


                var newPredictionToUpdateOutputStatus = new CustomTwoClassificationImagePredictionResults();
                newPredictionToUpdateOutputStatus.PredictionId = _imageClassificationCount.ToString();
                newPredictionToUpdateOutputStatus.ImageOriginalPath = item.ToString();
                Console.WriteLine(item.ToString());
                newPredictionToUpdateOutputStatus.ModelOutputscore = mop.Score.FirstOrDefault().ToString();
                newPredictionToUpdateOutputStatus.ModelOutputPrediction = mop.Prediction;


                Console.WriteLine("number of items in observable collection");


                //from the backgroud thread in backgroudworker, I need to call the ui thread to update the listview
                uiContext.Send(x => PredictionResults.Add(newPredictionToUpdateOutputStatus), null);
                //PredictionResults.Add(newPredictionToUpdateOutputStatus);


                Console.WriteLine(PredictionResults.Count.ToString());

            }

        }

        private SynchronizationContext uiContext;
        private void GetImageFilesAndBeginWorker()
        {

            //  var uiContext = SynchronizationContext.Current;

            //  synchronization context in the ui thread.
            uiContext = SynchronizationContext.Current;
            filesToProcess = Directory.GetFiles(InputDirText);
            worker.RunWorkerAsync();

        }

        private void startClassifyingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
         //   throw new NotImplementedException();
        }
        #endregion

    }
}










//https://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker