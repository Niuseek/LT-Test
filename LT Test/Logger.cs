using LT_Test.Interfaces;

namespace LT_Test
{
    public class Logger
    {
        private readonly List<ILogger> _loggers = new();

        public void RegisterLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        public void UnregisterLogger(ILogger logger)
        {
            _loggers.Remove(logger);
        }

        public async Task LogAsync(string message)
        {
            var tasks = _loggers.Select(logger => logger.LogAsync(message));

            await Task.WhenAll(tasks);
        }
    }
}
