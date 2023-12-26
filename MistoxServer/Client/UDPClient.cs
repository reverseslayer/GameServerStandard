using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

// Network Sync

namespace MistoxServer.Client {
    public class mUDPClient : IDisposable {

        public event EventHandler onReceived;
        ServerMode Mode;

        Socket udpClient;
        bool Alive;
        int Port;

        public mUDPClient( IPEndPoint ServerAddress, ServerMode mode ) {
            udpClient = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
            udpClient.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true );
            udpClient.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 128 );
            udpClient.Bind( new IPEndPoint( IPAddress.Any, ServerAddress.Port ) );
            Alive = true;
            Port = ServerAddress.Port;
            Mode = mode;
            Thread Client = new Thread(async() => { await ReceiveThread(); });
            Client.Start();
        }

        async Task ReceiveThread() {
            while( Alive ) {
                try {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await udpClient.ReceiveAsync( buffer );
                    if (Mode == ServerMode.Passive) {
                        onReceived?.Invoke( buffer.Sub( 0, bytesRead ), new EventArgs() );
                    } else if (Mode == ServerMode.Authoritative) {
                        dynamic data = mSerialize.uReceive( buffer.Sub(0, bytesRead) );
                        if( data != null ) {
                            onReceived?.Invoke( data, new EventArgs() );
                        }
                    }
                } catch( Exception ) {

                }
            }
        }

        public async Task Send<Packet>( Packet Data, IPEndPoint remoteHost ) {
            if (Mode == ServerMode.Authoritative) {
                byte[] byteData = mSerialize.PacketSerialize( Data );
                await udpClient.SendToAsync( byteData, new IPEndPoint( remoteHost.Address, Port ) );
            } else if (Mode == ServerMode.Passive) {
                byte[] byteData = Data as byte[];
                await udpClient.SendToAsync( byteData, new IPEndPoint( remoteHost.Address, Port ) );
            }
        }

        public void Dispose() {
            Alive = false;
            udpClient.Close();
            udpClient = null;
        }
    }
}
