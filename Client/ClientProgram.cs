using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Utillities; 

namespace Client
{
    
    class ClientProgram
    {
        static void Main(string[] args)
        {

            var client = new NetworkClient();
            client.Connect("localhost", 5000);

            var message = new Request("thisMethod", "thisPath", "thisBody");

            var messageAsJson = JsonSerializer.Serialize<Request>(message);
            client.Write(messageAsJson);

            var response = client.Read();
            Console.WriteLine($"Server response '{response}'");

        }
    }
}
