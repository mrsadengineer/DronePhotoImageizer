using DronePhotoImageizer.WpfClient.MVVMFramework;
using System;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DronePhotoImageizer.WpfClient.ViewModels
{
    public class ImageInspectionViewModel : ObservableObject
    {
        private Image _InspectionImage;

        public Image InspectionImage

        {
            get { return _InspectionImage; }
            set { _InspectionImage = value;
                RaisePropertyChangedEvent("OutputDirText");
            }
        }
        public ImageInspectionViewModel()
        {
      _InspectionImage = new Image();
        }
        public ICommand InspectImageCommand
        {
            get { return new DelegateCommand(LoadImage_Analysis_Button_OnClick); }
        }
        private void LoadImage_Analysis_Button_OnClick()
        {       
        
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            //dlg.DefaultExt = ".png";
            //dlg.Filter = "PNG Files (*.png)|*.png";
            Nullable<bool> dialogResult = dlg.ShowDialog();

            if (dialogResult == true)
            {
             
                _InspectionImage.Width = 300;
                _InspectionImage.Height = 200;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dlg.FileName, UriKind.Absolute);
                bitmap.EndInit();
                InspectionImage.Source = bitmap;
            }
        }
    }
}











//private void LoadImage_Analysis_Button_Click(object sender, RoutedEventArgs e)
//{
//    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
//    // Set filter for file extension and default file extension 
//    //dlg.DefaultExt = ".png";
//    //dlg.Filter = "PNG Files (*.png)|*.png";
//    Nullable<bool> dialogResult = dlg.ShowDialog();

//    if (dialogResult == true)
//    {
//        // Create Image and set its width and height  
//        //Image dynamicImage = new Image();
//        //          dynamicImage.Width = 300;
//        //          dynamicImage.Height = 200;
//        _InspectionImage.Width = 300;
//        _InspectionImage.Height = 200;
//        // Create a BitmapSource  
//        BitmapImage bitmap = new BitmapImage();
//        bitmap.BeginInit();
//        bitmap.UriSource = new Uri(dlg.FileName, UriKind.Absolute);
//        //    bitmap.UriSource = new Uri(@"C:\Books\Book WPF\How do I\ImageSample\ImageSample\Flower.JPG");
//        bitmap.EndInit();
//        // Set Image.Source  
//        //dynamicImage.Source = bitmap;
//        InspectionImage.Source = bitmap;
//        // Add Image to Window  
//        //  LayoutRoot.Children.Add(dynamicImage);
//     //   LoadImageAnalysis_ContentControl.Content = dynamicImage;
//    }
//}
