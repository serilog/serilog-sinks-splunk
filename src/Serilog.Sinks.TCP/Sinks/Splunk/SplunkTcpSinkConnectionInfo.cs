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

using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Defines connection info used to connect against Splunk
    /// using TCP.
    /// </summary>
    public class SplunkTcpSinkConnectionInfo : SplunkSocketSinkConnectionInfo
    {
        /// <inheritdoc/>
        protected override SocketType SocketType => SocketType.Stream;

        /// <inheritdoc/>
        protected override ProtocolType ProtocolType => ProtocolType.Tcp;

        /// <inheritdoc/>
        public SplunkTcpSinkConnectionInfo(string host, int port)
            : base(host, port)
        {
        }

        /// <inheritdoc/>
        public SplunkTcpSinkConnectionInfo(IPAddress address, int port)
            : base(address, port)
        {
        }
    }
}
