// ConsoleConnector.cpp : 此檔案包含 'main' 函式。程式會於該處開始執行及結束執行。
//

#include <iostream>
#include <string>
#include <fstream>
#include <streambuf>

int main(int argc, char* argv[])
{
    std::string AppID = "";
    std::cout << "Reading Parameter File >>>>\n";

    std::ifstream Param_Path_f("Parameter_to_connector_path.inf");
    if (Param_Path_f)
    {
        std::string str((std::istreambuf_iterator<char>(Param_Path_f)),
        std::istreambuf_iterator<char>());
        AppID = str;
        std::cout << "[ DEBUG ] App ID is " << AppID << "\n";

    }
    else
    {
        std::cout << "[ DEBUG ] File Not Exist!\n";
        std::ofstream Param_Path_f_w("Parameter_to_connector_path.inf");
        /*
        if (!Param_Path_f_w)
        {
            std::cout << "[ DEBUG ] Cannot Write File!\n";
            system("PAUSE");
            return EXIT_FAILURE;
        }*/
        std::cout << "[ INFO ] Some information is needed, Please input the AppID, followed by the OpenVINO Windows Demo app!\n";
        std::cin >> AppID;
        Param_Path_f_w << AppID;
        Param_Path_f_w.close();
        std::ifstream Param_Path_f("Parameter_to_connector_path.inf");
        if (Param_Path_f)
        {
            std::string str((std::istreambuf_iterator<char>(Param_Path_f)),
            std::istreambuf_iterator<char>());
            AppID = str;
            std::cout << "[ DEBUG ] App ID is " << AppID << "\n";
        }
        else
        {
            std::cout << "[ DEBUG ] Write Failed! Non such file!\n";
            system("PAUSE");
            return EXIT_FAILURE;
        }
    }
    std::string str((std::istreambuf_iterator<char>(Param_Path_f)),
        std::istreambuf_iterator<char>());


    std::string arguments[10];
    for (int i = 0; i < argc ; i++)
    {
        std::cout << "[ DEBUG ] argv[" << i << "]=" << argv[i] << "\n";
        arguments[i] = argv[i];
    }
    std::string excute_str = "\"C:\\Program Files (x86)\\IntelSWTools\\openvino\\bin\\setupvars.bat\" & %USERPROFILE%\\Documents\\Intel\\OpenVINO\\inference_engine_samples_build\\intel64\\Release\\classification_sample_async.exe -h";
    system(excute_str.c_str());
    //system("\"C:\\Program Files (x86)\\IntelSWTools\\openvino_2020.1.033\\deployment_tools\\demo\\demo_squeezenet_download_convert_run.bat\"");
    //system("%USERPROFILE%\\Documents\\Intel\\OpenVINO\\inference_engine_samples_build\\intel64\\Release\\classification_sample_async.exe -h");
    system("PAUSE");
    
}

// 執行程式: Ctrl + F5 或 [偵錯] > [啟動但不偵錯] 功能表
// 偵錯程式: F5 或 [偵錯] > [啟動偵錯] 功能表

// 開始使用的提示: 
//   1. 使用 [方案總管] 視窗，新增/管理檔案
//   2. 使用 [Team Explorer] 視窗，連線到原始檔控制
//   3. 使用 [輸出] 視窗，參閱組建輸出與其他訊息
//   4. 使用 [錯誤清單] 視窗，檢視錯誤
//   5. 前往 [專案] > [新增項目]，建立新的程式碼檔案，或是前往 [專案] > [新增現有項目]，將現有程式碼檔案新增至專案
//   6. 之後要再次開啟此專案時，請前往 [檔案] > [開啟] > [專案]，然後選取 .sln 檔案
