global using Console = FriedFolder.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedFolder
{
    public static class Console
    {
        static Console() 
        {
            Initialize();
        }
        private static readonly string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FriedFolder");
        private static string DataPath => Path.Combine(MainPath, "debug");
        private static string DataPathFile => Path.Combine(DataPath, "webBrowserFiles.txt");
        private static void Initialize() 
        {
            if (!Directory.Exists(MainPath))
                Directory.CreateDirectory(MainPath);

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            File.WriteAllText(DataPathFile, string.Empty);
        }
        public static void WriteLine(string obj) 
        {
            File.AppendAllText(DataPathFile, obj + "\n");
        }
        public static void Write(string obj)
        {
            File.AppendAllText(DataPathFile, obj);
        }
        public static string Execute(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
    }
}
