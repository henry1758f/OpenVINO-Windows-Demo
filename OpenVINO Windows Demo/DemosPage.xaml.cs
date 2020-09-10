using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenVINO_Windows_Demo
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class DemosPage : Page
    {
        public DemosPage()
        {
            this.InitializeComponent();
        }
        public Demo_and_Sample_List[] Demos { get; } = new Demo_and_Sample_List[]
        {
            ("Interactive Face Detection C++ Demo",
            "PATH",
            "This demo showcases Object Detection task applied for face recognition using sequence of neural networks. Async API can improve overall frame-rate of the application, because rather than wait for inference to complete, the application can continue operating on the host while accelerator is busy. This demo executes four parallel infer requests for the Age/Gender Recognition, Head Pose Estimation, Emotions Recognition, and Facial Landmarks Detection networks that run simultaneously.",
            "https://docs.openvinotoolkit.org/latest/_demos_interactive_face_detection_demo_README.html",
            "/Assets/Interactive_Face_Detection_C++_Demo.png",
            new string[]{  }
            ),
            ("Crossroad Camera C++ Demo",
            "PATH",
            "This demo provides an inference pipeline for persons' detection, recognition and reidentification. The demo uses Person Detection network followed by the Person Attributes Recognition and Person Reidentification Retail networks applied on top of the detection results.",
            "https://docs.openvinotoolkit.org/latest/_demos_crossroad_camera_demo_README.html",
            "/Assets/Crossroad_Camera_Demo.png",
            new string[]{  }
            ),
            ("Face Recognition Demo",
            "PATH",
            "This example demonstrates an approach to create interactive applications for video processing. It shows the basic architecture for building model pipelines supporting model placement on different devices and simultaneous parallel or sequential execution using OpenVINO library in Python. In particular, this demo uses 3 models to build a pipeline able to detect faces on videos, their keypoints (aka \"landmarks\"), and recognize persons using the provided faces database (the gallery).",
            "https://docs.openvinotoolkit.org/latest/_demos_crossroad_camera_demo_README.html",
            "/Assets/Face_Recognition_Demo.png",
            new string[]{ "2020.4" }
            ),
            ("Human Pose Estimation C++ Demo",
            "PATH",
            "This demo showcases the work of multi-person 2D pose estimation algorithm. The task is to predict a pose: body skeleton, which consists of keypoints and connections between them, for every person in an input video. The pose may contain up to 18 keypoints: ears, eyes, nose, neck, shoulders, elbows, wrists, hips, knees, and ankles. Some of potential use cases of the algorithm are action recognition and behavior understanding. ",
            "",
            "/Assets/Human_Pose_Estimation_Demo.png",
            new string[]{  }
            ),
            ("Gaze Estimation Demo",
            "PATH",
            "This demo showcases the work of gaze estimation model.",
            "",
            "/Assets/Gaze_Estimation_Demo.png",
            new string[]{  }
            ),
            ("Image Segmentation C++ Demo",
            "PATH",
            "This topic demonstrates how to run the Image Segmentation demo application, which does inference using semantic segmentation networks.",
            "https://docs.openvinotoolkit.org/latest/_demos_crossroad_camera_demo_README.html",
            "/Assets/Image_Segmentation_Demo.png",
            new string[]{  }
            ),
            ("Face Recognition Demo Azure IoT Showcase",
            "PATH",
            "This example demonstrates an approach to create interactive applications for video processing. It shows the basic architecture for building model pipelines supporting model placement on different devices and simultaneous parallel or sequential execution using OpenVINO library in Python. In particular, this demo uses 3 models to build a pipeline able to detect faces on videos, their keypoints (aka \"landmarks\"), and recognize persons using the provided faces database (the gallery).",
            "https://docs.openvinotoolkit.org/latest/_demos_crossroad_camera_demo_README.html",
            "/Assets/Face_Recognition_Demo_Azure.png",
            new string[]{  }
            )
        };

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Interactive_Face_Detection_Demo_Page", typeof(Demos.Interactive_Face_Detection_Demo_Page)),
        };

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var itemName = e.ClickedItem;
            Demo_and_Sample_List item = (Demo_and_Sample_List)e.ClickedItem;
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

            foreach (string str in item.not_support_version)
            {
                if(localSettings.Values["OpenVINO"].ToString().Contains(str))
                {
                    MessageDialog messageDialogs = new MessageDialog(resourceLoader.GetString("Demo_UnSupportOpenVINOver"), resourceLoader.GetString("Error"));
                    await messageDialogs.ShowAsync();
                    return;
                }
            }
            if(item.Name == "Interactive Face Detection C++ Demo")
            {
                //messageDialog.ShowAsync();
                //var frame = (Frame)Window.Current.Content;
                //frame.Navigate(typeof(Demos.Interactive_Face_Detection_Demo_Page));

                this.Frame.Navigate(typeof(Demos.Interactive_Face_Detection_Demo_Page),null);
                //MainPage.ContentFrame.Navigate(typeof(Demos.Interactive_Face_Detection_Demo_Page));
                //MainPage.NavView_Navigate("Interactive_Face_Detection_Demo_Page", null);

            }
            else if (item.Name == "Crossroad Camera C++ Demo")
            {
                this.Frame.Navigate(typeof(Demos.Crossroad_Camera_Demo_Page), null);
            }
            else if (item.Name == "Face Recognition Demo")
            {
                this.Frame.Navigate(typeof(Demos.Face_Recognition_Demo_Page), null);
            }
            else if (item.Name == "Human Pose Estimation C++ Demo")
            {
                this.Frame.Navigate(typeof(Demos.Human_Pose_Estimation_Demo_Page), null);
            }
            else if (item.Name == "Gaze Estimation Demo")
            {
                this.Frame.Navigate(typeof(Demos.Gaze_Estimation_Demo_Page), null);
            }
            else if (item.Name == "Image Segmentation C++ Demo")
            {
                this.Frame.Navigate(typeof(Demos.Segmentation_demo), null);
            }
            else if (item.Name == "Face Recognition Demo Azure IoT Showcase")
            {
                this.Frame.Navigate(typeof(Demos.Face_Recognition_Demo_Azure_Iot_Page), null);
            }
        }
    }
    public class Demo_and_Sample_List
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string details { get; set; }
        public string url { get; set; }
        public string preview_Path { get; set; }
        public string[] not_support_version { get; set; }

        public static implicit operator Demo_and_Sample_List((string Name, string Path, string details, string url, string preview_Path, string[] not_support_version) info)
        {
            return new Demo_and_Sample_List { Name = info.Name, Path = info.Path, details = info.details, url = info.url, preview_Path = info.preview_Path , not_support_version = info.not_support_version };
        }
    }
}
