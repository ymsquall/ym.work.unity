using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Assets.Script.Tools;
using Assets.Script.Network.Messages;

namespace Assets.Script.Network.TcpIP
{
    /// <summary>
    /// Builds HTTP messags from incoming bytes.
    /// </summary>
    public class TcpIPMessageBuilder : IMessageBuilder, IDisposable
    {
        private IBufferSliceStack mStack;
        private readonly IBufferSlice mBodySlice;
        //private readonly TcpIPHeaderParser mHeaderParser = new HttpHeaderParser();
        private readonly ConcurrentLinkedQueue<IMessage> mMessageQueue = new ConcurrentLinkedQueue<IMessage>();
        private int mBodyBytestLeft;
        private byte[] mHeaderLength = null;
        private Stream mBodyStream;
        private IMessage mNowMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBuilder" /> class.
        /// </summary>
        /// <param name="stack">Slices are used when processing incoming data.</param>
        /// <example>
        /// <code>
        /// var builder = new HttpMessageBuilder(new BufferSliceStack(100, 65535)); 
        /// </code>
        /// </example>
        public TcpIPMessageBuilder(IBufferSliceStack stack)
        {
            mStack = stack;
            //mHeaderParser.HeaderParsed += OnHeader;
            //mHeaderParser.Completed += OnHeaderComplete;
            //mHeaderParser.RequestLineParsed += OnRequestLine;
            mBodySlice = mStack.Pop();
            mBodyStream = new SliceStream(mBodySlice);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            mStack.Push(mBodySlice);
        }

        #endregion

        #region IMessageBuilder Members

        /// <summary>
        /// Append more bytes to your message building
        /// </summary>
        /// <param name="reader">Contains bytes which was received from the other end</param>
        /// <returns><c>true</c> if a complete message has been built; otherwise <c>false</c>.</returns>
        /// <remarks>You must handle/read everything which is available in the buffer</remarks>
        public bool Append(IBufferReader reader)
        {
            //mHeaderParser.Parse(reader);
            if (mBodyBytestLeft < 0)
            {

            }
            else if (mBodyBytestLeft == 0)
            {
                if (reader.RemainingLength < Packet.HeaderSize || null != mHeaderLength)
                {
                    if (null == mHeaderLength)
                    {
                        mHeaderLength = new byte[reader.RemainingLength];
                        reader.Read(mHeaderLength, 0, mHeaderLength.Length);
                    }
                    else
                    {
                        bool compleate = false;
                        int lastLength = Packet.HeaderSize - mHeaderLength.Length;
                        byte[] newHeaderLength = null;
                        if (reader.RemainingLength < lastLength)
                            newHeaderLength = new byte[reader.RemainingLength];
                        else
                        {
                            newHeaderLength = new byte[lastLength];
                            compleate = true;
                        }
                        reader.Read(newHeaderLength, 0, newHeaderLength.Length);
                        byte[] laseHLength = mHeaderLength.Clone() as byte[];
                        mHeaderLength = new byte[laseHLength.Length + newHeaderLength.Length];
                        int index = 0;
                        foreach(byte b in laseHLength)
                            mHeaderLength[index++] = b;
                        foreach (byte b in newHeaderLength)
                            mHeaderLength[index++] = b;
                        if(compleate)
                            mBodyBytestLeft = BitConverter.ToInt32(mHeaderLength, 0);
                    }
                }
                else
                {
                    byte[] length = new byte[Packet.HeaderSize];
                    reader.Read(length, 0, Packet.HeaderSize);
                    mBodyBytestLeft = BitConverter.ToInt32(length, 0);
                }
            }
            if(mBodyBytestLeft > 0)
            {
                var bytesToRead = Math.Min(reader.RemainingLength, mBodyBytestLeft);
                reader.CopyTo(mBodyStream, bytesToRead);
                mBodyBytestLeft -= bytesToRead;
                if (mBodyBytestLeft == 0)
                {
                    mBodyStream.Position = 0;
                    mMessageQueue.Enqueue(mNowMessage);
                    mNowMessage = null;
                }
                if (reader.RemainingLength > 0)
                {
                    //mHeaderParser.Parse(reader);
                }
            }
            return !mMessageQueue.IsEmpty;
        }

        /// <summary>
        /// Try to dequeue a message
        /// </summary>
        /// <param name="message">Message that the builder has built.</param>
        /// <returns><c>true</c> if a message was available; otherwise <c>false</c>.</returns>
        bool IMessageBuilder.TryDequeue(out object message)
        {
            IMessage msg;
            var result = mMessageQueue.TryDequeue(out msg);
            message = msg;
            return result;
        }

        /// <summary>
        /// Reset builder state
        /// </summary>
        public void Reset()
        {
            mBodyBytestLeft = 0;
            //mHeaderParser.Reset();
            IMessage message;
            while (mMessageQueue.TryDequeue(out message))
            {
                
            }
        }

        /// <summary>
        /// Try to dequeue a message
        /// </summary>
        /// <param name="message">Message that the builder has built.</param>
        /// <returns><c>true</c> if a message was available; otherwise <c>false</c>.</returns>
        public bool TryDequeue(out IMessage message)
        {
            IMessage msg;
            var result = mMessageQueue.TryDequeue(out msg);
            message = msg;
            return result;
        }


        #endregion

        private void OnRequestLine(object sender, RequestLineEventArgs e)
        {
            _message = new HttpRequest(e.Verb, e.Url, e.HttpVersion);
        }

        private void OnHeaderComplete(object sender, EventArgs e)
        {
            _bodyBytestLeft = _message.ContentLength;
            if (_message.ContentLength == 0)
            {
                _messages.Enqueue(_message);
                _message = null;
                return;
            }

            if (_message.ContentLength > _bodySlice.Count)
                _bodyStream = new FileStream(Path.GetTempFileName(), FileMode.Create);
            else
                _bodyStream = new SliceStream(_bodySlice);

            _message.Body = _bodyStream;
        }

        private void OnHeader(object sender, HeaderEventArgs e)
        {
            _message.AddHeader(e.Name, e.Value);
        }

    }
}