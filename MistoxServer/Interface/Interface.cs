using System;
using System.Net;

namespace MistoxServer {
    public partial class mServer {
        public static IMistoxServer newServer(int port) {
            Console.WriteLine("Initilizing server");
            return new ServerInterface(port);
        }

        public static IMistoxServer newClient(string ServerIPOrHostName, int Port) {
            int index = 0;
            IPAddress[] remoteAddress = Dns.GetHostAddresses(ServerIPOrHostName);
            for( int i=0; i< remoteAddress.Length; i++ ) {
                if( remoteAddress[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || remoteAddress [i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ) {
                    index = i;
                }
            }
            string ipAddress = remoteAddress[index].ToString();
            if ( remoteAddress.Length > 0) {
                Console.WriteLine("Initilizing connection to server at IP : " + ipAddress);
                return new ClientInterface(ipAddress, Port);
            } else {
                Console.WriteLine("The server at " + ServerIPOrHostName + " doesn't exit or cannot be found");
                return null;
            }
        }  
    }

    public interface IMistoxServer {

        public event EventHandler onReceive;
        public void Send<Packet>(Packet data, SendType speed);

    }

    public enum SendType {
        SlowUpdate,
        FastUpdate
    }

}
