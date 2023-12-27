using System.Net;

namespace MistoxServer.Server {

    public class Connection {
        public mTCPServer slowClient { get; set; }
        public IPEndPoint fastClient { get; set; }
    }

}
