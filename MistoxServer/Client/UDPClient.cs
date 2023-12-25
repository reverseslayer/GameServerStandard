using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

// Network Sync

namespace MistoxServer.Client {
    public class mUDPClient {

        public event EventHandler onReceived;

        Socket udpClient;
        bool Alive;

        public mUDPClient( IPEndPoint ServerAddress ) {
            udpClient = new Socket( ServerAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp );
            if (ServerAddress.AddressFamily == AddressFamily.InterNetwork ) {
                udpClient.SetSocketOption( SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true );
                udpClient.Bind( new IPEndPoint( IPAddress.Any, ServerAddress.Port ) );
            } else if(ServerAddress.AddressFamily == AddressFamily.InterNetworkV6 ) {
                udpClient.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.ReuseAddress, true );
                udpClient.Bind( new IPEndPoint( IPAddress.IPv6Any, ServerAddress.Port ) );
            }
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
            udpClient.SendTo( byteData, remoteHost );
        }

        public void Dispose() {
            Alive = false;
            udpClient.Close();
            udpClient = null;
        }
    }
}
