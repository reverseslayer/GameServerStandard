using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Network Sync

namespace MistoxServer.Client {
    public class mUDPClient {

        UdpClient Server;
        IPEndPoint ep;

        public event EventHandler onReceived;
        bool Alive;

        public mUDPClient( IPEndPoint ServerAddress ) {
            Server = new UdpClient( ServerAddress );
            ep = ServerAddress;
            Alive = true;
            Thread Client = new Thread(ReceiveThread);
            Client.Start();
        }

        void ReceiveThread() {
            while( Alive ) {
                try {
                    byte[] BufferedData = new byte[0];
                    while( Alive ) {
                        byte[] StreamData = Server.Receive( ref ep );
                        BufferedData.Join( StreamData );
                        if( BufferedData.Length > 4 ) {
                            int dataLength = BitConverter.ToInt32( BufferedData.Sub(0, 4) );
                            if( BufferedData.Length >= dataLength + 4 ) {
                                onReceived.Invoke( mSerialize.PacketDeserialize( BufferedData.Sub( 0, dataLength + 4 ) ), new EventArgs() );
                                BufferedData.Sub( dataLength + 4, BufferedData.Length - (dataLength + 4) );
                            }
                        }
                    }
                } catch( Exception e ) {
                    Console.WriteLine( "A user has disconnected for reason : " + e.ToString() );
                }
            }
        }

        public void Send<Packet>( Packet Data ) {
            byte[] byteData = mSerialize.PacketSerialize( Data );
            Server.Send( byteData, byteData.Length );
        }

        public void Dispose() {
            Alive = false;
            Server.Close();
            Server = null;
        }
    }
}
