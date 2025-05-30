using LT_Test.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT_Test.Loggers
{
    public class ConsoleLogger : ILogger
    {
        private static readonly object ConsoleLock = new object();

        public Task LogAsync(string message)
        {
            lock (ConsoleLock)
            {
                Console.WriteLine($"[Console Log] {message}");
            }

            return Task.CompletedTask;
        }
    }
}
