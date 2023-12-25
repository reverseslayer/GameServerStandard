using MistoxServer.Client;
using System;
using System.Net;

namespace MistoxServer {
    public class ClientInterface : IMistoxServer {

        mTCPClient SlowUpdate;
        mUDPClient FastUpdate;
        IPEndPoint mPEndPoint;

        public event EventHandler onReceive;

        public ClientInterface( string IpOrHostName, int Port ) {
            // Get Server IP
            IPHostEntry host = Dns.GetHostEntry( IpOrHostName );
            mPEndPoint = new IPEndPoint(host.AddressList[0], Port);

            Console.WriteLine( "The client is initilized and trying to connect to the server at ip : " + mPEndPoint.Address );

            // Make a UDP connection to the server
            try {
                FastUpdate = new mUDPClient( mPEndPoint );
                FastUpdate.onReceived += onFastUpdateReceive;
            } catch( Exception e ) {
                Console.WriteLine( "An error has occured with the connection to the server. Error { " );
                Console.WriteLine( e.ToString() );
                Console.WriteLine( "}" );
            }

            // Make a TCP connection to the server
            try {
                SlowUpdate = new mTCPClient( mPEndPoint );
                SlowUpdate.onReceived += onSlowUpdateReceive;
            } catch( Exception e ) {
                Console.WriteLine( "An error has occured with the connection to the server. Error { " );
                Console.WriteLine( e.ToString() );
                Console.WriteLine( "}" );
            }
        }

        void onFastUpdateReceive(object packet, EventArgs e) {
            onReceive?.Invoke(packet, e);
        }

        void onSlowUpdateReceive(object packet, EventArgs e ) {
            onReceive?.Invoke( packet, e );
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
