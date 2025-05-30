using LT_Test.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LT_Test.Loggers
{
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string filePath;
        private readonly ILogger fallbackLogger;
        private readonly SemaphoreSlim fileLock = new(1, 1);
        private bool disposed;

        public FileLogger(string path, ILogger? _fallbackLogger = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(path));
            }

            filePath = Path.GetFullPath(path);

            fallbackLogger = _fallbackLogger ?? new ConsoleLogger();

            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public async Task LogAsync(string message)
        {
            if (disposed || string.IsNullOrWhiteSpace(message)) return;

            var formattedMessage = $"[File Log] {message}{Environment.NewLine}";

            await fileLock.WaitAsync();

            try
            {
                await File.AppendAllTextAsync(filePath, formattedMessage);
            }
            catch (Exception ex)
            {
                await fallbackLogger.LogAsync($"[File Log] Failed to write to file: {ex.Message}");
            }
            finally
            {
                fileLock.Release();
            }
        }

        public void Dispose()
        {
            disposed = true;

            fileLock.Dispose();

            if (fallbackLogger is IDisposable d)
            {
                d.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
