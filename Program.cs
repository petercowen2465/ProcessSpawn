using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;


namespace ProcessSpawn
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            AsyncDecrypt();
            SyncDecrypt();
            SyncEncrypt();
            */
            SyncDecryptToDB();

        }

        static string gpgName = @"gpg";
        static string encryptArgs = @"-v --batch --yes --recipient testkey@gmail.com --armor --output C:\code\gpg\encrypted\test-armor.gpg --encrypt";
        static string decryptArgs = @"-v --batch --yes --passphrase testKey27072020 --pinentry-mode loopback --decrypt C:\code\gpg\test-armor.gpg";
        static string fileName = @"C:\code\gpg\test-armor.gpg";
        private static void AsyncDecrypt()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = gpgName;  //@"cmd.exe";
            startInfo.Arguments = decryptArgs; // @"/C type C:\Temp\test.txt";

            process.StartInfo = startInfo;
           

            process.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);
            process.Start();
            StreamReader stdError = process.StandardError;
            process.BeginOutputReadLine();

            process.WaitForExit();
            Console.WriteLine("Standard error ={0}", stdError.ReadToEnd());
            Console.WriteLine("AsyncDecryt Done ({0})", process.ExitCode);
        }

        private static void SyncDecrypt()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = gpgName;  //@"cmd.exe";
            startInfo.Arguments = decryptArgs; // @"/C type C:\Temp\test.txt";

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
            Console.WriteLine("SyncDecrypt Done ({0})", process.ExitCode);
        }

        private static void SyncDecryptToDB()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=Test;Trusted_Connection=True;"; // ConfigurationManager.AppSettings["ConnectionString"];
            string destinationTable = @"Test"; // ConfigurationManager.AppSettings["DestinationTable"];
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);
            bulkCopy.DestinationTableName = destinationTable;
            bulkCopy.BatchSize = 10000;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = gpgName;  //@"cmd.exe";
            startInfo.Arguments = decryptArgs; // @"/C type C:\Temp\test.txt";

            process.StartInfo = startInfo;

            process.Start();

            StreamReader reader = process.StandardOutput;
            string output;
            StreamReader stdError = process.StandardError;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("FileName", typeof(string));
            dataTable.Columns.Add("RunGUID", typeof(string));
            dataTable.Columns.Add("Data", typeof(string));

            string runGUID = Guid.NewGuid().ToString();
            int i = 0;
            while (reader.Peek() >= 0)
            {
                var row = dataTable.NewRow();
                row["FileName"] = fileName;
                row["RunGUID"] = runGUID;
                row["Data"] = reader.ReadLine();
                dataTable.Rows.Add(row);
                i++;
                if (i > 100000)
                {
                    // write to db
                    bulkCopy.WriteToServer(dataTable);
                    // clear memory used
                    dataTable.Clear();
                    i = 0;
                }
            }
            bulkCopy.WriteToServer(dataTable);


            Console.WriteLine("Standard error ={0}", stdError.ReadToEnd());
            process.WaitForExit();
            Console.WriteLine("SyncDecrypt Done ({0})", process.ExitCode);
        }
        private static void SyncEncrypt()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = gpgName;
            startInfo.Arguments = encryptArgs;
            process.StartInfo = startInfo;

            process.Start();

            StreamWriter writer = process.StandardInput;
            string line;
            StreamReader stdError = process.StandardError;

            using (StreamReader sr = new StreamReader(@"C:\code\gpg\testFileSmall.txt"))
            {
                while((line = sr.ReadLine()) != null) {
                    writer.WriteLine(line);
                }

            }
            writer.Flush();
            writer.Close();

            Console.WriteLine("Standard error ={0}", stdError.ReadToEnd());
            process.WaitForExit();
            Console.WriteLine("SyncEncrypt Done ({0})", process.ExitCode);
            
        }
    }
}
