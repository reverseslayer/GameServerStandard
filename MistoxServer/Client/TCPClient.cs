using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

//IP Range Updater

namespace MistoxServer.Client {
    public class mTCPClient {

        TcpClient Server;

        public event EventHandler onReceived;
        bool Alive;

        public mTCPClient( IPEndPoint ServerAddress ) {
            Server = new TcpClient( ServerAddress );
            Alive = true;
            Thread RThread = new Thread(ReceiveThread);
            RThread.Start();
        }

        void ReceiveThread() {
            while( Alive ) {
                try {
                    byte[] BufferedData = new byte[0];
                    while( Alive ) {
                        byte[] StreamData = new byte[1024];
                        int bytesRead = Server.GetStream().Read(StreamData, 0, StreamData.Length);
                        BufferedData.Join( StreamData );
                        if( BufferedData.Length > 4 ) {
                            int dataLength = BitConverter.ToInt32( BufferedData.Sub(0, 4) );
                            if( BufferedData.Length >= dataLength + 4 ) {
                                onReceived.Invoke( mSerialize.PacketDeserialize( BufferedData.Sub( 0, dataLength + 4 ) ), new EventArgs() );
                                BufferedData.Sub( dataLength + 4, BufferedData.Length - (dataLength + 4) );
                            }
                        }
                    }
                } catch( Exception e ) {
                    Console.WriteLine( "A user has disconnected for reason : " + e.ToString() );
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
