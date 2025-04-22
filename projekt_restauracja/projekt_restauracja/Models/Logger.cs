using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using projekt_restauracja.Services;
namespace projekt_restauracja.Models
{

    public static class Logger
    {

        private static readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;

        private static readonly string _logFilePath = Path.Combine(projectDirectory, "Data", "log.txt");
        

        public static void Init()
        {
            LogManager.OnLog += SaveLogToFile;
        }

        private static void SaveLogToFile(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText(_logFilePath, $"[{timestamp}] {message}{Environment.NewLine}");
        }

        public static void DisplayLogs()
        {
            if (File.Exists(_logFilePath))
            {
                string logs = File.ReadAllText(_logFilePath);
                Console.WriteLine("📄 Log entries:\n");
                Console.WriteLine(logs);
            }
            else
            {
                Console.WriteLine("❌ Log file not found.");
            }

            Console.WriteLine("\n[Press any key to return to menu...]");
            Console.ReadKey();
        }

    }
}
