using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MistoxServer.Client;

// Client Connections

namespace MistoxServer.Server {
    public class mTCPServer {

        public event EventHandler onConnected; // returns connection object

        TcpListener Listener;
        bool Alive;
        int port;

        public mTCPServer( int Port ) {
            port = Port;
            Thread ConnectionThread = new Thread(ListenerThread);
            ConnectionThread.Start();
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
                onConnected.Invoke( user, new EventArgs() );
            }
        }

        public void Dispose() {
            Alive = false;
            Listener.Stop();
            Listener = null;
        }
    }

}
