using MistoxServer.Client;
using MistoxServer.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MistoxServer {
    public class ServerInterface : IMistoxServer {

        mUDPClient FastUpdateServer;
        mTCPListener SlowUpdateServer;
        List<Connection> Connections = new List<Connection>();

        public event EventHandler onReceive;

        public ServerInterface( int port ) {
            FastUpdateServer = new mUDPClient( new IPEndPoint( IPAddress.IPv6Any, port ) );
            SlowUpdateServer = new mTCPListener( port );

            SlowUpdateServer.onConnected += OnConnected;
            FastUpdateServer.onReceived += onReceive;
            SlowUpdateServer.onDisconnected += OnDisconnected;

            Console.WriteLine( "The Server is initilized and waiting for clients to connect at port : " + port );
        }

        void OnConnected( object sender, EventArgs e ) {
            Connection user = (Connection)sender;
            user.slowClient.onReceived += onReceive;
            user.slowClient.onDisconnected += OnDisconnected;
            Connections.Add( user );
        }

        void OnDisconnected( object sender, EventArgs e ) {
            Connection user = (Connection)sender;
            Connections.Remove( user );
        }

        public void Send<Packet>( Packet data, SendType speed ) {
            if (speed == SendType.SlowUpdate) {
                foreach( Connection cur in Connections ) {
                    cur.slowClient.Send( data );
                }
            } else {
                foreach( Connection cur in Connections ) {
                    FastUpdateServer.Send( data, cur.fastClient );
                }
            }
        }
    }
}
