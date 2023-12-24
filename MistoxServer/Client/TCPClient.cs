using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MistoxServer;
using static System.Runtime.InteropServices.JavaScript.JSType;

//IP Range Updater

namespace MistoxServer.Client {
    public class mTCPClient {

        TcpClient Server;

        public event EventHandler onReceived;
        bool Alive;
        bool notified = false;

        public mTCPClient( IPEndPoint ServerAddress ) {
            Server = new TcpClient();
            Server.Connect( ServerAddress );
            Alive = true;
            Thread RThread = new Thread(ReceiveThread);
            RThread.Start();
        }

        void ReceiveThread() {
            byte[] StreamData = new byte[1024];
            while( Alive ) {
                try {
                    if (Server.Connected && !notified ) {
                        Console.WriteLine( "Connected to server" );
                        notified = true;
                    }
                    while( Alive ) {
                        int bytesRead = Server.GetStream().Read(StreamData, 0, StreamData.Length);
                        dynamic data = mSerialize.tReceive( StreamData.Sub( 0, bytesRead ) );
                        if( data != null ) {
                            onReceived?.Invoke( data, new EventArgs() );
                        }
                    }
                } catch( Exception e ) {
                    Console.WriteLine( "You have disconnected from the server for reason : " + e );
                    Alive = false;
                }
            }
        }

        public void Send<Packet>(Packet packet) {
            byte[] data = mSerialize.PacketSerialize( packet );
            Server.GetStream().Write( data, 0, data.Length );
        }

        public void Dispose() {
            Alive = false;
            Server.Close();
            Server = null;
        }
    }
}
