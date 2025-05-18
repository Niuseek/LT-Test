using LT_Test.Interfaces;

namespace LT_Test
{
    public class Logger
    {
        private readonly List<ILogger> loggers = new();

        public void RegisterLogger(ILogger logger)
        {
            if (!loggers.Contains(logger))
            {
                loggers.Add(logger);
            }
        }

        public void UnregisterLogger(ILogger logger)
        {
            loggers.Remove(logger);
        }

        public async Task LogAsync(string message)
        {
            var tasks = loggers.Select(logger => logger.LogAsync(message));

            await Task.WhenAll(tasks);
        }
    }
}
