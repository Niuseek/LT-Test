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
            return Task.Run(async () =>
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    await writer.WriteLineAsync($"[File Log] {DateTime.Now}: {message}");
                }
            });
        }
    }
}
