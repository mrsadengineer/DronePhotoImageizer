using DronePhotoImageizer.WpfClient.MVVMFramework;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;


using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using System.Windows.Input;
using Image = SixLabors.ImageSharp.Image;

namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class ResizeDuplicatesViewModel : ObservableObject
    {
        private string _statusString;
        private string _inputDirText;
        private string _outputDirText;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        public ResizeDuplicatesViewModel()
        {
            worker.DoWork += startResizingImages;
            worker.RunWorkerCompleted += startRenamingAndOrderingCompleted;
        }
        private void resizeAndTransferDuplicates()
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

        public ICommand StartImageResizing
        {
            get { return new DelegateCommand(resizeAndTransferDuplicates); }
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


        private void startResizingImages(object sender, DoWorkEventArgs e)
        {
            StatusString = "Starting to Resize";
            //int numberOfImageResizes = 1;

            try
            {
                if (System.IO.Directory.Exists(_inputDirText))
                {
                    //Only do top level because that is how photos are created and stored with drones.
                    // List<string> dirs = new List<string>(Directory.EnumerateDirectories(_inputDirText));
                    // foreach (var dir in dirs)
                    //   {

                    Console.Write("output directory: ");
                    Console.WriteLine(_outputDirText);

                    //var mlContext = new MLContext();
                    Console.WriteLine(_inputDirText);
                    var files = Directory.GetFiles(_inputDirText);
                    //System.IO.Directory.CreateDirectory("output");

                    foreach (var file in files)
                    {
                        using (Image image = Image.Load(file))
                        {
                            image.Mutate(x => x
                                 .Resize(image.Width / 40, image.Height / 40));
                            // .Grayscale());
                            var tosaveas = Path.Combine(_outputDirText, file.Substring(file.LastIndexOf('.') - 5, 9));
                            image.Save(tosaveas); // Automatic encoder selected based on extension.
                        }
                    }



                    //IEnumerable<ImageData> images = LoadImagesFromDirectory(folder: _inputDirText, useFolderNameAsLabel: true);
                    //IDataView imageData = mlContext.Data.LoadFromEnumerable(images);

                    //var pipeline = mlContext.Transforms.LoadImages("ImageObject", _inputDirText, "ImagePath")
                    //    .Append(mlContext.Transforms.ResizeImages("ImageObjectResized",
                    //    inputColumnName: "ImageObject", imageWidth: 100, imageHeight: 100));

                    //var transformedData = pipeline.Fit(imageData).Transform(imageData);

                    //transformedData.Preview();
                    //PrintColumns(transformedData);

                    // IDataView dataView = mlContext.Data.
                    //var transformedData = pipeline.Fit()


                    //foreach (var file in files)
                    //{
                    //    //var fileName = numberOfImageResizes.ToString();
                    //    //fileName = String.Concat(fileName, ".JPG");


                    //    var destFile = System.IO.Path.Combine(_outputDirText, "resized" + file.ToString());
                    //    System.IO.File.Copy(file, destFile, true);
                    //    System.Console.Write("FILE NAME: ");
                    //    Console.WriteLine(file);
                    //    numberOfImageResizes++;



                    //}
                    // }
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




        private static IEnumerable<ImageData> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        {
            var files = Directory.GetFiles(folder, "*",
                searchOption: SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
                    continue;

                var label = Path.GetFileName(file);

                if (useFolderNameAsLabel)
                    label = Directory.GetParent(file).Name;
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new ImageData()
                {
                    ImagePath = file,
                    Label = label
                };
            }
        }



        private static void PrintColumns(IDataView transformedData)
        {
            // The transformedData IDataView contains the loaded images now.
            Console.WriteLine("{0, -25} {1, -25} {2, -25}", "ImagePath", "Name",
                "ImageObject");

            using (var cursor = transformedData.GetRowCursor(transformedData
                .Schema))
            {
                // Note that it is best to get the getters and values *before*
                // iteration, so as to facilitate buffer sharing (if applicable),
                // and column-type validation once, rather than many times.
                ReadOnlyMemory<char> imagePath = default;
                ReadOnlyMemory<char> name = default;
                Bitmap imageObject = null;

                var imagePathGetter = cursor.GetGetter<ReadOnlyMemory<char>>(cursor
                    .Schema["ImagePath"]);

                var nameGetter = cursor.GetGetter<ReadOnlyMemory<char>>(cursor
                    .Schema["Name"]);

                var imageObjectGetter = cursor.GetGetter<Bitmap>(cursor.Schema[
                    "ImageObject"]);

                while (cursor.MoveNext())
                {

                    imagePathGetter(ref imagePath);
                    nameGetter(ref name);
                    imageObjectGetter(ref imageObject);

                    Console.WriteLine("{0, -25} {1, -25} {2, -25}", imagePath, name,
                        imageObject.PhysicalDimension);
                }

                // Dispose the image.
                imageObject.Dispose();
            }
        }




    }

    class ImageData
    {
        public string ImagePath { get; set; }

        public string Label { get; set; }
    }

    //class ModelInput
    //{
    //    public byte[] Image { get; set; }

    //    public UInt32 LabelAsKey { get; set; }

    //    public string ImagePath { get; set; }

    //    public string Label { get; set; }
    //}

    //class ModelOutput
    //{
    //    public string ImagePath { get; set; }

    //    public string Label { get; set; }

    //    public string PredictedLabel { get; set; }
    //}



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