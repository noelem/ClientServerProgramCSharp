using System.IO;
using System;
using System.Net.Sockets;
using System.Text;
using Utillities;
using System.Text.Json.Serialization;

namespace Utillities
{
    
    public class Response
    {
        public string Status { get; set; }
        public string Body { get; set; }
   
    }

    public class Request
    {
        public string Method { get; set; }
        public String Path { get; set; }
        public String Body { get; set; }
        public String Date { get; set; }
   
        [JsonConstructor]
        public Request(string Method, string Path, string Body, string Date) {
   
            this.Method = Method;
            this.Path = Path;
            this.Body = Body;
            this.Date = Date;
            
        }

        /*
        public Request(string Method, string Path , DateTime Date) {
   
            Method = this.Method;
            Path = this.Path;
            Date = DateTime.Now;
            
        }*/

    }
}
