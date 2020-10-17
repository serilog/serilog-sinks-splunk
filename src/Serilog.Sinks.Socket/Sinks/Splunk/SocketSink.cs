// Copyright 2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// A base sink that logs to Splunk via socket
    /// </summary>
    public class SocketSink : ILogEventSink, IDisposable
    {
        private static readonly object _renewLock = new object();
        private readonly SplunkSocketSinkConnectionInfo _connectionInfo;
        private readonly ITextFormatter _formatter;
        private Socket _socket;
        private bool _disposedValue = false;

        /// <summary>
        /// Creates an instance of the Splunk Socket Sink.
        /// </summary>
        /// <param name="connectionInfo">Connection info used for connecting against Splunk.</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="renderTemplate">If true, the message template will be rendered</param>
        public SocketSink(SplunkSocketSinkConnectionInfo connectionInfo, IFormatProvider formatProvider = null, bool renderTemplate = true)
            : this(connectionInfo, CreateDefaultFormatter(formatProvider, renderTemplate))
        {
        }

        /// <summary>
        /// Creates an instance of the Splunk Socket Sink.
        /// </summary>
        /// <param name="connectionInfo">Connection info used for connecting against Splunk.</param>
        /// <param name="formatter">Custom formatter to use if you e.g. do not want to use the JsonFormatter.</param>
        public SocketSink(SplunkSocketSinkConnectionInfo connectionInfo, ITextFormatter formatter)
        {
            _connectionInfo = connectionInfo;
            _formatter = formatter;
            _socket = _connectionInfo.TryOpenSocket();
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _socket.SecureDispose();
                    _socket = null;
                }

                _disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc/>
        public virtual void Emit(LogEvent logEvent)
        {
            byte[] data = Convert(logEvent);
            try
            {
                _socket.Send(data);
            }
            catch (ObjectDisposedException)
            {
                // Socket has been renewed, try to resent
                _socket.Send(data);
            }
            catch (SocketException)
            {
                // Try to reconnect and resent
                RenewSocket();
                _socket.Send(data);
            }
        }

        private void RenewSocket()
        {
            lock (_renewLock)
            {
                var brokenSocket = _socket;
                _socket = _connectionInfo.TryOpenSocket();
                brokenSocket.SecureDispose();
            }
        }

        private byte[] Convert(LogEvent logEvent)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
                _formatter.Format(logEvent, sw);
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static SplunkJsonFormatter CreateDefaultFormatter(IFormatProvider formatProvider, bool renderTemplate)
        {
            return new SplunkJsonFormatter(renderTemplate, formatProvider);
        }
    }
}

