using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_restauracja.Services
{
    public static class LogManager
    {
        public delegate void LogEventHandler(string message);
        public static event LogEventHandler OnLog;

        public static void Log(string message)
        {
            OnLog?.Invoke(message);
        }
    }

}
