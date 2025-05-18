using LT_Test.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT_Test.Loggers
{
    public class FileLogger : ILogger
    {
        private string _filePath;

        private readonly ConsoleLogger _fallbackLogger = new();

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public async Task LogAsync(string message)
        {
            string formattedMessage = $"[File Log] {DateTime.Now}: {message}{Environment.NewLine}";

            try
            {
                await File.AppendAllTextAsync(_filePath, formattedMessage);
            }
            catch (Exception ex)
            {
                await _fallbackLogger.LogAsync($"[File Log] Failed to write to file: {_filePath}. Exception: {ex.Message}");
            }
        }
    }
}
