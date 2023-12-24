using MistoxServer.Client;
using System;
using System.Net;

namespace MistoxServer {
    public class ClientInterface : IMistoxServer {

        mTCPClient SlowUpdate;
        mUDPClient FastUpdate;

        public event EventHandler onReceive;

        void onFastUpdateReceive(object packet, EventArgs e) {
            onReceive?.Invoke(packet, e);
        }

        void onSlowUpdateReceive(object packet, EventArgs e ) {
            onReceive?.Invoke( packet, e );
        }

        public ClientInterface( string ServerIP, int Port ) {
            IPHostEntry host = Dns.GetHostEntry( ServerIP );
            IPEndPoint server = new IPEndPoint(host.AddressList[0], Port);

            // Create the web network UDP client
            FastUpdate = new mUDPClient( server );
            FastUpdate.onReceived += onFastUpdateReceive;

            Console.WriteLine( "The client is initilized and trying to connect to the server at ip : " + ServerIP );

            // Make a connection to the server
            try {
                SlowUpdate = new mTCPClient( server );
                SlowUpdate.onReceived += onSlowUpdateReceive;
            }catch(Exception e) {
                Console.WriteLine("An error has occured with the connection to the server. Error { ");
                Console.WriteLine(e.ToString());
                Console.WriteLine("}");
            }
        }

        public void Send<Packet>(Packet data, SendType speed) {
            if (SendType.SlowUpdate == speed) {
                SlowUpdate.Send( data );
            } else {
                FastUpdate.Send( data );
            }
        }
    }
}
