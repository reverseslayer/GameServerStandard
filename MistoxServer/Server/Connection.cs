using System;
using System.Net.Sockets;
using System.Net;
using MistoxServer.Client;
using Newtonsoft.Json;

namespace MistoxServer.Server {

    public class Connection : IDisposable {
        public int ID;
        public string UserName;
        public TcpClient slowClient;
        public mUDPServer fastClient;

        public event EventHandler onReceived;
        public event EventHandler onDisconnected;

        public Connection( TcpClient client ) {
            ID = new Random().Next( 1, 1000000 );
            slowClient = client;
            fastClient = new mUDPServer( ( IPEndPoint )client.Client.RemoteEndPoint );
        }

        bool Alive = true;
        public void ReceiveThread( Connection Client ) {
            bool connected = true;
            while( Alive && connected ) {
                try {
                    byte[] StreamData = new byte[1024];
                    int bytesRead = slowClient.GetStream().Read(StreamData, 0, StreamData.Length);
                    dynamic data = mSerialize.tReceive( StreamData.Sub( 0, bytesRead ) );
                    if (data != null ) {
                        onReceived?.Invoke( data, new EventArgs() );
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
