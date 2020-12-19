using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RCApp
{
    class RCListener
    {
        UdpClient udp;
        IPEndPoint endpoint;

        public RCListener(int port)
        {
            endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            udp = new UdpClient(endpoint);
        }

        public async Task Listen()
        {
            Console.WriteLine("RCListener.Listen : waiting for message");
            byte[] bytes = (await udp.ReceiveAsync()).Buffer;
            Console.WriteLine("RCListener.Listen : ye got mail!");
            string rawMessage = Encoding.UTF8.GetString(bytes);

            if (rawMessage.StartsWith("rwRegionCastData"))
            {
                Console.WriteLine("RCListener.Listen : message has correct header");

                // custom parsing of UDP from mod
                Dictionary<string, string> message = Parsing.ParseUdpMessage(rawMessage);
                OnMessageReceived(message);
            }
        }

        protected void OnMessageReceived(Dictionary<string, string> message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, message);
            }
        }

        public delegate void ListenerEventHandler(RCListener client, Dictionary<string, string> message);
        public static event ListenerEventHandler MessageReceived;
    }
}
