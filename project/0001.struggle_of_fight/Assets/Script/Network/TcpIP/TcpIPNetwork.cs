using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Griffin.Networking;
using Griffin.Networking.Clients;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;

namespace Assets.Script.Network.TcpIP
{
    class TcpIPNetwork
    {
        private TcpIPNetwork()
        {
            mLocalClient = new TcpIPClient();
        }
        static TcpIPNetwork mInstance = new TcpIPNetwork();
        public TcpIPNetwork Inst { get { return mInstance; } }

        private TcpIPClient mLocalClient;

        public bool ConnectLoginServer(string address, int port)
        {
            IPHostEntry IPHost = Dns.GetHostEntry(address);
            IPEndPoint ep = new IPEndPoint(IPHost.AddressList[0], port);
            mLocalClient.Connect(ep);

            return true;
        }

    }
}
