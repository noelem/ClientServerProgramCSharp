using System;
using System.Net.Sockets;
using System.Text;

namespace Utillities
{
    public static class TcpClientExtensions
    {
        public static void Send(this TcpClient client, string message)
        {
            client.GetStream().Write(Encoding.UTF8.GetBytes(message));
        }

        public static string Receive(this TcpClient client)
        {
            var buffer = new byte[1024];
            var rdCnt =client.GetStream().Read(buffer);
            return Encoding.UTF8.GetString(buffer, 0, rdCnt);
        }
    }
}
