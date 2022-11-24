using DynamicData.Cache.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MedEye.Tracker
{
    static public class Tracker
    {
        static private Process tracker;
        static public void StartTracking()
        {


            string startupPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()))));

            // 1) Create Process Info
            var psi = new ProcessStartInfo();
            
            psi.FileName = startupPath + @"\Python\Python310\python.exe";

            // 2) Provide script and arguments
            var script = startupPath + @"\MedEye\GazeTracking\example.py";


            psi.Arguments = $"\"{script}\" \"\" \"\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            // 4) Execute process and get output
            tracker = Process.Start(psi);
        }

        static public string GetResult()
        {
            tracker.StandardInput.WriteLine("x");
            string result = tracker.StandardOutput.ReadToEnd();

            return result;
        }
    }
}
