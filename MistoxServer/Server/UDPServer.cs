using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// Network Sync

namespace MistoxServer.Client {
    public class mUDPServer {

        UdpClient Client;
        IPEndPoint ep;

        public event EventHandler onReceived;
        bool Alive;

        public mUDPServer( IPEndPoint endpoint ) {
            Client = new UdpClient( new IPEndPoint( IPAddress.Any, endpoint.Port ) );
            ep = endpoint;
            Alive = true;
            Thread thread = new Thread(ReceiveThread);
            thread.Start();
        }

        void ReceiveThread() {
            while( Alive ) {
                try {
                    while( Alive ) {
                        dynamic data = mSerialize.uReceive( Client.Receive( ref ep ) );
                        if( data != null ) {
                            onReceived?.Invoke( data, new EventArgs() );
                        }
                    }
                } catch( Exception e ) {
                    Console.WriteLine( "A user has disconnected for reason : " + e.ToString() );
                }
            }
        }

        public void Send<Packet>( Packet Data ) {
            byte[] byteData = mSerialize.PacketSerialize( Data );
            Client.Send( byteData, byteData.Length, ep );
        }

        public void Dispose() {
            Alive = false;
            Client.Close();
            Client = null;
        }
    }
}
