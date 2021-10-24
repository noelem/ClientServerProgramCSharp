using Newtonsoft.Json;
using System;
using System.Linq;
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
                client.Write(returns.ToLower());
                //client.Write("{\"status\":\"4 bad request\", \"body\":null}");
            }

        }
        static Response RequestHandler(Request r)
        {
            Response response = new();
            response.Status += "4";
            if (r.Method == null) { response.Status += " missing method"; }
            if (r.Date == null) { response.Status += " missing date"; }
            if (r.Path == null) { response.Status += " missing resource"; }
            switch (r.Method)
            {
                case "update":
                    response.Status += !ValidateUpdate(r) ? " bad request" : " ok";
                    break;

                case "create":
                    response.Status += !ValidateCreate(r) ? " bad request" : " ok";
                    break;                
                
                case "delete":
                    response.Status += !ValidateDelete(r) ? " bad request" : " ok";
                    break;     
                    
                case "read":
                    response.Status += !ValidateRead(r) ? " bad request" : " ok";
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
            if (r.Path == null) return false;
            return false;
        }        
        static bool ValidateCreate(Request r)
        {
            if (r.Path == null) return false;
            if (r.Path.Any(char.IsDigit)) return false; //if there is an id in the path, we deny the request. WE controll the ids, muahaha
            return true;
        }        
        static bool ValidateDelete(Request r)
        {
            if (r.Path == null) return false;
            if (!r.Path.Any(char.IsDigit)) return false; //an id needs to be provided, cant delete nothing
            return true;
        }        
        static bool ValidateRead(Request r)
        {
            if (r.Path == null) return false;
            return false;
        }






    }    
}
