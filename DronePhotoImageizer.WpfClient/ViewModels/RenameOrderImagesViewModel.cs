using DronePhotoImageizer.WpfClient.MVVMFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using System.Windows.Input;

namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class RenameOrderImagesViewModel : ObservableObject
    {
        private string _statusString;
        private string _inputDirText;
        private string _outputDirText;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        public RenameOrderImagesViewModel()
        {
            worker.DoWork += startNameChanging;
            worker.RunWorkerCompleted += startRenamingAndOrderingCompleted;
        }
        private void renameAndOrderImages()
        {
            worker.RunWorkerAsync();
        }
        #region MVVM Properties and Commands
        public string StatusString
        {
            get { return _statusString; }
            set
            {
                _statusString = value;
                RaisePropertyChangedEvent("StatusString");
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

        public ICommand StartNameChange
        {
            get { return new DelegateCommand(renameAndOrderImages); }
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


        private void startNameChanging(object sender, DoWorkEventArgs e)
        {
            StatusString = "Copying and Renaming Images";
            int standingNumber = 1;
            try
            {
                if (System.IO.Directory.Exists(_inputDirText))
                {
                    //Only do top level because that is how photos are created and stored with drones.
                    List<string> dirs = new List<string>(Directory.EnumerateDirectories(_inputDirText));
                    foreach (var dir in dirs)
                    {
                        Console.WriteLine(dir);
                        var files = Directory.GetFiles(dir);
                        foreach (var file in files)
                        {
                            var fileName = standingNumber.ToString();
                            fileName = String.Concat(fileName, ".JPG");
                            var destFile = System.IO.Path.Combine(_outputDirText, fileName.PadLeft(9, '0'));
                            System.IO.File.Copy(file, destFile, true);
                            System.Console.Write("FILE NAME: ");
                            Console.WriteLine(fileName);
                            standingNumber++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Source path does not exist!");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.Write("output directory: ");
            Console.WriteLine(_outputDirText);
        }

        private void startRenamingAndOrderingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //  throw new NotImplementedException();
            StatusString = "Completed";
        }
        private void SetInputDirectoryMethod()
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();
            //    dialog.InitialDirectory = inputdir_classify_txtbx.Text; // Use current value for initial dir
            dialog.InitialDirectory = InputDirText; // Use current value for initial di
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
                //  targetDirectoryPath = OutputDirText;
            }
        }

    }
}




//https://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker
//////////////////////////
//  string[] files = System.IO.Directory.GetFiles(sourcePath);

// Copy the files and overwrite destination files if they already exist.
//foreach (string s in files)
//{
// Use static Path methods to extract only the file name from the path.
// var fileName = System.IO.Path.GetFileName(file);
//System.IO.Path.GetFileName(file);
////////////////////////////////////////
                //Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                //  Console.WriteLine($"{dirs.Count} directories found.");
            //var inventoryDir = System.IO.Path.Combine(_outputDirText, "inventory");
           // Console.Write(" directory: ");
            //Console.WriteLine(inventoryDir);
            //var infrastructureDir = System.IO.Path.Combine(_outputDirText, "infrastructure");
            //Console.WriteLine(infrastructureDir);