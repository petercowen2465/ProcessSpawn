using System;
using System.Diagnostics;

namespace ProcessSpawn
{
    class Program
    {
        static void Main(string[] args)
        {

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = @"cmd.exe";
            startInfo.Arguments = @"/C type C:\Temp\test.txt";

            process.StartInfo = startInfo;

            process.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);
            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
            Console.WriteLine("Done");
        }

    }
}
