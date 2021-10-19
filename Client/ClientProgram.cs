using System;
using System.Net.Sockets;
using System.Text;
using Utillities;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {

            var client = new TcpClient();

            client.Connect("localhost", 5000);

            var message = "hello";
            client.Send(message);

            var response = client.Receive();

            Console.WriteLine($"Server response '{response}'");

        }
    }
}
