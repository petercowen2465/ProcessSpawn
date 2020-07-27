using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ProcessSpawn
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncRead();
            SyncRead();
        }

        static string fileName = @"gpg";
        static string arguments = @"-v --batch --passphrase testKey27072020 --pinentry-mode loopback --decrypt C:\code\gpg\test-armor.gpg";
        private static void AsyncRead()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = fileName;  //@"cmd.exe";
            startInfo.Arguments = arguments; // @"/C type C:\Temp\test.txt";

            process.StartInfo = startInfo;

            process.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);
            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
            Console.WriteLine("AsyncRead Done");
        }

        private static void SyncRead()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = fileName;  //@"cmd.exe";
            startInfo.Arguments = arguments; // @"/C type C:\Temp\test.txt";

            process.StartInfo = startInfo;

            process.Start();

            StreamReader reader = process.StandardOutput;
            string output;
            StreamReader stdError = process.StandardError;

            while (reader.Peek() >= 0)
            {
                output = reader.ReadLine();
                Console.WriteLine("received output: {0}", output);
            }
            Console.WriteLine("Standard error ={0}", stdError.ReadToEnd());
            process.WaitForExit();
            Console.WriteLine("SyncRead Done");
        }
    }
}
