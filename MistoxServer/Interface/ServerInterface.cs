using MistoxServer.Server;
using System;
using System.Collections.Generic;

namespace MistoxServer {
    public class ServerInterface : IMistoxServer {

        public mTCPServer SlowUpdateServer;

        public List<Connection> Connections = new List<Connection>();

        public event EventHandler onReceive;
        public event EventHandler onDisconnect;

        public ServerInterface( int port ) {
            SlowUpdateServer = new mTCPServer( port );
            SlowUpdateServer.onConnected += OnConnected;
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
            Connection user = (Connection)sender;
            onDisconnect?.Invoke( sender, e );
            Connections.Remove(user);
        }

        public void Send<Packet>( Packet data, SendType speed ) {
            if (speed == SendType.SlowUpdate) {
                foreach( Connection cur in Connections ) {
                    cur.Send( data );
                }
            } else {
                foreach( Connection cur in Connections ) {
                    cur.fastClient.Send( data );
                }
            }
        }
    }
}
