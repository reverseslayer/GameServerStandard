using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MistoxServer.Server {
    public class mTCPServer : IDisposable {
        public TcpClient slowClient;
        ServerMode Mode;

        public event EventHandler onReceived;
        public event EventHandler onDisconnected;

        public mTCPServer( TcpClient client, ServerMode mode ) {
            slowClient = client;
            slowClient.Client.NoDelay = true;
            Mode = mode;
        }

        bool Alive = true;
        public async Task ReceiveThread( Connection Client ) {
            bool connected = true;
            while( Alive && connected ) {
                try {
                    byte[] StreamData = new byte[1024];
                    int bytesRead = await slowClient.GetStream().ReadAsync(StreamData, 0, StreamData.Length);
                    if( bytesRead > 0 ) {
                        if ( Mode == ServerMode.Passive) {
                            onReceived?.Invoke( StreamData.Sub( 0, bytesRead ), new EventArgs() );
                        } else if ( Mode == ServerMode.Authoritative) {
                            dynamic data = mSerialize.tReceive( StreamData.Sub( 0, bytesRead ) );
                            if( data != null ) {
                                onReceived?.Invoke( data, new EventArgs() );
                            }
                        }
                    }
                } catch( Exception ) {
                    Console.WriteLine( "User disconnected" );
                    connected = false;
                    onDisconnected?.Invoke( Client, new EventArgs() );
                }
            }
        }

        public async Task Send<T>( T packet ) {
            if( Mode == ServerMode.Authoritative ) {
                byte[] byteData = mSerialize.PacketSerialize( packet );
                await slowClient.GetStream().WriteAsync( byteData, 0, byteData.Length );
            } else if( Mode == ServerMode.Passive ) {
                byte[] byteData = packet as byte[];
                await slowClient.GetStream().WriteAsync( byteData, 0, byteData.Length );
            }
        }

        public void Dispose() {
            Alive = false;
        }
    }
}
