using MistoxServer.Client;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MistoxServer {
    public class ClientInterface : IMistoxServer {

        mTCPClient SlowUpdate;
        mUDPClient FastUpdate;
        IPEndPoint mPEndPoint;

        public event EventHandler onConnected;
        public event EventHandler onSlowReceive;
        public event EventHandler onFastReceive;
        public event EventHandler onDisconnected;

        public ClientInterface( string IpOrHostName, int Port ) {
            // Get Server IP
            IPHostEntry host = Dns.GetHostEntry( IpOrHostName );

            foreach( IPAddress entry in host.AddressList ) {
                if ( entry.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ) {
                    mPEndPoint = new IPEndPoint( entry, Port );
                }
            }

            Console.WriteLine( "The client is initilized and trying to connect to the server at ip : " + mPEndPoint.Address );
            // Make a UDP connection to the server
            try {
                FastUpdate = new mUDPClient( mPEndPoint, ServerMode.Authoritative );
                FastUpdate.onReceived += (object o, EventArgs e) => { onFastReceive?.Invoke( o, e ); }; 
            } catch( Exception e ) {
                Console.WriteLine( "An error has occured with the connection to the server. Error { " );
                Console.WriteLine( e.ToString() );
                Console.WriteLine( "}" );
            }
            // Make a TCP connection to the server
            try {
                SlowUpdate = new mTCPClient( mPEndPoint );
                SlowUpdate.onConnected += ( object o, EventArgs e ) => { onConnected?.Invoke( o, e ); };
                SlowUpdate.onReceived += ( object o, EventArgs e ) => { onSlowReceive?.Invoke( o, e ); };
                SlowUpdate.onDisconnected += ( object o, EventArgs e ) => { onDisconnected?.Invoke( o, e ); };
            } catch( Exception e ) {
                Console.WriteLine( "An error has occured with the connection to the server. Error { " );
                Console.WriteLine( e.ToString() );
                Console.WriteLine( "}" );
            }
        }

        public async Task Send<Packet>(Packet data, SendType speed) {
            if (SendType.SlowUpdate == speed) {
                await SlowUpdate.Send( data );
            } else {
                await FastUpdate.Send( data, mPEndPoint );
            }
        }
    }
}
