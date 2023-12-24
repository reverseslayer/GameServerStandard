using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// Client Connections

namespace MistoxServer.Server {
    public class mTCPServer {

        public event EventHandler onConnected; // returns connection object

        TcpListener Listener;
        bool Alive;
        int port;

        public mTCPServer( int Port ) {
            port = Port;
            Alive = true;
            Thread ConnectionThreadv6 = new Thread(ListenerThreadV6);
            ConnectionThreadv6.Start();
            Thread ConnectionThreadv4 = new Thread(ListenerThread);
            ConnectionThreadv4.Start();
        }

        void ListenerThreadV6() {
            Listener = new TcpListener( IPAddress.IPv6Any, port );
            Listener.Start();
            while( Alive ) {
                TcpClient client = Listener.AcceptTcpClient();
                Console.WriteLine( "New User Connected" );
                Connection user = new Connection( client );
                Thread receiveThread = new Thread(() => user.ReceiveThread(user));
                receiveThread.Start();
                onConnected?.Invoke( user, new EventArgs() );
            }
        }

        void ListenerThread() {
            Listener = new TcpListener( IPAddress.Any, port );
            Listener.Start();
            while( Alive ) {
                TcpClient client = Listener.AcceptTcpClient();
                Console.WriteLine( "New User Connected" );
                Connection user = new Connection( client );
                Thread receiveThread = new Thread(() => user.ReceiveThread(user));
                receiveThread.Start();
                onConnected?.Invoke( user, new EventArgs() );
            }
        }

        public void Dispose() {
            Alive = false;
            Listener.Stop();
            Listener = null;
        }
    }

}
