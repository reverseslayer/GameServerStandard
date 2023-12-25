using System;
using System.Net.Sockets;
using System.Net;
using MistoxServer.Client;

namespace MistoxServer.Server {

    public class Connection {
        public int ID { get; set; }
        public string UserName { get; set; }
        public mTCPServer slowClient { get; set; }
        public IPEndPoint fastClient { get; set; }
    }

}
