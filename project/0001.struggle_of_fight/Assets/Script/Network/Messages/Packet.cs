using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assets.Script.Network.Messages
{
    public enum MsgType
    {
        // login group
        login,

    }
    public class Packet
    {
        public const Type HeaderSizeType = typeof(UInt32);
        public const int HeaderSize = System.Runtime.InteropServices.Marshal.SizeOf(HeaderSizeType);

        public const Type MsgIDSizeType = typeof(UInt32);
        public const int MsgIDSize = System.Runtime.InteropServices.Marshal.SizeOf(MsgIDSizeType);

        public static IMessage CreatePacket(Stream s)
        {
            byte[] type = new byte[HeaderSize];
            s.Read(type, 0, HeaderSize);
            MsgType msgType = (MsgType)BitConverter.ToInt32(type, 0);
            switch(msgType)
            {
                case MsgType.:
            }
        }
    }
}
