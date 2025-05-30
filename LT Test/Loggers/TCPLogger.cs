using LT_Test.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LT_Test.Loggers
{
    public class TCPLogger : ILogger, IDisposable
    {
        private readonly IPEndPoint ipEndPoint;
        private readonly ILogger fallbackLogger;
        private TcpClient? client;
        private NetworkStream? stream;
        private readonly SemaphoreSlim writeLock = new(1, 1);
        private bool disposed = false;

        public TCPLogger(string ip, int port, ILogger? _fallbackLogger = null)
        : this(new IPEndPoint(IPAddress.Parse(ip), port), _fallbackLogger) { }

        public TCPLogger(IPAddress ip, int port, ILogger? _fallbackLogger = null)
            : this(new IPEndPoint(ip, port), _fallbackLogger) { }

        public TCPLogger(IPEndPoint _ipEndPoint, ILogger? _fallbackLogger = null)
        {
            ipEndPoint = _ipEndPoint ?? throw new ArgumentNullException(nameof(ipEndPoint));
            fallbackLogger = _fallbackLogger ?? new ConsoleLogger();
        }

        public async Task LogAsync(string message)
        {
            if (disposed || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            string formattedMessage = $"[TCP Log] {message}{Environment.NewLine}";
            byte[] data = Encoding.UTF8.GetBytes(formattedMessage);

            await writeLock.WaitAsync();

            try
            {
                await EnsureConnectedAsync();

                if (stream != null && stream.CanWrite)
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                await fallbackLogger.LogAsync($"[TCP Log] Error: {ex.Message}");
                CleanupConnection();
            }
            finally
            {
                writeLock.Release();
            }
        }

        private async Task EnsureConnectedAsync()
        {
            if (client?.Connected == true)
            {
                return;
            }

            CleanupConnection();

            client = new TcpClient();
            await client.ConnectAsync(ipEndPoint);
            stream = client.GetStream();
        }

        private void CleanupConnection()
        {
            try
            {
                stream?.Dispose();
                client?.Close();
            }
            catch
            {
                // intentionally swallow exception as we're cleaning up connection
            }

            stream = null;
            client = null;
        }

        public void Dispose()
        {
            disposed = true;
            writeLock.Wait();

            CleanupConnection();
            writeLock.Dispose();

            if (fallbackLogger is IDisposable d)
            {
                d.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
