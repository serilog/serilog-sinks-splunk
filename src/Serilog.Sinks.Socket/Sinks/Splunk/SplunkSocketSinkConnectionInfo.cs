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

using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Defines connection info used to connect against Splunk
    /// using socket.
    /// </summary>
    public class SplunkSocketSinkConnectionInfo
    {
        private readonly Action<Socket> _connect;
        private readonly RetryPolicy _policy;

        /// <summary>
        /// Gets or sets the type of the Socket.
        /// </summary>
        protected virtual SocketType SocketType { get; set; }

        /// <summary>
        /// Gets or sets the protocol type of the Socket.
        /// </summary>
        protected virtual ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="SplunkSocketSinkConnectionInfo"/> used
        /// for defining connection info for connecting using socket against Splunk.
        /// </summary>
        /// <param name="host">Splunk host.</param>
        /// <param name="port">Splunk port.</param>
        public SplunkSocketSinkConnectionInfo(string host, int port)
            : this((socket) => socket.Connect(host, port))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="SplunkSocketSinkConnectionInfo"/> used
        /// for defining connection info for connecting using socket against Splunk.
        /// </summary>
        /// <param name="address">Splunk address.</param>
        /// <param name="port">Splunk port.</param>
        public SplunkSocketSinkConnectionInfo(IPAddress address, int port)
            : this((socket) => socket.Connect(address, port))
        {
        }

        private SplunkSocketSinkConnectionInfo(Action<Socket> connect)
        {
            _connect = connect;
            _policy = Policy.Handle<SocketException>().Retry(3);
        }

        internal virtual Socket TryOpenSocket()
        {
            return _policy.Execute(() =>
            {
                Socket socket = null;
                try
                {
                    socket = new Socket(SocketType, ProtocolType);
                    _connect(socket);
                    return socket;
                }
                catch
                {
                    socket.SecureDispose();
                    throw;
                }
            });
        }
    }
}

