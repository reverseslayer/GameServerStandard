using MistoxServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MistoxServer.Server {

    public class Connection : IDisposable {
        public int ID;
        public string UserName;
        public TcpClient slowClient;
        public IPEndPoint remoteAddress;

        public event EventHandler onReceived;
        public event EventHandler onDisconnected;

        public Connection( TcpClient client ) {
            ID = new Random().Next( 1, 1000000 );
            slowClient = client;
            remoteAddress = ( IPEndPoint )client.Client.RemoteEndPoint;
        }

        bool Alive = true;
        public void ReceiveThread( Connection Client ) {
            bool connected = true;
            while( Alive && connected ) {
                try {
                    byte[] BufferedData = new byte[0];
                    while( Alive ) {
                        byte[] StreamData = new byte[1024];
                        int bytesRead = Client.slowClient.GetStream().Read(StreamData, 0, StreamData.Length);
                        BufferedData.Join( StreamData );
                        if( BufferedData.Length > 4 ) {
                            int dataLength = BitConverter.ToInt32( BufferedData.Sub(0, 4) );
                            if( BufferedData.Length >= dataLength + 4 ) {
                                Client.onReceived.Invoke( mSerialize.PacketDeserialize( BufferedData.Sub( 0, dataLength + 4 ) ), new EventArgs() );
                                BufferedData.Sub( dataLength + 4, BufferedData.Length - (dataLength + 4) );
                            }
                        }
                    }
                } catch( Exception e ) {
                    Console.WriteLine( "A user has disconnected for reason : " + e.ToString() );
                    connected = false;
                    onDisconnected.Invoke( Client, new EventArgs() );
                }
            }
        }

        public void Send<T>( T packet ) {
            byte[] data = mSerialize.PacketSerialize( packet );
            slowClient.GetStream().Write( data, 0, data.Length );
        }

        public void Dispose() {
            Alive = false;
        }
    }

}
