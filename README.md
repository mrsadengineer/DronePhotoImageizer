# Drone Photo Imageizer

## Introduction 
The project aims to help drone user to manage and analyze images. Drones produce lots of images.
And depending on the camera and configuration, the drone can produce many and large photos. To even begin 
to analyze it, the images have to be renamed or resorted. Resizing maybe required to avoid overloading
the image processing. This desktop app intends to automate soem of this burdon in a user interface 
to walk you through the workflow in cleaning and preparing drone photos to integrating with other APIs 
to apply trained models for image classicaion, to stitch together photo for an areial map, and
 to detect objects from that image or others. These libraries will be listed in the technologies section.   

## IIIF
Looking into IIIF


#### todo and release planning

##### version 0.3.0
[X] RenamingAndCombining algorithm for converting fresh drone photage to single directory, ordered and numbered files.  
[X] UI for classifying and duplicating two-class-output directory (infrastructure/assets)  
[ ] UI for classifying and duplicating six-class-output directory (inventory/infrastructure/vehicles/road/water/field)  
[X] Add Usercontrol for Resizing the images 


##### version beyond
[ ] Creating a grid of images by gps coordinates metadata 
[ ] User Control Stitching Map from multiple images (Emgu.CV) 
[ ] Training models from client or web service (tensorflow or ml.net)  
[ ] Emgu.CV to inspect images 
[ ] Inspect directory of images
[ ] object detection
[ ] evaluate models
[ ] add RELEASE/CHANGELOG 




## learning objectives
* FullStack C# (.NET 5)
* MVVM Decktop frontend.
* Console Service Server Backend (stitching? training/modelbuilding?)
* C#
* Batch processing
* EMGU.CV for bindings to OpenCV
* Select File/Directory dialogs in WPF
* Developing for Multiple Platforms. using class libraries
* (microsoft.ml.imageanalytics) ML.NET for classification and object detection 

### functional specs
* Batch ordering, sorting, renaming, and resizing images
* batch classifying images by uploaded model
*  -- Binary class
*  -- Multi Class
* resize images to lower processing needs for large files.
* Using Emgu.CV For Panorama/Scans Stitching
* object detection
    * detects lables on stiched map or single image
* Inspect collection of images as stats
  * number of images
  * max and min in gps metadata
  * list all metadata
  * list readable metadata
* create grid/matrix of photos by gps to begin to stitch

### technicals specs 
* x64 bit for Ml.NET
* include 


### wish list
* web asp.net implementation
* feature for building models from training
* feature for transfer learning
* Add features from hackathon 
* More detail image
* create generic model from data on the web  


## Technologies and Tools
Project is created with:  

Visual studio 2019 community  
WPF Client (.NET 5)  
ML.NET  (1.5.3)  
SciSharp.Tensorflow.Redist (2.1.0)   
SixLabors.ImageSharp (1.0.2)  


## Project status
The source code is being converted from a previous beta project. The version number is to be determined.     

## Sources
https://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html   
https://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker  
https://stackoverflow.com/questions/10315188/open-file-dialog-and-select-a-file-using-wpf-controls-and-c-sharp   
https://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker  
https://docs.microsoft.com/en-us/dotnet/framework/cross-platform/using-portable-class-library-with-model-view-view-model  
https://www.exiv2.org/tags.html  
https://stackoverflow.com/questions/4983766/getting-gps-data-from-an-images-exif-in-c-sharp  



## Setup and Launch


## Other information
 
#### Placement for Future README Sub sections  
Table of contents  
Illustrations  
Examples of use  



