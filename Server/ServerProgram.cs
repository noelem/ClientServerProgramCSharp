using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                    }
                };
                Console.WriteLine($"Message: {messageFromJson}");
                Request message = JsonConvert.DeserializeObject<Request>(messageFromJson, settings);

                Console.WriteLine(message.Method);
                Console.WriteLine(message.Path);
                Console.WriteLine(message.Date);
                Console.WriteLine(message.Body);
                string returns = JsonConvert.SerializeObject(RequestHandler(message));
                Console.WriteLine(returns);
                client.Write(returns);
            }

        }
        static Response RequestHandler(Request r)
        {
            Response response = new();
            response.Status += "4";
            if (r.Method == null) { response.Status += " missing method"; }
            if (r.Date == null) { response.Status += " missing date"; }
            if (r.Path == null) { response.Status += " missing path"; }
            switch (r.Method)
            {
                case "update":
                    ValidateUpdate(r);
                    break;

                case "create":
                    ValidateUpdate(r);
                    break;                
                
                case "delete":
                    ValidateUpdate(r);
                    break;     
                    
                case "read":
                    ValidateUpdate(r);
                    break;
            }
               
            
            return response;
        }
        static Response CreateMethodHandler(Request r)
        {

            return new Response();
        }

        static bool ValidateUpdate(Request r)
        {
            return false;
        }        
        static bool ValidateCreate(Request r)
        {
            return false;
        }        
        static bool ValidateDelete(Request r)
        {
            return false;
        }        
        static bool ValidateRead(Request r)
        {
            return false;
        }






    }    
}
