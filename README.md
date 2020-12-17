# Drone Photo Imageizer

## Introduction 
The project aims to help drone user to manage and analyze images. Drones produce lots of images.
And depending on the camera and configuration, the drone can produce many and large photos. To even begin 
to analyze it, the images have to be renamed or resorted. Resizing maybe required to avoid overloading
the image processing. This desktop app intends to automate soem of this burdon in a user interface 
to walk you through the workflow in cleaning and preparing drone photos to integrating with other APIs 
to apply trained models for image classicaion, to stitch together photo for an areial map, and
 to detect objects from that image or others. These libraries will be listed in the technologies section.   


#### todo
create generic model from data on the web  

## learning objectives
* FullStack C# (.NET 5)
* MVVM Decktop frontend.
* Console Service Server Backend (stitching? training/modelbuilding?)
* C#
* Batch processing
* EMGU.CV for bindings to OpenCV
* Select File/Directory dialogs in WPF

### functional specs
* Batch ordering, sorting, renaming, and resizing images
* batch classifying images by uploaded model
*  -- Binary class
*  -- Multi Class
* resize images to lower processing needs for large files.
  * (microsoft.ml.imageanalytics)
* Using Emgu.CV For Panorama Stitching
* object detection
    * detects lables on stiched map or single image

### wish list
* web asp.net implementation
* feature for building models from training
* feature for transfer learning
* Add features from hackathon 
* More detail image 


## Technologies
Project is created with:  

WPF Client (.NET 5)  
ML.NET  (1.5.3)


## Project status
The source code is being converted from a previous beta project. The version number is to be determined.     

## Sources
https://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html   
https://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker  
https://stackoverflow.com/questions/10315188/open-file-dialog-and-select-a-file-using-wpf-controls-and-c-sharp  

## Setup and Launch


## Other information
 
#### Placement for Future README Sub sections  
Table of contents  
Illustrations  
Examples of use  



