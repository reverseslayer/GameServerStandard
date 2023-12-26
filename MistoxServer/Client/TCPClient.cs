using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

//IP Range Updater

namespace MistoxServer.Client {
    public class mTCPClient : IDisposable {

        TcpClient Server;

        public event EventHandler onReceived;
        public bool Alive;
        bool notified = false;

        public mTCPClient( IPEndPoint ServerAddress ) {
            Server = new TcpClient( AddressFamily.InterNetwork );
            Server.NoDelay = true;
            Server.Connect( ServerAddress );
            Alive = true;
            Thread RThread = new Thread(async () => { await ReceiveThread(); });
            RThread.Start();
        }

        async Task ReceiveThread() {
            using( NetworkStream ns = Server.GetStream() ) {
                try {
                    while( Alive ) {
                        if( Server.Connected && !notified ) {
                            Console.WriteLine( "Connected to server" );
                            notified = true;
                        } else if( !Server.Connected ) {
                            Console.WriteLine( "Disconnected from server" );
                            Alive = false;
                        }
                        byte[] StreamData = new byte[1024];
                        int bytesRead = await ns.ReadAsync(StreamData, 0, StreamData.Length);
                        if( bytesRead > 0 ) {
                            dynamic data = mSerialize.tReceive( StreamData.Sub( 0, bytesRead ) );
                            if( data != null ) {
                                onReceived?.Invoke( data, new EventArgs() );
                            }
                        }
                    }
                } catch( Exception ) {
                    Console.WriteLine( "You have disconnected from the server" );
                    Alive = false;
                }
            }
        }

        public async Task Send<Packet>(Packet packet) {
            byte[] data = mSerialize.PacketSerialize( packet );
            await Server.GetStream().WriteAsync( data, 0, data.Length );
        }

        public void Dispose() {
            Alive = false;
            Server.Close();
            Server = null;
        }
    }
}
