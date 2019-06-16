using System;
using System.Diagnostics;
using System.Threading;

namespace Yolo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RunMultipleYolo();
        }
        private static void RunMultipleYolo()
        {
            const string mainPath = @"C:\Users\Z6PLT\Desktop\Moje\ML\PythonProjects\darknet\build\darknet\x64\";
            const string program = "darknet_no_gpu.exe";
            const string data = mainPath + @"data\obj.data";
            const string backup = mainPath + @"backup\yolov3-tiny-obj_last.weights";
            const string cfg = mainPath + @"yolov3-tiny-obj.cfg";


            var programCounter = 0;
            const string processParameters = "detector train " + data + " " + cfg + " " + backup + " -dont_show";
            while (true)
            {
                var proc = new Process { StartInfo = { FileName = mainPath + program, Arguments = processParameters } };

                proc.Start();
                programCounter++;
                Thread.Sleep(5000);
                while (Process.GetProcessesByName("darknet_no_gpu").Length % 2 == 1 || Process.GetProcessesByName("darknet_no_gpu").Length > 100)
                {
                    Thread.Sleep(5555);
                    Console.WriteLine("Program number: " + programCounter + "\n" + Process.GetProcessesByName("darknet_no_gpu").Length);
                }
            }
        }
    }
}
