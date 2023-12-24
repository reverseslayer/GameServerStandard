using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// Network Sync

namespace MistoxServer.Client {
    public class mUDPClient {

        UdpClient Server;
        IPEndPoint ep;

        public event EventHandler onReceived;
        bool Alive;

        public mUDPClient( IPEndPoint ServerAddress ) {
            Server = new UdpClient( new IPEndPoint(IPAddress.Any, ServerAddress.Port) );
            ep = ServerAddress;
            Alive = true;
            Thread Client = new Thread(ReceiveThread);
            Client.Start();
        }

        void ReceiveThread() {
            while( Alive ) {
                try {
                    IPEndPoint receive = new IPEndPoint( IPAddress.Any, ep.Port );
                    while( Alive ) {
                        dynamic data = mSerialize.uReceive( Server.Receive( ref ep ) );
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
            Server.Send( byteData, byteData.Length, ep );
        }

        public void Dispose() {
            Alive = false;
            Server.Close();
            Server = null;
        }
    }
}
