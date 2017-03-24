using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace WindowsGame1
{
   
    /// <summary>
    /// Represents UDP client responsible for receiving/sending raw data from/to the Arena Server
    /// </summary>
    class ArenaClient
    {
        private const int receptionPort = 11000;
        private Arena arena;
        private IPAddress remoteAddress;
        //Work buffers
        public byte[] send_buffer = new byte[1024];
        public byte[] receive_buffer = new byte[1024];
        //End work buffers

        public ArenaClient(Arena arena, string IP)
        {
            this.arena = arena;
            remoteAddress = IPAddress.Parse(IP);
            
        }
        public ArenaClient(Arena arena)
        {
            this.arena = arena;
            remoteAddress = IPAddress.Parse("82.192.75.178");
        }

        public void StartListener()
        {
            bool done = false;
            UdpClient listener = new UdpClient(receptionPort + 1000 + arena.Me.id);
            IPEndPoint RemoteEndPoint = new IPEndPoint(remoteAddress, receptionPort-1);  
            listener.AllowNatTraversal(true);

            //UDP hole punch
            listener.Send(new byte[10], 10, RemoteEndPoint);
            
            //start listening
            try
            {
                while (!done)   
                {
                    Thread.Sleep(15);
                    this.receive_buffer = listener.Receive(ref RemoteEndPoint);
                    //System.Console.WriteLine("Received packet");
                   
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

        public void StartSender()
        {
            preparePacket();
            SubscribeToArena();
            bool done = false;

            IPEndPoint RemoteEndPoint = new IPEndPoint(remoteAddress, receptionPort + arena.Me.id);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                while (!done)
                {                 
                    preparePacket();
                    server.SendTo(send_buffer, send_buffer.Length, SocketFlags.None, RemoteEndPoint);
                    //System.Console.WriteLine("Sent packet");
                    Thread.Sleep(15);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                server.Close();
            }
        }

        public void SubscribeToArena()
        {
            IPEndPoint RemoteEndPoint = new IPEndPoint(remoteAddress, receptionPort);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                server.SendTo(send_buffer, send_buffer.Length, SocketFlags.None, RemoteEndPoint);
                System.Console.WriteLine("Tried to subscribe");
                Thread.Sleep(15);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                server.Close();
            }
        }

        private void preparePacket()
        {
            this.send_buffer = this.arena.GetMyStatePacket();
        }

    }
}
