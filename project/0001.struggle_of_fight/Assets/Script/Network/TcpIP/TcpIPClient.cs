using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Griffin.Networking.Clients;
using Griffin.Networking.Messaging;

namespace Assets.Script.Network.TcpIP
{
    class TcpIPClient : MessagingClient
    {
        public TcpIPClient()
            : base(new TcpIPMessageFactory())
        {
            Received += new EventHandler<ReceivedMessageEventArgs>(OnTcpIPClientReceived);
            Disconnected += new EventHandler(OnTcpIPClientDisconnected);
        }

        void OnTcpIPClientReceived(object sender, ReceivedMessageEventArgs e)
        {
            //SliceStream stream = e.BufferReader as SliceStream;
        }
        void OnTcpIPClientDisconnected(object sender, EventArgs e)
        {

        }
    }
}
