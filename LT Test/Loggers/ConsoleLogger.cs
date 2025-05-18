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
        public Task LogAsync(string message)
        {
            return Task.Run(() => Console.WriteLine($"{DateTime.Now}: {message}"));
        }
    }
}
