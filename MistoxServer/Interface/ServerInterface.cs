using MistoxServer.Client;
using MistoxServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MistoxServer {
    public class ServerInterface : IMistoxServer {

        public mTCPServer SlowUpdateServer;
        public mUDPServer FastUpdateServer;

        public List<Connection> Connections = new List<Connection>();

        public event EventHandler onReceive;

        public ServerInterface( int port ) {
            SlowUpdateServer = new mTCPServer( port );
            FastUpdateServer = new mUDPServer( port );

            SlowUpdateServer.onConnected += OnConnected;
            FastUpdateServer.onReceived += OnFastReceived;
        }

        void OnConnected( object sender, EventArgs e ) {
            Connection user = (Connection)sender;
            user.onReceived += OnSlowReceived;
            user.onDisconnected += OnDisconnected;
            Connections.Add( user );
        }

        void OnSlowReceived( object sender, EventArgs e ) {
            onReceive?.Invoke( sender, e );
            Send( sender, SendType.SlowUpdate );
        }

        void OnFastReceived( object sender, EventArgs e ) {
            onReceive?.Invoke( sender, e );
            Send( sender, SendType.FastUpdate );
        }

        void OnDisconnected( object sender, EventArgs e ) {

        }

        public void Send<Packet>( Packet data, SendType speed ) {
            if (speed == SendType.SlowUpdate) {
                foreach( Connection cur in Connections ) {
                    cur.Send( data );
                }
            } else {
                foreach( Connection cur in Connections ) {
                    FastUpdateServer.Send( data, cur.remoteAddress.Address );
                }
            }
        }
    }
}
