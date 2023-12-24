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
    public class mUDPServer {

        UdpClient Client;
        IPEndPoint ep;

        public event EventHandler onReceived;
        bool Alive;

        public mUDPServer( int Port ) {
            Client = new UdpClient();
            ep = new IPEndPoint( IPAddress.Any, Port );
            Alive = true;
            Thread thread = new Thread(ReceiveThread);
            thread.Start();
        }

        void ReceiveThread() {
            while( Alive ) {
                try {
                    byte[] BufferedData = new byte[0];
                    while( Alive ) {
                        byte[] StreamData = Client.Receive( ref ep );
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

        public void Send<Packet>( Packet Data, IPAddress dest ) {
            byte[] byteData = mSerialize.PacketSerialize( Data );
            Client.Send( byteData, byteData.Length, new IPEndPoint( dest, ep.Port ) );
        }

        public void Dispose() {
            Alive = false;
            Client.Close();
            Client = null;
        }
    }
}
