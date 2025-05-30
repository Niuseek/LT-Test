using LT_Test.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT_Test.Message
{
    public class LogEntry
    {
        public string Message { get; }
        public LogLevel Level { get; }
        public DateTime Timestamp { get; }

        public LogEntry(string _message, LogLevel level)
        {
            Message = _message;
            Level = level;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[{Level}] {Timestamp:O} - {Message}";
        }
    }

}
