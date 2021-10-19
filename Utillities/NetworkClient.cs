using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Utillities
{
    public class NetworkClient
    {
        private TcpClient _client;

        public NetworkClient()
        {
            _client = new TcpClient();
        }

        public NetworkClient(TcpClient client)
        {
            _client = client;
        }


        public void Connect(string ip, int port)
        {
            _client.Connect("localhost", 5000);
        }

        public void Write(string msg)
        {
            _client.GetStream().Write(Encoding.UTF8.GetBytes(msg));
        }

        public string Read()
        {
            var buffer = new byte[1024];
            var rdCnt = _client.GetStream().Read(buffer);
            return Encoding.UTF8.GetString(buffer, 0, rdCnt);
        }
    }
}
