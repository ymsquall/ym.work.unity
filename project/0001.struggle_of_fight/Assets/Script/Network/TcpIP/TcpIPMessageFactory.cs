using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;

namespace Assets.Script.Network.TcpIP
{
    /// <summary>
    /// Used to convert byte[] arrays to/from binary messages.
    /// </summary>
    public class TcpIPMessageFactory : IMessageFormatterFactory
    {
        /// <param name="stack">Used to provide <c>byte[]</c> buffers to the workers..</param>
        private readonly IBufferSliceStack mStack;
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpIPMessageFactory"/> class.
        /// </summary>
        public TcpIPMessageFactory()
        {
            mStack = new BufferSliceStack(1, 65535);
        }
        #region IMessageFormatterFactory Members

        /// <summary>
        /// Create a new serializer (used to convert messages to byte buffers)
        /// </summary>
        /// <returns>Created serializer</returns>
        public IMessageSerializer CreateSerializer()
        {
            return new TcpIPMessageSerializer();
        }

        /// <summary>
        /// Create a message builder (used to compose messages from byte buffers)
        /// </summary>
        /// <returns></returns>
        public IMessageBuilder CreateBuilder()
        {
            return new TcpIPMessageBuilder(mStack);
        }

        #endregion
    }
}