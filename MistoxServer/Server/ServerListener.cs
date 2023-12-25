using MistoxServer.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// Client Connections

namespace MistoxServer.Server {
    public class mTCPListener : IDisposable {

        public event EventHandler onConnected;
        public event EventHandler onDisconnected;
        public event EventHandler onReceive;

        TcpListener Listener;
        bool Alive;
        int port;

        public mTCPListener( int Port ) {
            port = Port;
            Alive = true;
            Thread ConnectionThread = new Thread(ListenerThread);
            ConnectionThread.Start();
        }

        void ListenerThread() {
            Listener = new TcpListener( IPAddress.IPv6Any, port );
            Listener.Server.DualMode = true;
            Listener.Start();
            while( Alive ) {
                TcpClient client = Listener.AcceptTcpClient();

                Connection user = new Connection(){
                    slowClient = new mTCPServer(client),
                    fastClient = new IPEndPoint( ((IPEndPoint)client.Client.RemoteEndPoint).Address, port ),
                    ID = new Random().Next(1, 10000000),
                };

                Console.WriteLine( "New User Connected" );

                onConnected?.Invoke( user, new EventArgs() );
                user.slowClient.onDisconnected += onDisconnected;
                user.slowClient.onReceived += onReceive;

                Thread receiveThread = new Thread(() => user.slowClient.ReceiveThread(user));
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
