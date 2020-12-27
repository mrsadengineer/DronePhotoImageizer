﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Input;
using DronePhotoImageizer.WpfClient.MVVMFramework;
using DronePhotoImageizer.WpfClient.Models;
using Microsoft.ML;




namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class ImageClassificationMLNETViewModel : ObservableObject
    {


        private string _statusString;
        private string _progressString;

        private string _inputDirText;
        private string _outputDirText;
        private string _modelInputFileText;
        private readonly BackgroundWorker worker;

        #region MVVM Bindable Properties
        public string StatusString
        {
            get { return _statusString; }
            set
            {
                _statusString = value;
                RaisePropertyChangedEvent("StatusString");
            }
        }
        public string ProgressString
        {
            get { return _progressString; }
            set
            {
                _progressString = value;
                RaisePropertyChangedEvent("ProgressString");
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

        #region nfile/directory methods dialog (win32) 

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

        public ICommand CancelAsyncBackgroundWorker
        {
            get { return new DelegateCommand(CancelAsyncButton_Clicked); }
        }
        #endregion
        #region Constructor and background worker logic 
        public ImageClassificationMLNETViewModel()
        {
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.DoWork += workerDoWork;
            worker.RunWorkerCompleted += workerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
        }



        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // throw new NotImplementedException();

            ProgressString = e.ProgressPercentage.ToString();
            System.Diagnostics.Debug.WriteLine(ProgressString);
        }


        private void startGetImageFileAndClassifcationWork()
        {
            //  synchronization context in the ui thread.
            // uiContext = SynchronizationContext.Current;
            worker.RunWorkerAsync();

        }
        private void CancelAsyncButton_Clicked()
        {
            worker.CancelAsync();

        }
        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //   throw new NotImplementedException();            
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                StatusString = e.Error.Message;
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                StatusString = "Canceled";
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                //StatusString = e.Result.ToString();
                if ((bool)e.Result)
                {
                    StatusString = "Success";
                }
                else
                {
                    StatusString = "Something happened";
                }
                //StatusString = "Completed";
            }
        }

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            //todo: check if member fields are not null
            e.Result = ClassifyDuplicateSortDronoPhotos(_modelInputFileText, _inputDirText, _outputDirText, worker, e);

        }

        private bool ClassifyDuplicateSortDronoPhotos(string modelInputFileText, string inputDirText, string outputDirText, BackgroundWorker worker, DoWorkEventArgs e)
        {
            StatusString = "initializing prediction engine";
            int imageClassificationCount = 0;
            int highestPercentageReached = 0;
            string[] filesToProcess = Directory.GetFiles(inputDirText);
            MLContext mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(modelInputFileText, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            StatusString = "In Progress";
            foreach (var item in filesToProcess)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    ModelInput mip = new ModelInput();
                    mip.Label = "none";//useful for evaluation section 
                    mip.ImageSource = item;
                    ModelOutput result = predEngine.Predict(mip);
                    //Degbugging
                    string toprintDebugConsole = $"prediction class: {result.Prediction}|| score: {result.Score.FirstOrDefault()}";
                    System.Diagnostics.Debug.WriteLine(imageClassificationCount++);
                    System.Diagnostics.Debug.WriteLine(item);
                    System.Diagnostics.Debug.WriteLine(toprintDebugConsole);
                    //StatusString = toprintDebugConsole;
                    var filename = System.IO.Path.GetFileName(item);
                    string disclass = System.IO.Path.Combine(outputDirText, result.Prediction);
                    //
                    if (!System.IO.Directory.Exists(disclass))
                    {
                        System.IO.Directory.CreateDirectory(disclass);
                    }
                    var destfile = System.IO.Path.Combine(disclass, filename);
                    System.IO.File.Copy(item, destfile, true);
                    //precentage
                    int percentComplete =
                        (int)((float)imageClassificationCount / (float)filesToProcess.Length * 100);
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }
            }
            return true;// all files have been classifed and sorted
        }
        #endregion

    }
}





//private string _targetOutputDirectoryPath;
//private int _imageClassificationCount = 0;
//private ObservableCollection<CustomTwoClassificationImagePredictionResults> _predictedResults;
//public ObservableCollection<CustomTwoClassificationImagePredictionResults> PredictionResults
//{
//    get { return _predictedResults; }
//    set
//    {
//        _predictedResults = value;
//        RaisePropertyChangedEvent("PredictionResults");
//    }
//}
//var inventoryDir = System.IO.Path.Combine(outputDirText, "inventory");
//Console.WriteLine(inventoryDir);
//Console.WriteLine(inventoryDir);
//var infrastructureDir = System.IO.Path.Combine(outputDirText, "infrastructure");
//Console.WriteLine(infrastructureDir);
//if (!System.IO.Directory.Exists(inventoryDir))
//{
//    System.IO.Directory.CreateDirectory(inventoryDir);
//}
//if (!System.IO.Directory.Exists(infrastructureDir))
//{
//    System.IO.Directory.CreateDirectory(infrastructureDir);
//}

//check to see if all there string items varilables exist. 
//should probably check for inputdirecttext is validated
// _filesToProcess 
//I need property and feild for data binding of the text box for model location
// _predictedResults = new ObservableCollection<CustomTwoClassificationImagePredictionResults>();
// _targetOutputDirectoryPath = OutputDirText;
//// Load model & 
//create prediction engine

//create output directories
//Console.WriteLine(outputDirText);
// Report progress as a percentage of the total task.
//update ui with
//var newPredictionToUpdateOutputStatus = new CustomTwoClassificationImagePredictionResults();
//newPredictionToUpdateOutputStatus.PredictionId = _imageClassificationCount.ToString();
//newPredictionToUpdateOutputStatus.ImageOriginalPath = item.ToString();
//Console.WriteLine(item.ToString());
//newPredictionToUpdateOutputStatus.ModelOutputscore = result.Score.FirstOrDefault().ToString();
//newPredictionToUpdateOutputStatus.ModelOutputPrediction = result.Prediction;
//from the backgroud thread in backgroudworker, I need to call the ui thread to update the listview
//uiContext.Send(x => PredictionResults.Add(newPredictionToUpdateOutputStatus), null);
//PredictionResults.Add(newPredictionToUpdateOutputStatus);
//for me
//System.Diagnostics.Debug.Write("PredictionResults Count: ");
//System.Diagnostics.Debug.WriteLine(PredictionResults.Count.ToString());