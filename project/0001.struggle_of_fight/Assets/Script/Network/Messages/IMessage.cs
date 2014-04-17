using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assets.Script.Network.Messages
{
    public interface IMessage
    {
        Int32 MessageID { get; }
        Stream Body { get; set; }
    }
}
