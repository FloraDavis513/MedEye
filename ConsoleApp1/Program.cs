using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

string startupPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()))));

// 1) Create Process Info
var psi = new ProcessStartInfo();
psi.FileName = startupPath + @"\Python\Python310\python.exe";

// 2) Provide script and arguments
var script = startupPath + @"\GazeTracking\example.py";


psi.Arguments = $"\"{script}\" \"\" \"\"";

// 3) Process configuration
psi.UseShellExecute = false;
psi.CreateNoWindow = true;
psi.RedirectStandardOutput = true;
psi.RedirectStandardError = true;

// 4) Execute process and get output
var errors = "";
var results = "";

using (var process = Process.Start(psi))
{
    errors = process.StandardError.ReadToEnd();
    results= process.StandardOutput.ReadToEnd();
}

// 5) Display output
Console.WriteLine("ERRORS:");
Console.WriteLine(errors);
Console.WriteLine("RESULTS:");
Console.WriteLine(results);



