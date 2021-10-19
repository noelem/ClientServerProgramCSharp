using System;
using System.Net.Sockets;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {

            var client = new TcpClient();

            client.Connect("localhost", 5000);


            
 
        }
    }
}
