#define DEBUG
#define TERMINAL_SHOW
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        static string App_path = "";

        static string openvino_install_dir = @"C:\Program Files (x86)\IntelSWTools\openvino\";
        static string setupvars_path = openvino_install_dir + @"bin\setupvars.bat";
        static string demo_Path = @"%USERPROFILE%\Documents\Intel\OpenVINO\inference_engine_demos_build\intel64\Release\";
        static string python_demo_path = openvino_install_dir + @"deployment_tools\open_model_zoo\demos\python_demos\";
        static string synnex_demo = @"%USERPROFILE%\Documents\Intel\OpenVINO\synnex_demos\";

        static string model_path = @"%USERPROFILE%\Documents\Intel\openvino\openvino_models\models";
        static string model_cache = @"%USERPROFILE%\Documents\Intel\openvino\openvino_models\cache";
        static string irs_path = @"%USERPROFILE%\Documents\Intel\openvino\openvino_models\ir";


        static void Main(string[] args)
        {
            new Thread(ThreadProc).Start();
            Console.Title = "OpenVINO Windows Demo Tool - Console Connector";
            Console.WriteLine("This process runs between OpenVINO Windows Demo Tool and system");
            
            string input = "";
            while (!input.Equals("Exit"))
            {
                Console.WriteLine("\r\nPress E to Force Exit this console ...");
                input = Console.ReadLine().ToString();
            }
            
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
                    send_message(args, "command", "ECHO");
                    break;
                case "Home_Page":
                    Home_Page(args, value);
                    break;
                case "Interactive_face_detection_demo":
                    Interactive_face_detection_demo(value);
                    send_message(args, "command", "ECHO");
                    break;
                case "Face_Recognition_Demo_Page":
                    Face_Recognition_Demo_Page(value);
                    send_message(args, "command", "ECHO");
                    break;
                case "Crossroad_Camera_Demo_Page":
                    Crossroad_Camera_Demo_Page(value);
                    send_message(args, "command", "ECHO");
                    break;
                case "Human_Pose_Estimation_Demo_Page":
                    Human_Pose_Estimation_Demo_Page(value);
                    send_message(args, "command", "ECHO");
                    break;
                case "Gaze_Estimation_Demo_Page":
                    Gaze_Estimation_Demo_Page(value);
                    send_message(args, "command", "ECHO");
                    break;
                case "Face_Recognition_Demo_Azure_Iot_Page":
                    Face_Recognition_Demo_Azure_Iot_Page(value);
                    send_message(args, "command", "ECHO");
                    break;
                case "App_Path":
                    App_path = value;
                    send_message(args, "command", "ECHO");
                    break;
                case "Downloader":
                    Downloader_Page(args, value);
                    break;
                case "print_model_info":
                    Print_Model_info(args, value);
                    break;
                default:
                    send_message(args, "command", "ECHO");
                    break;
            }
        }
        private static void send_message(AppServiceRequestReceivedEventArgs args, string key_str,string value_str)
        {
            ValueSet valueSet = new ValueSet();
            valueSet.Add(key_str, value_str);
            //Send back message to UWP
            args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "\tMessage to UWP has been sent (" + key_str + " , " + value_str + ")");
        }
        private static void command(string value_str)
        {
            Console.WriteLine("[INFO] Command: " + value_str);
            switch (value_str)
            {
                case "Close":
                    Environment.Exit(0);
                    break;
                case "Sample_Build":
                    string path = Directory.GetCurrentDirectory();
                    ProcessStartInfo processStartInfo = new ProcessStartInfo()
                    {
                        FileName = "cmd.exe",
                        //Arguments = "/C \"\"" + App_path + "\\Demos\\autobuildAgent.bat\" & PAUSE\"",
                        Arguments = "/C \"\"" + App_path + "\\Demos\\autobuildAgent.bat\" ",
                        //UseShellExecute = true,
                        //RedirectStandardOutput = true,
                        //CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Normal
            };
                    var process = new Process()
                    {
                        StartInfo = processStartInfo
                    };
#if (DEBUG)
                    Console.WriteLine("[DEBUG] RUNNING:" + value_str);
#endif
                    process.Start();
                    break;
                default:
                    break;
            }
        }

        private static void Print_Model_info(AppServiceRequestReceivedEventArgs args, string value_str)
        {
#if (DEBUG)
            Console.WriteLine("[DEBUG] Print_Model_info:" + value_str);
#endif

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/C \"C:\\Program Files (x86)\\IntelSWTools\\openvino\\deployment_tools\\tools\\model_downloader\\info_dumper.py\" --name " + value_str,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                var process = new Process()
                {
                    StartInfo = processStartInfo
                };
                process.Start();
                string message = "";
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    Console.WriteLine("[INFO] Get " + value_str + " info:\n" + line);
                    message += line + "\n";
                }
                send_message(args, "Model_Info", message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
                    
        }
        private static int Get_All_Model_info_json()
        {
            const int SUCCESS = 0;
            const int FAILURE = 1;
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/C \"C:\\Program Files (x86)\\IntelSWTools\\openvino\\deployment_tools\\tools\\model_downloader\\info_dumper.py\" --all",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                var process = new Process()
                {
                    StartInfo = processStartInfo
                };
                process.Start();
                string message = "";
                Console.WriteLine("[INFO] Get All JSON model info :\n");
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line + "\n");
                    message += line + "\n";
                }
                File.WriteAllText(App_path + "\\models_info.json", message);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return FAILURE;
            }
            return SUCCESS;
        }

        private static void Downloader_Page(AppServiceRequestReceivedEventArgs args, string value_str)
        {
#if (DEBUG)
            Console.WriteLine("[DEBUG] DOWNLOADER_PAGE:" + value_str);
#endif
            switch (value_str)
            {
                case "print_all_model_name":
                    try
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C \"C:\\Program Files (x86)\\IntelSWTools\\openvino\\deployment_tools\\tools\\model_downloader\\info_dumper.py\" --print_all",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };
                        var process = new Process()
                        {
                            StartInfo = processStartInfo
                        };
                        process.Start();
                        string message = "";
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            line = line.Substring(line.LastIndexOf("\\") + 1);
                            Console.WriteLine("[INFO] Get All_model name:\n" + line);
                            message += line + "\n";


                        }
                        send_message(args, "All_Model_Name", message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "Get_All_Model_info_json":
                    if (Get_All_Model_info_json().Equals(1))
                    {
                        send_message(args, "Get_All_Model_info_json", "FAILED");
                    }
                    else
                    {
                        send_message(args, "Get_All_Model_info_json", "SUCCESS");
                    }
                    break;
                case "Downloaded_Model_Name":
                    try
                    {
                        Home_Page(args, "OMZ_Model_check");
                        /*ProcessStartInfo processStartInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C \"\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };
                        var process = new Process()
                        {
                            StartInfo = processStartInfo
                        };
                        process.Start();
                        string message = "";
                        Console.WriteLine("[INFO] Get Downloaded_model name:\n");
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            line = line.Substring(line.LastIndexOf("\\") + 1);
                            Console.WriteLine(line);
                            message += line + "\n";
                        }
                        send_message(args, "Downloaded_Model_Name", message);*/
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                default:
                    if (value_str.Contains("DOWNLOAD "))
                    {
                        string model = value_str.Substring(value_str.IndexOf(" ")+1);
                        Console.WriteLine("[INFO] Downloading " + model + " !\n");
                        try
                        {
                            ProcessStartInfo processStartInfo = new ProcessStartInfo()
                            {
                                FileName = "cmd.exe",
                                Arguments = "/C \"\"C:\\Program Files (x86)\\IntelSWTools\\openvino\\deployment_tools\\tools\\model_downloader\\downloader.py\" --name \"" + model + "\" --output_dir \"" + model_path + "\" --cache_dir \"" + model_cache + "\"\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = false,
                                WindowStyle = ProcessWindowStyle.Minimized
                            };
                        var process = new Process()
                            {
                                StartInfo = processStartInfo
                            };
#if (DEBUG)
                            Console.WriteLine("[DEBUG] DOWNLOAD:" + processStartInfo.Arguments);
#endif
                            process.Start();
                            string message = "";
                            while (!process.StandardOutput.EndOfStream)
                            {
                                var line = process.StandardOutput.ReadLine();
                                line = line.Substring(line.LastIndexOf("\\") + 1);
                                Console.WriteLine(line);
                            }
                            Console.WriteLine("[INFO] Download " + model + " Completed!");
                            send_message(args, "command", "ECHO");
                            process.Close();

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        try
                        {
                            ProcessStartInfo processStartInfo_MO = new ProcessStartInfo()
                            {
                                FileName = "cmd.exe",
                                Arguments = "/C \"\"" + setupvars_path + "\" & python \"C:\\Program Files (x86)\\IntelSWTools\\openvino\\deployment_tools\\tools\\model_downloader\\converter.py\" --mo \"" + openvino_install_dir + "deployment_tools\\model_optimizer\\mo.py\" --name \"" + model + "\" -d \"" + model_path + "\" -o \"" + irs_path + "\" --precisions \"FP32\"\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = false,
                                WindowStyle = ProcessWindowStyle.Minimized
                            };
                            var process_MO = new Process()
                            {
                                StartInfo = processStartInfo_MO
                            };
    #if (DEBUG)
                            Console.WriteLine("[DEBUG] DOWNLOAD:" + processStartInfo_MO.Arguments);
    #endif
                            process_MO.Start();
                            string message = "";
                            while (!process_MO.StandardOutput.EndOfStream)
                            {
                                var line = process_MO.StandardOutput.ReadLine();
                                line = line.Substring(line.LastIndexOf("\\") + 1);
                                Console.WriteLine(line);
                            }
                            Console.WriteLine("[INFO] MO to FP32 : " + model + " Completed!");
                            send_message(args, "command", "ECHO");
                            process_MO.Close();

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        
                    }
                    break;
            }
        }
        private static void Home_Page(AppServiceRequestReceivedEventArgs args, string value_str)
        {
#if (DEBUG)
            Console.WriteLine("[DEBUG] HOME_PAGE:" + value_str);
#endif
            switch (value_str)
            {
                case "ECHO":
                    send_message(args, "command", "ECHO");
                    break;
                case "CPU_check":
                    try
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C \"wmic cpu get Name\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };
                        var process = new Process()
                        {
                            StartInfo = processStartInfo
                        };
                        process.Start();
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            if (line.Contains("CPU"))
                            {
                                Console.WriteLine("[INFO] Get CPU info:" + line);
                                send_message(args, "CPU", line);
                            }
                            else
                            {
#if (DEBUG)
                                Console.WriteLine("[DEBUG] " + processStartInfo.Arguments + ":" + line);
#endif
                            }
                            
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "OpenVINO_check":
                    try
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C \"dir \"C:\\Program Files (x86)\\IntelSWTools\\\" |find \"openvino\"\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };
                        var process = new Process()
                        {
                            StartInfo = processStartInfo
                        };
                        process.Start();
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            if (line.Contains("["))
                            {
                                line = line.Substring(0,line.LastIndexOf("\\"));
                                line = line.Substring(line.LastIndexOf("\\")+1);
                                Console.WriteLine("[INFO] Get OpenVINO info:" + line);
                                send_message(args, "OpenVINO", line);
                            }
                            else
                            {
#if (DEBUG)
                                Console.WriteLine("[DEBUG] " + processStartInfo.Arguments + ":" + line);
#endif
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "SampleDemo_check":
                    try
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C \"dir %USERPROFILE%\\Documents\\Intel\\OpenVINO /B /S |find \".exe\" \"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };
                        var process = new Process()
                        {
                            StartInfo = processStartInfo
                        };
                        process.Start();
                        string message = "";
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            if (line.Contains(".exe"))
                            {
                                line = line.Substring(line.LastIndexOf("\\") + 1);
                                Console.WriteLine("[INFO] Get SampleDemo info:" + line);
                                message += line + "\n";
                                
                            }
                            else
                            {
#if (DEBUG)
                                Console.WriteLine("[DEBUG] " + processStartInfo.Arguments + ":" + line.Substring(line.LastIndexOf("\\") + 1) + "\n");
#endif
                            }

                        }
                        send_message(args, "SampleDemo", message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "OMZ_Model_check":
                    try
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C \"dir %USERPROFILE%\\Documents\\Intel\\OpenVINO\\openvino_models /B /S |find \".xml\" \"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };
                        var process = new Process()
                        {
                            StartInfo = processStartInfo
                        };
                        process.Start();
                        string message = "";
                        int counter = 0;
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var line = process.StandardOutput.ReadLine();
                            if (line.Contains(".xml"))
                            {
                                //string temp = line.Substring(0,line.LastIndexOf("\\"));
                                //temp = temp.Substring(temp.LastIndexOf("\\") + 1);

                                //line = line.Substring(line.LastIndexOf("\\") + 1);
                                //Console.WriteLine("[INFO] Get OMZ_model info:" + line + " [" + temp + "] ");
                                //message += temp + "\\" + line + "\n";
                                message += line + "\n";
                                counter++;
                            }
                            else
                            {
#if (DEBUG)
                                Console.WriteLine("[DEBUG] " + processStartInfo.Arguments + ":" + line.Substring(line.LastIndexOf("\\") + 1) + "\n");
#endif
                            }

                        }
                        Console.WriteLine(message);
                        File.WriteAllText(App_path + "\\downloaded_models_list.inf", message);
                        send_message(args, "OMZ_Model", counter.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                default:
                    break;
            }
        }

        private static void Interactive_face_detection_demo(string value_str)
        {
            Console.WriteLine("[INFO] Interactive_face_detection_demo " + value_str);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Arguments = "/C \"" + setupvars_path + "\" & " + demo_Path + "interactive_face_detection_demo.exe " + value_str + " ";
            //processStartInfo.Arguments = "/C \"" + setupvars_path + "\" &  %USERPROFILE%\\Documents\\Intel\\OpenVINO\\inference_engine_demos_build\\intel64\\Release\\interactive_face_detection_demo.exe" +
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
            Console.WriteLine("[DEBUG] " + processStartInfo.Arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(processStartInfo);


            //Process.Start("cmd.exe", "/C \"" + openvino_install_dir + setupvars_path + "\" && PAUSE");

        }
        private static void Face_Recognition_Demo_Azure_Iot_Page(string value_str)
        {
            Console.WriteLine("[INFO] Face_Recognition_Demo_Azure_Iot_Page " + value_str);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";
            //processStartInfo.Arguments = "/C \"\"" + setupvars_path + "\" & python \"" + python_demo_path + "face_recognition_demo\\face_recognition_demo.py\"\" " + value_str + " & PAUSE ";
            //processStartInfo.Arguments = "/C \"\"" + setupvars_path + "\" & python \"" + synnex_demo + "face_recognition_demo_Azure_IoT\\face_recognition_demo.py\"\" " + value_str + " & PAUSE ";
            processStartInfo.Arguments = "/C \"\"" + setupvars_path + "\" & python \"" + App_path + "\\Demos\\synnex_demos\\face_recognition_demo_Azure_IoT\\face_recognition_demo.py\"\" " + value_str + " & PAUSE ";
            Console.WriteLine("[DEBUG] " + processStartInfo.Arguments);
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(processStartInfo);
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
