using System;
using System.Text;
using Griffin.Networking.Buffers;
using Assets.Script.Network.Messages;

namespace Assets.Script.Network.TcpIP
{
    /// <summary>
    /// Can serialize HTTP headers
    /// </summary>
    public class TcpIPHeaderSerializer
    {
        private readonly Encoding mEncoding = Encoding.UTF8;
        /// <summary>
        /// Send all headers to the client
        /// </summary>
        /// <param name="response">Response containing call headers.</param>
        /// <param name="writer">Writer to write everything to</param>
        public void SerializeResponse(IMessage response, IBufferWriter writer)
        {
            /*
            WriteString(writer, "{0} {1} {2}\r\n", response.ProtocolVersion, response.StatusCode,
                        response.StatusDescription);

            var contentType = response.ContentType ?? "text/html";
            if (response.ContentEncoding != null)
                contentType += ";charset=" + response.ContentEncoding.WebName;

            var length = response.ContentLength == 0 || response.Body != null
                             ? response.ContentLength
                             : response.Body.Length;

            // go through all property headers.
            WriteString(writer, "Content-Type: {0}\r\n", contentType);
            WriteString(writer, "Content-Length: {0}\r\n", length);
            //writer.WriteLine(response.KeepAlive ? "Connection: Keep-Alive" : "Connection: Close");

            if (response.Cookies != null && response.Cookies.Count > 0)
            {
                SerializeCookies(response, writer);
            }

            foreach (var header in response.Headers)
                WriteString(writer, "{0}: {1}\r\n", header.Name, header.Value);

            // header/body delimiter
            WriteString(writer, "\r\n");
            */
        }

        private void WriteString(IBufferWriter writer, string text, params object[] formatters)
        {
            var str = string.Format(text, formatters);
            var buffer = mEncoding.GetBytes(str);
            writer.Write(buffer, 0, buffer.Length);
        }

        private void SerializeCookies(IMessage response, IBufferWriter writer)
        {
            /*
            //Set-Cookie: <name>=<value>[; <name>=<value>][; expires=<date>][; domain=<domain_name>][; path=<some_path>][; secure][; httponly]
            foreach (var cookie in response.Cookies)
            {
                WriteString(writer, "Set-Cookie: {0}={1}", cookie.Name, cookie.Value ?? string.Empty);

                if (cookie.Expires > DateTime.MinValue)
                    WriteString(writer, ";expires={0}", cookie.Expires.ToString("R"));
                if (!string.IsNullOrEmpty(cookie.Path))
                    WriteString(writer, ";path={0}", cookie.Path);
                if (!string.IsNullOrEmpty(cookie.Domain))
                    this.WriteString(writer, ";domain={0}", cookie.Domain);

                WriteString(writer, "\r\n");
            }
            */
        }
    }
}