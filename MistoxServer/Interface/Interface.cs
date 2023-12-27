using System;
using System.Net;
using System.Threading.Tasks;

namespace MistoxServer {
    public partial class mServer {
        public static IMistoxServer newServer(int port, ServerMode mode) {
            return new ServerInterface(port, mode);
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
                return new ClientInterface(ipAddress, Port);
            } else {
                Console.WriteLine("The server at " + ServerIPOrHostName + " doesn't exit or cannot be found");
                return null;
            }
        }  
    }

    public interface IMistoxServer {

        public event EventHandler onConnected;
        public event EventHandler onSlowReceive;
        public event EventHandler onFastReceive;
        public event EventHandler onDisconnected;

        public Task Send<Packet>(Packet data, SendType speed);

    }

    public enum SendType {
        SlowUpdate,
        FastUpdate
    }

    public enum ServerMode {
        Passive,
        Authoritative
    }

}
