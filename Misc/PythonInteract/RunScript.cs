﻿using System;
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
        public static string RunScript(string scriptPath, string[] args, string pythonPath = @"C:\Program Files\Anaconda3\python.exe")
        {
            string output = string.Empty;
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
                output = outputStreamReader.ReadLine();

                pythonProcess.WaitForExit();
                pythonProcess.Close();
            }
            catch (Exception e)
            {
                output = e.Message;
            }
            return output;
        }
    }
}