using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT_Test.Interfaces
{
    public interface ILogger
    {
        Task LogAsync(string message);
    }
}
