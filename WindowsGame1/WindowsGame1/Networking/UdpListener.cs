using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace WindowsGame1
{

    /// <summary>
    /// Represents UDP client responsible for receiving/sending raw data from the Arena Server
    /// </summary>
    class UdpListener
    {
        private const int listenPort = 11000;
        private IPAddress address;
        public UdpListener(string IP)
        {
            address = IPAddress.Parse(IP);
        }
        public UdpListener()
        {
            address = IPAddress.Parse("82.192.75.178");
        }
        public void SubscribeForSnapshots() 
        {
            //Tell the server to include you in subscribelist and get ready to receive arena-snapshot packets, mkay?
        }
        public void StartListener()
        {
            bool done = false;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(address, listenPort);

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                        groupEP.ToString(),
                        Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
