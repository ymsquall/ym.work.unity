using System;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Assets.Script.Network.Messages;

namespace Assets.Script.Network.TcpIP
{
    /// <summary>
    /// Takes HTTP messages and serialize them into bytes.
    /// </summary>
    public class TcpIPMessageSerializer : IMessageSerializer
    {
        #region IMessageSerializer Members

        /// <summary>
        /// Serialize a message into something that can be transported over the socket.
        /// </summary>
        /// <param name="message">Message to serialize</param>
        /// <param name="writer">Buffer used to store the message</param>
        public void Serialize(object message, IBufferWriter writer)
        {
            var msg = (IMessage) message;
            var serializer = new TcpIPHeaderSerializer();
            serializer.SerializeResponse(msg, writer);
            writer.Copy(msg.Body);
        }

        #endregion
    }
}