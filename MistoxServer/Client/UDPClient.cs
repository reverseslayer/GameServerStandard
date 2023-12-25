using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

// Network Sync

namespace MistoxServer.Client {
    public class mUDPClient : IDisposable {

        public event EventHandler onReceived;

        Socket udpClient;
        bool Alive;

        public mUDPClient( IPEndPoint ServerAddress ) {
            udpClient = new Socket( AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp );
            udpClient.DualMode = true;
            udpClient.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.ReuseAddress, true );
            udpClient.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IpTimeToLive, 128 );
            udpClient.Bind( new IPEndPoint( IPAddress.IPv6Any, ServerAddress.Port ) );
            Alive = true;
            Thread Client = new Thread(ReceiveThread);
            Client.Start();
        }

        void ReceiveThread() {
            while( Alive ) {
                try {
                    byte[] buffer = new byte[1024];
                    int bytesRead = udpClient.Receive( buffer );
                    dynamic data = mSerialize.uReceive( buffer.Sub(0, bytesRead) );
                    if( data != null ) {
                        onReceived?.Invoke( data, new EventArgs() );
                    }
                } catch( Exception e ) {

                }
            }
        }

        public void Send<Packet>( Packet Data, IPEndPoint remoteHost ) {
            byte[] byteData = mSerialize.PacketSerialize( Data );
            udpClient.SendTo( byteData, new IPEndPoint( remoteHost.Address, 6500 ) );
        }

        public void Dispose() {
            Alive = false;
            udpClient.Close();
            udpClient = null;
        }
    }
}
