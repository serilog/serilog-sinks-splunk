﻿// Copyright 2016 Serilog Contributors
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

using Serilog.Formatting;
using System;

namespace Serilog.Sinks.Splunk
{

    /// <summary>
    /// A sink that logs to Splunk over UDP
    /// </summary>
    [Obsolete("Use SocketSink")]
    public class UdpSink : SocketSink
    {
        /// <inheritdoc/>
        public UdpSink(SplunkUdpSinkConnectionInfo connectionInfo, ITextFormatter formatter)
            : base(connectionInfo, formatter)
        {
        }

        /// <inheritdoc/>
        public UdpSink(SplunkUdpSinkConnectionInfo connectionInfo, IFormatProvider formatProvider = null, bool renderTemplate = true)
            : base(connectionInfo, formatProvider, renderTemplate)
        {
        }
    }
}

