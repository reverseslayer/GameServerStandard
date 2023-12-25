using MistoxServer.Server;
using MistoxServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MistoxServer.Server {
    public class mTCPServer : IDisposable {
        public TcpClient slowClient;

        public event EventHandler onReceived;
        public event EventHandler onDisconnected;

        public mTCPServer( TcpClient client ) {
            slowClient = client;
            slowClient.Client.NoDelay = true;
        }

        bool Alive = true;
        public void ReceiveThread( Connection Client ) {
            bool connected = true;
            while( Alive && connected ) {
                try {
                    byte[] StreamData = new byte[1024];
                    int bytesRead = slowClient.GetStream().Read(StreamData, 0, StreamData.Length);
                    if( bytesRead > 0 ) {
                        dynamic data = mSerialize.tReceive( StreamData.Sub( 0, bytesRead ) );
                        if( data != null ) {
                            onReceived?.Invoke( data, new EventArgs() );
                        }
                    }
                } catch( Exception e ) {
                    Console.WriteLine( "A user has disconnected for reason : " + e.ToString() );
                    connected = false;
                    onDisconnected?.Invoke( Client, new EventArgs() );
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
