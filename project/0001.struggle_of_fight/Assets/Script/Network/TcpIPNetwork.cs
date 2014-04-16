using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Network
{
    class TcpIPNetwork
    {
        private TcpIPNetwork() { }

        static TcpIPNetwork mInstance = new TcpIPNetwork();
        public TcpIPNetwork Inst { get { return mInstance; } }
    }
}
