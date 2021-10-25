using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
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
        public static List<Category> categories = new List<Category>() ;
        static void Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Loopback, 5000);
            server.Start();
            Console.WriteLine("Server started");

            categories.Add(new Category { Id = 1, Name = "Beverages" });
            categories.Add(new Category { Id = 2, Name = "Condiments" });
            categories.Add(new Category { Id = 3, Name = "Confections" });

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
                Console.WriteLine($"Recieved message: {messageFromJson}");
                Request message = JsonConvert.DeserializeObject<Request>(messageFromJson, settings);


                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                string returns = JsonConvert.SerializeObject(RequestHandler(message), serializerSettings);
                
                Console.WriteLine($"Sent message back: {returns}");
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
                if(r.Method != "read" && r.Method != "delete")
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
                    if (response.Status.Contains("ok") && !response.Status.Contains("illegal body"))
                    {
                        int toUpdate = categories.FindIndex(c => c.Id.ToString() == r.Path.Split("/")[3]);
                        if (toUpdate != -1) // -1 means not found in our list
                        {
                            Console.WriteLine(toUpdate);
                            Category updatedValues = FromJson<Category>(r.Body);
                            Console.WriteLine(updatedValues.Name);
                            Console.WriteLine(updatedValues.Id);
                            categories[toUpdate] = updatedValues;
                            response.Status = "3 updated";
                        } else
                        {
                            response.Status = "5 not found";
                        }
                    }
                    break;

                case "create":
                    response.Status += !ValidateCreate(r) ? " bad request" : " ok";
                    if (response.Status.Contains(" ok"))
                        response = CreateMethodHandler(r);
                    break;                
                
                case "delete":
                    response.Status += !ValidateDelete(r) ? " bad request" : " ok";
                    if (response.Status.Contains("ok"))
                    {
                        if (!categories.Any(c => c.Id.ToString() == r.Path.Split("/")[3]))
                        {
                            response.Status = "5 not found";
                        }
                        else
                        {
                            response.Status = DeleteMethodHandler(r);
                        }
                    }
                    break;     
                    
                case "read":
                    response.Status += !ValidateRead(r) ? " bad request" : "1 ok";
                    if (response.Status.Contains("ok"))
                    {
                        if (r.Path.EndsWith("api/categories")) //read all categories
                        {
                            response.Status = "1 Ok";
                            response.Body = ToJson(categories);
                            break;
                        }
                        if (!categories.Any(c => c.Id.ToString() == r.Path.Split("/")[3]))
                        {
                            response.Status = "5 not found";
                        }
                        else
                        {
                            response.Status = "1 Ok";
                            response.Body = categories.Find(c => c.Id.ToString() == r.Path.Split("/")[3]).ToJson();
                        }
                    }

                    break;

                case "echo":
                    break;
            }
               
            
            return response;
        }
        static Response CreateMethodHandler(Request r)
        {
            Category c1 = JsonConvert.DeserializeObject<Category>(r.Body);
            Category c = new Category { Id = categories.Count, Name = c1.Name };
            categories.Add(c);

            return new Response() { Status = " ok", Body = c.ToJson() };
        }
        static string DeleteMethodHandler(Request r)
        {
            int idToDelete = int.Parse(r.Path.Split("/")[3]);
            categories.RemoveAll(c => c.Id == idToDelete);
            return "1 ok";
        }

        static bool ValidateUpdate(Request r)
        {
            if (r.Path == null) return false;
            if (r.Body == null) return false;
            if (!r.Path.StartsWith("/api/categories/")) return false;
            return true;
        }        
        static bool ValidateCreate(Request r)
        {
            if (r.Path == null) return false;
            if (r.Body == null) return false;
            if (r.Path.Any(char.IsDigit)) return false; //if there is an id in the path, we deny the request.
            return true;
        }        
        static bool ValidateDelete(Request r)
        {
            if (r.Path == null) return false;
            if (!r.Path.Any(char.IsDigit)) return false; //an id needs to be provided.
            return true;
        }        
        static bool ValidateRead(Request r)
        {
            if (r.Path == null) return false;
            if (!r.Path.StartsWith("/api/categories")) return false;
            if (r.Path.EndsWith("/api/categories")) return true;
            if (!r.Path.Any(char.IsDigit)) return false;
            return true;
        }
        public static string ToJson(List<Category> r)
        {
            return System.Text.Json.JsonSerializer.Serialize(r, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        public static T FromJson<T>(string element)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(element, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
    public class Category
    {
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        [JsonPropertyName("cid")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
