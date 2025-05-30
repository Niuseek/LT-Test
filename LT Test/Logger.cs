using LT_Test.Enums;
using LT_Test.Interfaces;
using LT_Test.Message;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Threading.Channels;

namespace LT_Test
{
    public class Logger
    {
        private readonly ConcurrentDictionary<ILogger, byte> loggers = new();
        private readonly Channel<LogEntry> logChannel;
        private readonly CancellationTokenSource cts = new();
        private readonly Task dispatcherTask;
        private readonly ConcurrentQueue<Task> runningLogTasks = new();

        public Logger(int capacity = 500)
        {
            logChannel = Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

            dispatcherTask = Task.Run(ProcessQueueAsync);
        }

        public Task LogAsync(string message, LogLevel level = LogLevel.Info)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Task.CompletedTask;
            }

            return logChannel.Writer.WriteAsync(new LogEntry(message, level)).AsTask();
        }

        private async Task ProcessQueueAsync()
        {
            await foreach (var entry in logChannel.Reader.ReadAllAsync(cts.Token))
            {
                foreach (var logger in loggers.Keys)
                {
                    var task = ProcessMessageAsync(logger, entry);
                    runningLogTasks.Enqueue(task);

                    _ = task.ContinueWith(t =>
                    {
                        TrimCompletedTasks();
                    });
                }
            }
        }

        private async Task ProcessMessageAsync(ILogger logger, LogEntry message)
        {
            try
            {
                await logger.LogAsync(message.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProcessMessageAsync] Failed due to exception: {ex.Message}");
            }
        }

        private void TrimCompletedTasks()
        {
            while (runningLogTasks.TryPeek(out var task) && task.IsCompleted)
            {
                runningLogTasks.TryDequeue(out _);
            }
        }

        public void RegisterLogger(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            loggers.TryAdd(logger, 0);
        }

        public void UnregisterLogger(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            loggers.TryRemove(logger, out _);
        }

        public async Task FlushAsync()
        {
            while (!logChannel.Reader.Completion.IsCompleted || logChannel.Reader.Count > 0)
            {
                if (logChannel.Reader.Count == 0)
                {
                    break;
                }

                await Task.Delay(50);
            }

            while (runningLogTasks.TryPeek(out var task))
            {
                await Task.WhenAll(runningLogTasks.Where(t => !t.IsCompleted).ToArray());
            }
        }

        public async ValueTask DisposeAsync()
        {
            logChannel.Writer.Complete();
            cts.Cancel();

            try
            {
                await dispatcherTask;
            }
            catch
            {
                // swallow exceptions on shutdown
            }

            cts.Dispose();

            foreach (var logger in loggers.Keys)
            {
                if (logger is IAsyncDisposable asyncLogger)
                {
                    await asyncLogger.DisposeAsync();
                }
                else if (logger is IDisposable syncLogger)
                {
                    syncLogger.Dispose();
                }
            }
        }
    }
}
