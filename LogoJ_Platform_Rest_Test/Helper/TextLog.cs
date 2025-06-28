using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal static class TextLog
    {
        private static readonly string logFilePath = Path.Combine(Application.StartupPath, "Logs", "UILog.txt");
        internal static async Task TextLoggingAsync(string message)
        {
            try
            {
                string logDir = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}";
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                    await sw.WriteAsync(logEntry);
            }
            catch (Exception)
            {
              
            }
        }
    }
}