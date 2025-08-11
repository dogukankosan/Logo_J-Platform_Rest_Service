using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal static class TextLog
    {
        internal static async Task LogToSQLiteAsync(string userName, string details)
        {
            try
            {
                string query = @"INSERT INTO ErrorLogs (UserName, Details, Date_) 
                                 VALUES (@userName, @details, @date)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@userName", userName },
                    { "@details", details },
                    { "@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                };
                var result = await SQLiteCrud.InsertUpdateDeleteAsync(query, parameters);
                if (!result.Success)
                    await LogToFileFallbackAsync($"SQLite log hatası: {result.ErrorMessage} | Orijinal mesaj: {details}");
            }
            catch (Exception ex)
            {
                await LogToFileFallbackAsync($"LogToSQLiteAsync exception: {ex.Message} | Orijinal mesaj: {details}");
            }
        }
        private static async Task LogToFileFallbackAsync(string message)
        {
            try
            {
                string logFilePath = Path.Combine(Application.StartupPath, "Logs", "UILog.txt");
                string logDir = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}";
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                    await sw.WriteAsync(logEntry);
            }
            catch
            {

            }
        }
    }
}