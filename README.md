# LT-Test
## Manual

**How to build**

Current library implementation doesn’t use any external libraries apart from xUnit (for unit testing), so the only thing that is needed is a fresh C# Net 8.0 project.

**Integration**

Just add the library to a console project and you can use following to test the code to test it. File logger will create a file in the binary location, unless path is specified.

```
using LT_Test;
using LT_Test.Loggers;

var consoleLogger = new ConsoleLogger();
var fileLogger = new FileLogger("log.txt");
var TCPLogger = new TCPLogger("127.0.0.1", 4222);

Logger logger = new Logger();

logger.RegisterLogger(consoleLogger);
logger.RegisterLogger(fileLogger);
logger.RegisterLogger(TCPLogger);

for (int i = 1; i <= 200; i++)
{
    await logger.LogAsync($"This is a test message to all registered loggers {i}");
}

await logger.FlushAsync();
```
