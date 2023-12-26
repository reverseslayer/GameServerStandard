using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

// Client Connections

namespace MistoxServer.Server {
    public class mTCPListener : IDisposable {

        public event EventHandler onConnected;
        public event EventHandler onDisconnected;
        public event EventHandler onReceive;

        TcpListener Listener;
        ServerMode ServerMode;
        bool Alive;
        int port;

        public mTCPListener( int Port, ServerMode mode ) {
            port = Port;
            Alive = true;
            ServerMode = mode;
            Thread ConnectionThread = new Thread(async () => await ListenerThread() );
            ConnectionThread.Start();
        }

        async Task ListenerThread() {
            Listener = new TcpListener( IPAddress.Any, port );
            Listener.Start();
            while( Alive ) {
                TcpClient client = await Listener.AcceptTcpClientAsync();

                Connection user = new Connection(){
                    slowClient = new mTCPServer(client, ServerMode),
                    fastClient = new IPEndPoint( ((IPEndPoint)client.Client.RemoteEndPoint).Address, port ),
                    ID = new Random().Next(1, 10000000),
                };

                Console.WriteLine( "New User Connected" );

                onConnected?.Invoke( user, new EventArgs() );
                user.slowClient.onDisconnected += onDisconnected;
                user.slowClient.onReceived += onReceive;

                Thread receiveThread = new Thread(async () => await user.slowClient.ReceiveThread(user) );
                receiveThread.Start();
            }
        }

        public void Dispose() {
            Alive = false;
            Listener.Stop();
            Listener = null;
        }
    }

}
