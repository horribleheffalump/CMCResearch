using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonInteract
{
    public static class Python
    {
        public static string RunScript(string scriptPath, string[] args, string pythonPath = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Anaconda3_64\python.exe")
        {
            StringBuilder output = new StringBuilder();
            try
            {
                ProcessStartInfo pythonProcessStartInfo = new ProcessStartInfo(pythonPath)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = scriptPath + " " + string.Join(" ", args)
                };

                Process pythonProcess = new Process
                {
                    StartInfo = pythonProcessStartInfo
                };
                pythonProcess.Start();

                StreamReader outputStreamReader = pythonProcess.StandardOutput;

                string output_line = outputStreamReader.ReadLine();
                while (!string.IsNullOrEmpty(output_line))
                {
                    output.AppendLine(output_line);
                    output_line = outputStreamReader.ReadLine();
                }

                pythonProcess.WaitForExit();
                pythonProcess.Close();
            }
            catch (Exception e)
            {
                output.AppendLine(e.Message);
            }
            return output.ToString();
        }
    }
}
