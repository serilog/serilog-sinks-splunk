// Copyright 2018 Serilog Contributors
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

using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Splunk;
using System;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.SplunkViaEventCollector() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationSplunkExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via TCP.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="connectionInfo"></param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the message template is rendered</param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaTcp(this LoggerSinkConfiguration loggerConfiguration, SplunkTcpSinkConnectionInfo connectionInfo,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum, IFormatProvider formatProvider = null, bool renderTemplate = true)
        {
            var sink = new SocketSink(connectionInfo, formatProvider, renderTemplate);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that writes log events as to a Splunk instance via TCP.
        /// </summary>
        /// <param name="loggerConfiguration">The logger config</param>
        /// <param name="connectionInfo"></param>
        /// <param name="formatter">Custom formatter to use if you e.g. do not want to use the JsonFormatter.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkViaTcp(this LoggerSinkConfiguration loggerConfiguration, SplunkTcpSinkConnectionInfo connectionInfo,
            ITextFormatter formatter, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var sink = new SocketSink(connectionInfo, formatter);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}