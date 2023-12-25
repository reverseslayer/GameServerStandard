using MistoxServer.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// Client Connections

namespace MistoxServer.Server {
    public class mTCPListener {

        public event EventHandler onConnected;
        public event EventHandler onDisconnected;
        public event EventHandler onReceive;

        TcpListener Listener;
        bool Alive;
        int port;

        public mTCPListener( int Port ) {
            port = Port;
            Alive = true;
            Thread ConnectionThreadv6 = new Thread(ListenerThreadV6);
            ConnectionThreadv6.Start();
            Thread ConnectionThreadv4 = new Thread(ListenerThread);
            ConnectionThreadv4.Start();
        }

        void ListenerRoutine() {
            TcpClient client = Listener.AcceptTcpClient();

            Connection user = new Connection(){
                slowClient = new mTCPServer(client),
                fastClient = (IPEndPoint)client.Client.RemoteEndPoint,
                ID = new Random().Next(1, 10000000)
            };

            onConnected?.Invoke( user, new EventArgs() );
            user.slowClient.onDisconnected += onDisconnected;
            user.slowClient.onReceived += onReceive;

            Thread receiveThread = new Thread(() => user.slowClient.ReceiveThread(user));
            receiveThread.Start();

            Console.WriteLine( "New User Connected" );
        }

        void ListenerThreadV6() {
            Listener = new TcpListener( IPAddress.IPv6Any, port );
            Listener.Start();
            while( Alive ) {
                ListenerRoutine();
            }
        }

        void ListenerThread() {
            Listener = new TcpListener( IPAddress.Any, port );
            Listener.Start();
            while( Alive ) {
                ListenerRoutine();
            }
        }

        public void Dispose() {
            Alive = false;
            Listener.Stop();
            Listener = null;
        }
    }

}
