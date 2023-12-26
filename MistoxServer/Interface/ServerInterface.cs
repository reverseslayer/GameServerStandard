using MistoxServer.Client;
using MistoxServer.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MistoxServer {
    public class ServerInterface : IMistoxServer {

        mUDPClient FastUpdateServer;
        mTCPListener SlowUpdateServer;
        List<Connection> Connections = new List<Connection>();

        public event EventHandler onSlowReceive;
        public event EventHandler onFastReceive;

        public ServerInterface( int port, ServerMode mode ) {
            FastUpdateServer = new mUDPClient( new IPEndPoint( IPAddress.IPv6Any, port ), mode );
            SlowUpdateServer = new mTCPListener( port, mode );

            SlowUpdateServer.onConnected += OnConnected;
            FastUpdateServer.onReceived += ( object o, EventArgs e ) => { onFastReceive?.Invoke( o, e ); };
            SlowUpdateServer.onDisconnected += OnDisconnected;

            Console.WriteLine( "The Server is initilized and waiting for clients to connect at port : " + port );
        }

        void OnConnected( object sender, EventArgs e ) {
            Connection user = (Connection)sender;
            user.slowClient.onReceived+= ( object o, EventArgs e ) => { onSlowReceive?.Invoke( o, e ); };
            user.slowClient.onDisconnected += OnDisconnected;
            Connections.Add( user );
        }

        void OnDisconnected( object sender, EventArgs e ) {
            Connection user = (Connection)sender;
            Connections.Remove( user );
        }

        public async Task Send<Packet>( Packet data, SendType speed ) {
            if (speed == SendType.SlowUpdate) {
                foreach( Connection cur in Connections ) {
                    await cur.slowClient.Send( data );
                }
            } else {
                foreach( Connection cur in Connections ) {
                    await FastUpdateServer.Send( data, cur.fastClient );
                }
            }
        }
    }
}
