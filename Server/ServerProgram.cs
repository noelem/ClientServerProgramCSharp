using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            IPAddress ipAddress = Dns.Resolve("localhost").AddressList[0];
            var server = new TcpListener(ipAddress, 5000);
            server.Start();
            Console.WriteLine("Server started");
            var client = server.AcceptTcpClient();
            Console.WriteLine("Client accepted");

        }
    }
}
