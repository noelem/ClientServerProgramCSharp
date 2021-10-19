﻿using System;
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
            //IPAddress ipAddress = Dns.Resolve("localhost").AddressList[0];
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server started");

            while (true)
            {
                var client = server.AcceptTcpClient();
                Console.WriteLine("Client accepted");

                //var stream = client.GetStream();

                //var buffer = new byte[1024];

                //var rdCnt = stream.Read(buffer);

                //var message = Encoding.UTF8.GetString(buffer, 0, rdCnt);

                var message = client.Receive();

                Console.WriteLine($"Client message '{message}' and the read count was {rdCnt}");

                //var response = Encoding.UTF8.GetBytes(message.ToUpper());
                //stream.Write(response);

                client.Send(message.ToUpper());
            }

        }
    }
}
