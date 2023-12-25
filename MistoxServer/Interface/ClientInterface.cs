using MistoxServer.Client;
using System;
using System.Net;

namespace MistoxServer {
    public class ClientInterface : IMistoxServer {

        mTCPClient SlowUpdate;
        mUDPClient FastUpdate;
        IPEndPoint mPEndPoint;

        public event EventHandler onSlowReceive;
        public event EventHandler onFastReceive;

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
                FastUpdate = new mUDPClient( mPEndPoint );
                FastUpdate.onReceived += onFastReceive;
            } catch( Exception e ) {
                Console.WriteLine( "An error has occured with the connection to the server. Error { " );
                Console.WriteLine( e.ToString() );
                Console.WriteLine( "}" );
            }
            // Make a TCP connection to the server
            try {
                SlowUpdate = new mTCPClient( mPEndPoint );
                SlowUpdate.onReceived += onSlowReceive;
            } catch( Exception e ) {
                Console.WriteLine( "An error has occured with the connection to the server. Error { " );
                Console.WriteLine( e.ToString() );
                Console.WriteLine( "}" );
            }
        }

        public void Send<Packet>(Packet data, SendType speed) {
            if (SendType.SlowUpdate == speed) {
                SlowUpdate.Send( data );
            } else {
                FastUpdate.Send( data, mPEndPoint );
            }
        }
    }
}
