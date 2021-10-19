using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utillities;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server started");

            while (true)
            {
                var client = server.AcceptTcpClient();
                Console.WriteLine("Client accepted");

                var message = client.Receive();

                Console.WriteLine($"Client message '{message}'");

                client.Send(message.ToUpper());
            }

        }
    }
}
