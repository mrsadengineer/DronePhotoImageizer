
using DronePhotoImageizer.WpfClient.MVVMFramework;
using DronePhotoImageizer.WpfClient.Models;
using System;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

using System.Threading;

using System.Windows.Input;

namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class ClassifyImageBySixViewModel : ObservableObject
    {


        private string targetDirectoryPath;
        private string _inputDirText;
        private string _outputDirText;

        private int _imageClassificationCount = 0;
        private string[] filesToProcess;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private ObservableCollection<CustomTwoClassificationImagePredictionResults> _predictedResults;

        //private ReadOnlyDictionary< labelPathDictionary


        public ClassifyImageBySixViewModel()//constructor
        {
            worker.DoWork += startClassifying;
            worker.RunWorkerCompleted += startClassifyingCompleted;

        }

        #region MVVM Properties and Commands

        public ICommand ClassifyAndCopy
        {
            get { return new DelegateCommand(GetImageFilesAndBeginWorker); }
        }

        public ICommand SetInputDirectory
        {
            get { return new DelegateCommand(SetInputDirectoryMethod); } //win32 directory select dialog
        }

        public ICommand SetOutputDirectory
        {
            get { return new DelegateCommand(SetOutputDirectoryMethod); } //win32 directory select dialog
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
        #endregion

        #region commons file/directory win32 interaction

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



        #region predictions


        private void startClassifying(object sender, DoWorkEventArgs e)
        {

            PredictionResults = new ObservableCollection<CustomTwoClassificationImagePredictionResults>();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(targetDirectoryPath);


            // set up output paths for all classes. 

            var inventoryDir = System.IO.Path.Combine(targetDirectoryPath, "inventory");            /////////// inventory

            if (!System.IO.Directory.Exists(inventoryDir))
            {
                System.IO.Directory.CreateDirectory(inventoryDir);
            }


            var infrastructureDir = System.IO.Path.Combine(targetDirectoryPath, "infrastructure");            /////////////////infrasturcture
            Console.WriteLine(infrastructureDir);
            if (!System.IO.Directory.Exists(infrastructureDir))
            {
                System.IO.Directory.CreateDirectory(infrastructureDir);
            }


            var fieldDir = System.IO.Path.Combine(targetDirectoryPath, "field");            /////////field
            Console.WriteLine(fieldDir);
            if (!System.IO.Directory.Exists(fieldDir))
            {
                System.IO.Directory.CreateDirectory(fieldDir);
            }


            var vehiclesDir = System.IO.Path.Combine(targetDirectoryPath, "vehicles");            ///////////vehicles
            Console.WriteLine(vehiclesDir);
            if (!System.IO.Directory.Exists(vehiclesDir))
            {
                System.IO.Directory.CreateDirectory(vehiclesDir);
            }

            var roadDir = System.IO.Path.Combine(targetDirectoryPath, "road");            //////////road
            Console.WriteLine(roadDir);
            if (!System.IO.Directory.Exists(roadDir))
            {
                System.IO.Directory.CreateDirectory(roadDir);
            }

            var waterDir = System.IO.Path.Combine(targetDirectoryPath, "water");            ///////////water
            Console.WriteLine(waterDir);
            if (!System.IO.Directory.Exists(waterDir))
            {
                System.IO.Directory.CreateDirectory(waterDir);
            }


            foreach (var item in filesToProcess)
            {
                _imageClassificationCount++;
                Console.WriteLine(_imageClassificationCount); //parse to model input
                ModelInput mip = new ModelInput();
                mip.Label = "none";
                mip.ImageSource = item;
                ModelOutput mop = ConsumeModel.Predict(mip, ConsumeModel.ClassicationModelEnum.classsix);





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
