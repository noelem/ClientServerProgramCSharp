using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
                var client = new NetworkClient(server.AcceptTcpClient());
                Console.WriteLine("Client accepted");

                var messageFromJson = client.Read();
                var message = JsonSerializer.Deserialize<Request>(messageFromJson);
                
                System.Console.WriteLine(message);

            }

        }
    
        //requestHandler()




    }    
}
