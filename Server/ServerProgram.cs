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
                System.Console.WriteLine(messageFromJson); // works

                //var message = JsonSerializer.Deserialize<Request>(messageFromJson);
                var test = JsonSerializer.Deserialize<Request>(messageFromJson);
                
                Console.WriteLine(test);

            }
        }
    
        //requestHandler()


    }    
}
