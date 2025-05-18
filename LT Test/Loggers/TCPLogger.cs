using LT_Test.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LT_Test.Loggers
{
    public class TCPLogger : ILogger
    {
        private readonly IPEndPoint ipEndPoint;
        private readonly ConsoleLogger _fallbackLogger = new();

        public TCPLogger(string ip, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public TCPLogger(IPAddress ip, int port)
        {
            ipEndPoint = new IPEndPoint(ip, port);
        }

        public TCPLogger(IPEndPoint _ipEndPoint)
        {
            ipEndPoint = _ipEndPoint;
        }

        public async Task LogAsync(string message)
        {
            try
            {
                using TcpClient client = new();

                await client.ConnectAsync(ipEndPoint);

                await using NetworkStream stream = client.GetStream();

                string formattedMessage = $"[TCP Log] {DateTime.Now}: {message}{Environment.NewLine}";

                byte[] data = Encoding.UTF8.GetBytes(formattedMessage);

                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                await _fallbackLogger.LogAsync($"[TCP Log] Error: Unable to connect to {ipEndPoint.Address}:{ipEndPoint.Port}. Exception: {ex.Message}");
            }
        }
    }
}
