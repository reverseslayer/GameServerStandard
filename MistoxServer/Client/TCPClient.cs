using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MistoxServer;
using static System.Runtime.InteropServices.JavaScript.JSType;

//IP Range Updater

namespace MistoxServer.Client {
    public class mTCPClient : IDisposable {

        TcpClient Server;

        public event EventHandler onReceived;
        public bool Alive;
        bool notified = false;

        public mTCPClient( IPEndPoint ServerAddress ) {
            Server = new TcpClient();
            Server.NoDelay = true;
            Server.Connect( ServerAddress );
            Alive = true;
            Thread RThread = new Thread(ReceiveThread);
            RThread.Start();
        }

        void ReceiveThread() {
            using( NetworkStream ns = Server.GetStream() ) {
                try {
                    while( Alive ) {
                        if( Server.Connected && !notified ) {
                            Console.WriteLine( "Connected to server" );
                            notified = true;
                        } else if( !Server.Connected ) {
                            Console.WriteLine( "Disconnected from server" );
                            Alive = false;
                        }
                        byte[] StreamData = new byte[1024];
                        int bytesRead = ns.Read(StreamData, 0, StreamData.Length);
                        if( bytesRead > 0 ) {
                            dynamic data = mSerialize.tReceive( StreamData.Sub( 0, bytesRead ) );
                            if( data != null ) {
                                onReceived?.Invoke( data, new EventArgs() );
                            }
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
