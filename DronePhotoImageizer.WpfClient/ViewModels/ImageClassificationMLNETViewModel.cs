using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Input;
using DronePhotoImageizer.WpfClient.MVVMFramework;
using DronePhotoImageizer.WpfClient.Models;
using Microsoft.ML;
using System.Collections.Generic;
using System.Windows.Threading;

namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class ImageClassificationMLNETViewModel : ObservableObject
    {

        private readonly BackgroundWorker worker;

        #region MVVM Bindable Public Properties and Private Fields
        
        private string _statusString;
        public string StatusString
        {
            get { return _statusString; }
            set
            {
                _statusString = value;
                RaisePropertyChangedEvent("StatusString");
            }
        }

        private string _progressString;
        public string ProgressString
        {
            get { return _progressString; }
            set
            {
                _progressString = value;
                RaisePropertyChangedEvent("ProgressString");
            }
        }

        private string _inputDirText;
        public string InputDirText
        {
            get { return _inputDirText; }
            set
            {
                _inputDirText = value;
                RaisePropertyChangedEvent("InputDirText");
            }
        }

        private string _outputDirText;
        public string OutputDirText
        {
            get { return _outputDirText; }
            set
            {
                _outputDirText = value;
                RaisePropertyChangedEvent("OutputDirText");
            }
        }
        
        private string _modelInputFileText;
        public string ModelInputFileText
        {
            get { return _modelInputFileText; }
            set
            {
                _modelInputFileText = value;
                RaisePropertyChangedEvent("ModelInputFileText");
            }
        }

        private string _itemNameString;
        public string ItemNameString
        {
            get { return _itemNameString; }
            set
            {
                _itemNameString = value;
                RaisePropertyChangedEvent("ItemNameString");
            }
        }

        private string _classificationString;
        public string ClassificationString
        {
            get { return _classificationString; }
            set
            {
                _classificationString = value;
                RaisePropertyChangedEvent("ClassificationString");
            }
        }
        
        private string _scoreString;
        public string ScoreString
        {
            get { return _scoreString; }
            set
            {
                _scoreString = value;
                RaisePropertyChangedEvent("ScoreString");
            }
        }
        
        private string _imageCountString;
        public string ImageCountString
        {
            get { return _imageCountString; }
            set
            {
                _imageCountString = value;
                RaisePropertyChangedEvent("ImageCountString");
            }
        }

        private ObservableCollection<string> _classificationListOfStrings;
        public ObservableCollection<string> ClassificationListOfStrings
        {
            get { return _classificationListOfStrings; }
            set
            {
                _classificationListOfStrings = value;
                RaisePropertyChangedEvent("ClassificationListOfStrings");
            }
        }




        #endregion
        
        #region File/Directory Dialog (win32) Methods 

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
        
        #region Constructor and BackgroundWorker Logic 
        
        public ImageClassificationMLNETViewModel()
        {
            _classificationListOfStrings = new ObservableCollection<string>(); 
            uiContext = AsyncOperationManager.SynchronizationContext;
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.DoWork += workerDoWork;
            worker.RunWorkerCompleted += workerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;


        }

        SynchronizationContext uiContext;// = SynchronizationContext
        private void startGetImageFileAndClassifcationWork()
        {
            ProgressString = "0";
            //  synchronization context in the ui thread.
            //var 
                uiContext = SynchronizationContext.Current;

            worker.RunWorkerAsync();

        }
        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            //todo: check if member fields are not null
            e.Result = ClassifyDuplicateSortDronoPhotos(_modelInputFileText, _inputDirText, _outputDirText, worker, e);

        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressString = e.ProgressPercentage.ToString();
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
            }
        }


        private void UpdateListBox()
        {//not using, but storing now to consider using delegate methods for updating UI thread from bgworker. Not tested,
            //but considering it as alternative. 
            ClassificationListOfStrings.Add(_classificationString);
        }

        private delegate void UpdateUIDelegate();

        private bool ClassifyDuplicateSortDronoPhotos(string modelInputFileText, string inputDirText, string outputDirText, BackgroundWorker worker, DoWorkEventArgs e)
        {
            StatusString = "initializing prediction engine";
            int imageClassificationCount = 0;
            ImageCountString = imageClassificationCount.ToString();
            int highestPercentageReached = 0;
            string[] filesToProcess = Directory.GetFiles(inputDirText);
            MLContext mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(modelInputFileText, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
            

            foreach (var item in filesToProcess)
            {
            StatusString = "In Progress";
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
                    //Updating UI Properties on Predictions
                    ItemNameString = item;
                    ClassificationString = result.Prediction;
                    ScoreString = result.Score.FirstOrDefault().ToString();
                    ImageCountString = imageClassificationCount.ToString();
                    //create destination and/or save new file to destination location and prediction
                    var filename = System.IO.Path.GetFileName(item);
                    string disclass = System.IO.Path.Combine(outputDirText, result.Prediction);
                    //TODO: find another way to compare and upload UI on the classifications found from the model.
                    if (!System.IO.Directory.Exists(disclass))
                    {
                        // uiContext.Send(x => ClassificationListOfStrings.Add(result.Prediction.ToString()), null);
                        uiContext.Post(x => ClassificationListOfStrings.Add(result.Prediction.ToString()), null);
                        //Dispatcher.CurrentDispatcher.Invoke(new Action(delegate ()
                        //{
                        //    ClassificationListOfStrings.Add(result.Prediction.ToString());
                        //}));
                        //ClassificationListOfStrings.Add(result.Prediction.ToString());
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




