#define DEBUG
#define TERMINAL_SHOW
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ConsoleConnector
{
    class Program
    {
        static AppServiceConnection connection = null;

        static string openvino_install_dir = @"C:\Program Files (x86)\IntelSWTools\openvino\";
        static string setupvars_path = openvino_install_dir + @"bin\setupvars.bat";
        static string demo_Path = @"%USERPROFILE%\Documents\Intel\OpenVINO\omz_demos_build\intel64\Release\";
        static string python_demo_path = openvino_install_dir + @"deployment_tools\open_model_zoo\demos\python_demos\";

        static void Main(string[] args)
        {
            new Thread(ThreadProc).Start();
            Console.Title = "Hello World";
            Console.WriteLine("This process runs at the full privileges of the user and has access to the entire public desktop API surface");
            Console.WriteLine("\r\nPress any key to exit ...");
            Console.ReadLine();
        }

        /// <summary>
        /// Creates the app service connection
        /// </summary>
        static async void ThreadProc()
        {
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "\t(a)ThreadProc Start!!");

            connection = new AppServiceConnection();
            connection.AppServiceName = "ConsoleConnectorCommunicationService";
            connection.PackageFamilyName = Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection established - waiting for requests");
                    Console.WriteLine();
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not installed.");
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not available.");
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                    return;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return;
            }
        }

        /// <summary>
        /// Receives message from UWP app and sends a response back
        /// </summary>
        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "\t(b) Get message from UWP!");

            string key = args.Request.Message.First().Key;
            string value = args.Request.Message.First().Value.ToString();
#if (DEBUG)
            Console.WriteLine("[DEBUG] " + key + ", " + value );
#endif
            switch(key)
            {
                case "Command":
                    command(value);
                    break;
                case "Interactive_face_detection_demo":
                    Interactive_face_detection_demo(value);
                    break;
                case "Face_Recognition_Demo_Page":
                    Face_Recognition_Demo_Page(value);
                    break;
                case "Crossroad_Camera_Demo_Page":
                    Crossroad_Camera_Demo_Page(value);
                    break;
                case "Human_Pose_Estimation_Demo_Page":
                    Human_Pose_Estimation_Demo_Page(value);
                    break;
                case "Gaze_Estimation_Demo_Page":
                    Gaze_Estimation_Demo_Page(value);
                    break;
                default:
                    break;
            }

            ValueSet valueSet = new ValueSet();


            valueSet.Add("serialNumber", "12345");

            //Send back message to UWP
            args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "\tMessage to UWP has been sent!!");
        }
        private static void command(string value_str)
        {
            Console.WriteLine("[INFO] Command: " + value_str);
        }

        private static void Interactive_face_detection_demo(string value_str)
        {
            Console.WriteLine("[INFO] Interactive_face_detection_demo " + value_str);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Arguments = "/C \"" + setupvars_path + "\" & " + demo_Path + "interactive_face_detection_demo.exe " + value_str + " ";
            //processStartInfo.Arguments = "/C \"" + setupvars_path + "\" &  %USERPROFILE%\\Documents\\Intel\\OpenVINO\\omz_demos_build\\intel64\\Release\\interactive_face_detection_demo.exe" +
            //    " -m D:\\Intel\\openvino_models\\models\\SYNNEX_demo\\intel\\face-detection-adas-binary-0001\\FP32-INT1\\face-detection-adas-binary-0001.xml -i cam & PAUSE";
            //processStartInfo.Arguments = "/C \"" + setupvars_path + "\" & PAUSE";

            Console.WriteLine("[DEBUG] " + processStartInfo.Arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(processStartInfo);


            //Process.Start("cmd.exe", "/C \"" + openvino_install_dir + setupvars_path + "\" && PAUSE");

        }
        private static void Face_Recognition_Demo_Page(string value_str)
        {
            Console.WriteLine("[INFO] Face_Recognition_Demo_Page " + value_str);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Arguments = "/C \"\"" + setupvars_path + "\" & python \"" + python_demo_path + "face_recognition_demo\\face_recognition_demo.py\"\" " + value_str + " & PAUSE ";
            //processStartInfo.Arguments = "/C \"\"" + setupvars_path + "\" & python \"D:\\Intel\\open_model_zoo\\demos\\python_demos\\face_recognition_demo_Azure_IoT\\face_recognition_demo.py\"\" " + value_str + " & PAUSE ";
            Console.WriteLine("[DEBUG] " + processStartInfo.Arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(processStartInfo);


            //Process.Start("cmd.exe", "/C \"" + openvino_install_dir + setupvars_path + "\" && PAUSE");

        }
        private static void Crossroad_Camera_Demo_Page(string value_str)
        {
            Console.WriteLine("[INFO] Crossroad_Camera_Demo_Page " + value_str);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";

            processStartInfo.Arguments = "/C \"" + setupvars_path + "\" & " + demo_Path + "crossroad_camera_demo.exe " + value_str + " & PAUSE ";
            Console.WriteLine("[DEBUG] " + processStartInfo.Arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(processStartInfo);


            //Process.Start("cmd.exe", "/C \"" + openvino_install_dir + setupvars_path + "\" && PAUSE");

        }
        private static void Human_Pose_Estimation_Demo_Page(string value_str)
        {
            Console.WriteLine("[INFO] Human_Pose_Estimation_Demo_Page " + value_str);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";

            processStartInfo.Arguments = "/C \"" + setupvars_path + "\" & " + demo_Path + "human_pose_estimation_demo.exe " + value_str + " & PAUSE ";
            Console.WriteLine("[DEBUG] " + processStartInfo.Arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(processStartInfo);


            //Process.Start("cmd.exe", "/C \"" + openvino_install_dir + setupvars_path + "\" && PAUSE");

        }
        private static void Gaze_Estimation_Demo_Page(string value_str)
        {
            Console.WriteLine("[INFO] Gaze_Estimation_Demo_Page " + value_str);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";

            processStartInfo.Arguments = "/C \"" + setupvars_path + "\" & " + demo_Path + "gaze_estimation_demo.exe " + value_str + " & PAUSE ";
            Console.WriteLine("[DEBUG] " + processStartInfo.Arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(processStartInfo);


            //Process.Start("cmd.exe", "/C \"" + openvino_install_dir + setupvars_path + "\" && PAUSE");

        }
    }
}
