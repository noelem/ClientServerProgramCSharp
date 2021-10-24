using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Utillities;

namespace Server
{
    class ServerProgram
    {
        enum METHOD_TYPES
        {
            create,
            read,
            update,
            delete,
            echo
        }
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
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                string returns = JsonConvert.SerializeObject(RequestHandler(message), serializerSettings);
                Console.WriteLine(returns);
                client.Write(returns);
                //client.Write("{\"status\":\"4 bad request\", \"body\":null}");
            }

        }
        static Response RequestHandler(Request r)
        {
            Response response = new();
            response.Status += "4";
            if (r.Method == null) { 
                response.Status += " missing method"; 
            } 
            else if (r.Method == "echo") { 
                response.Body = r.Body; 
            } else {
                if(!Enum.IsDefined(typeof(METHOD_TYPES), r.Method))
                {
                    response.Status += " illegal method";
                }
            } 

            if (r.Date == null) { response.Status += " missing date"; } else
            {
                if (!r.Date.All(char.IsDigit)) { response.Status += " illegal date"; }
            }
            if (r.Path == null) { response.Status += " missing resource"; }
            if (r.Body == null) {
                if(r.Method != "read")
                {
                    response.Status += " missing body";
                }
            } else if (r.Body.Length>1)
            {
                try
                {
                    var test = JToken.Parse(r.Body);
                }
                catch (Exception)
                {
                    response.Status += " illegal body";
                }
                
            }
            switch (r.Method)
            {
                case "update":
                    response.Status += !ValidateUpdate(r) ? " bad request" : " ok";
                    break;

                case "create":
                    response.Status += !ValidateCreate(r) ? " bad request" : " ok";
                    if (response.Status.Contains(" ok"))
                        response = CreateMethodHandler(r);
                    break;                
                
                case "delete":
                    response.Status += !ValidateDelete(r) ? " bad request" : " ok";
                    break;     
                    
                case "read":
                    response.Status += !ValidateRead(r) ? " bad request" : " ok";
                    break;

                case "echo":
                    break;
            }
               
            
            return response;
        }
        static Response CreateMethodHandler(Request r)
        {
            Category c1 = JsonConvert.DeserializeObject<Category>(r.Body);
            Category c = new Category { Id = 1, Name = c1.Name };
            
            return new Response() { Status = " ok", Body = Regex.Unescape(JsonConvert.SerializeObject(c)) };
        }

        static bool ValidateUpdate(Request r)
        {
            if (r.Path == null) return false;
            if (r.Body == null) return false;
            return false;
        }        
        static bool ValidateCreate(Request r)
        {
            if (r.Path == null) return false;
            if (r.Body == null) return false;
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
            if (!r.Path.StartsWith("/api/categories/")) return false;
            return false;
        }

    }
    public class Category
    {
        [JsonPropertyName("cid")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
