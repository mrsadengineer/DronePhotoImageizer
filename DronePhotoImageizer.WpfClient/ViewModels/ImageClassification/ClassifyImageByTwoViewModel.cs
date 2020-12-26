using System;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

using System.Threading;

using System.Windows.Input;

using DronePhotoImageizer.WpfClient.MVVMFramework;
using DronePhotoImageizer.WpfClient.Models;
using Microsoft.ML;

namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class ClassifyImageByTwoViewModel : ObservableObject
    {


        private string _targetOutputDirectoryPath;
        private string _inputDirText;
        private string _outputDirText;
        private string _modelInputFileText;
        private int _imageClassificationCount = 0;
        private string[] _filesToProcess;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private ObservableCollection<CustomTwoClassificationImagePredictionResults> _predictedResults;
        private SynchronizationContext uiContext;






        #region constructor, worker, and completed 
        public ClassifyImageByTwoViewModel()
        {
            worker.DoWork += ImageBinaryClassificationAndDuplicateWorker;
            worker.RunWorkerCompleted += workerCompleted;
        }

        private void startGetImageFileAndClassifcationWork()
        {
            //  synchronization context in the ui thread.
            uiContext = SynchronizationContext.Current;

            worker.RunWorkerAsync();

        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //   throw new NotImplementedException();
        }
        #endregion

        #region MVVM Bindable Properties

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
        public string ModelInputFileText
        {
            get { return _modelInputFileText; }
            set
            {
                _modelInputFileText = value;
                RaisePropertyChangedEvent("ModelInputFileText");
            }
        }
        #endregion


        #region commons file/directory win32 dialog interaction

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



            }
        }

        private void SetTrainedInputModelFileMethod()
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.OpenFileDialog();
            //    dialog.InitialDirectory = inputdir_classify_txtbx.Text; // Use current value for initial dir
            //dialog.InitialDirectory = InputDirText; // Use current value for initial dir
            dialog.InitialDirectory = Environment.CurrentDirectory;
            //  sourceDirectory = inputdir_classify_txtbx.Text;
            //   sourceDirectory = InputDirText;


            dialog.Title = "Select a Trained Model"; // instead of default "Save As"
                                                     // dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
                                                     // dialog.FileName = "select"; // Filename will then be "select.this.directory"

            // Set filter for file extension and default file extension 
            dialog.DefaultExt = ".zip";
            dialog.Filter = "ZIP File (*.zip)|*.zip";//"JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";



            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                ModelInputFileText = path;
            }
        }


        #endregion

        #region MVVM Commands
        public ICommand ClassifyAndCopy
        {
            get { return new DelegateCommand(startGetImageFileAndClassifcationWork); }
        }
        public ICommand SetInputDirectory
        {
            get { return new DelegateCommand(SetInputDirectoryMethod); }
        }
        public ICommand SetOutputDirectory
        {
            get { return new DelegateCommand(SetOutputDirectoryMethod); }
        }
        public ICommand SetTrainedInputModelFile
        {
            get { return new DelegateCommand(SetTrainedInputModelFileMethod); }
        }

        #endregion



        #region predictions


        private void ImageBinaryClassificationAndDuplicateWorker(object sender, DoWorkEventArgs e)
        {
            //should probably check for inputdirecttext is validated
            _filesToProcess = Directory.GetFiles(InputDirText);
            //I need property and feild for data binding of the text box for model location
            //modelFilePath = 
            _predictedResults = new ObservableCollection<CustomTwoClassificationImagePredictionResults>();
            _targetOutputDirectoryPath = OutputDirText;


            //create output directories
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(_targetOutputDirectoryPath);
            var inventoryDir = System.IO.Path.Combine(_targetOutputDirectoryPath, "inventory");
            Console.WriteLine(inventoryDir);
            Console.WriteLine(inventoryDir);
            var infrastructureDir = System.IO.Path.Combine(_targetOutputDirectoryPath, "infrastructure");
            Console.WriteLine(infrastructureDir);
            if (!System.IO.Directory.Exists(inventoryDir))
            {
                System.IO.Directory.CreateDirectory(inventoryDir);
            }
            if (!System.IO.Directory.Exists(infrastructureDir))
            {
                System.IO.Directory.CreateDirectory(infrastructureDir);
            }




            MLContext mlContext = new MLContext();
            // ModelOutput mop = ConsumeModel.Predict(mip, ConsumeModel.ClassicationModelEnum.classtwo);

            string modelPath = _modelInputFileText;

            //// Load model & create prediction engine
            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);

            foreach (var item in _filesToProcess)
            {
                Console.WriteLine(_imageClassificationCount.ToString());
                _imageClassificationCount++;
                Console.WriteLine(_imageClassificationCount); //parse to model input
                ModelInput mip = new ModelInput();
                mip.Label = "none";//useful for evaluation section 
                mip.ImageSource = item;





                //##########################################################

                var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

                Console.WriteLine($"number of columns is ======= {modelInputSchema.Count.ToString()}");
                foreach (var inputItem in modelInputSchema)
                {
                    Console.WriteLine($"name of column is ========={inputItem.Name}");
                }

                // Console.WriteLine($"count is ======= {modelInputSchema.}");

                // Use model to make prediction on input data
                ModelOutput result = predEngine.Predict(mip);



                string toprintDebugConsole = $"prediction class: {result.Prediction}|| score: {result.Score.FirstOrDefault()}";

                //##########################################################
                Console.WriteLine(toprintDebugConsole);
                Console.WriteLine(result.Prediction.ToString());
                Console.WriteLine(result.Score.FirstOrDefault());
                Console.WriteLine("##########################");
                Console.WriteLine(item);
                var filename = System.IO.Path.GetFileName(item);
                Console.WriteLine(filename);
                string disclass = System.IO.Path.Combine(_targetOutputDirectoryPath, result.Prediction);
                Console.WriteLine(disclass);
                var destfile = System.IO.Path.Combine(disclass, filename);
                Console.WriteLine(destfile);
                System.IO.File.Copy(item, destfile, true);



                //update ui with
                var newPredictionToUpdateOutputStatus = new CustomTwoClassificationImagePredictionResults();
                newPredictionToUpdateOutputStatus.PredictionId = _imageClassificationCount.ToString();
                newPredictionToUpdateOutputStatus.ImageOriginalPath = item.ToString();
                Console.WriteLine(item.ToString());
                newPredictionToUpdateOutputStatus.ModelOutputscore = result.Score.FirstOrDefault().ToString();
                newPredictionToUpdateOutputStatus.ModelOutputPrediction = result.Prediction;


                Console.WriteLine("number of items in observable collection");


                //from the backgroud thread in backgroudworker, I need to call the ui thread to update the listview
                uiContext.Send(x => PredictionResults.Add(newPredictionToUpdateOutputStatus), null);
                //PredictionResults.Add(newPredictionToUpdateOutputStatus);


                Console.WriteLine(PredictionResults.Count.ToString());

            }

        }

        #endregion

    }
}