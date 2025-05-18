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

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public Task LogAsync(string message)
        {
            string formattedMessage = $"[File Log] {DateTime.Now}: {message}{Environment.NewLine}";
            return File.AppendAllTextAsync(_filePath, formattedMessage);
        }
    }
}
