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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.Splunk
{
    /// <summary>
    /// Renders log events into a default JSON format for consumption by Splunk.
    /// </summary>
    public class SplunkJsonFormatter : ITextFormatter
    {
        static readonly JsonValueFormatter ValueFormatter = new JsonValueFormatter();

        readonly bool _renderTemplate;
        readonly IFormatProvider _formatProvider;
        readonly string _suffix;

        /// <summary>
        /// Construct a <see cref="SplunkJsonFormatter"/>.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the template used will be rendered and written to the output as a property named MessageTemplate</param>
        public SplunkJsonFormatter(
            bool renderTemplate,
            IFormatProvider formatProvider)
            : this(renderTemplate, formatProvider, null, null, null, null)
        {
        }

        /// <summary>
        /// Construct a <see cref="SplunkJsonFormatter"/>.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the template used will be rendered and written to the output as a property named MessageTemplate</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        public SplunkJsonFormatter(
            bool renderTemplate,
            IFormatProvider formatProvider,
            string source,
            string sourceType,
            string host,
            string index)
        {
            _renderTemplate = renderTemplate;
            _formatProvider = formatProvider;

            var suffixWriter = new StringWriter();
            suffixWriter.Write("}"); // Terminates "event"

            if (!string.IsNullOrWhiteSpace(source))
            {
                suffixWriter.Write(",\"source\":");
                JsonValueFormatter.WriteQuotedJsonString(source, suffixWriter);
            }

            if (!string.IsNullOrWhiteSpace(sourceType))
            {
                suffixWriter.Write(",\"sourcetype\":");
                JsonValueFormatter.WriteQuotedJsonString(sourceType, suffixWriter);
            }

            if (!string.IsNullOrWhiteSpace(host))
            {
                suffixWriter.Write(",\"host\":");
                JsonValueFormatter.WriteQuotedJsonString(host, suffixWriter);
            }

            if (!string.IsNullOrWhiteSpace(index))
            {
                suffixWriter.Write(",\"index\":");
                JsonValueFormatter.WriteQuotedJsonString(index, suffixWriter);
            }
            suffixWriter.Write('}'); // Terminates the payload
            _suffix = suffixWriter.ToString();
        }

        /// <summary>
        /// Construct a <see cref="SplunkJsonFormatter"/>.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="renderTemplate">If true, the template used will be rendered and written to the output as a property named MessageTemplate</param>
        /// <param name="index">The Splunk index to log to</param>
        /// <param name="source">The source of the event</param>
        /// <param name="sourceType">The source type of the event</param>
        /// <param name="host">The host of the event</param>
        public SplunkJsonFormatter(
            bool renderTemplate,
            IFormatProvider formatProvider,
            string source,
            string sourceType,
            string host,
            string index,
            CustomFields customFields)
        {
            _renderTemplate = renderTemplate;
            _formatProvider = formatProvider;

            var suffixWriter = new StringWriter();
            suffixWriter.Write("}"); // Terminates "event"

            if (!string.IsNullOrWhiteSpace(source))
            {
                suffixWriter.Write(",\"source\":");
                JsonValueFormatter.WriteQuotedJsonString(source, suffixWriter);
            }

            if (!string.IsNullOrWhiteSpace(sourceType))
            {
                suffixWriter.Write(",\"sourcetype\":");
                JsonValueFormatter.WriteQuotedJsonString(sourceType, suffixWriter);
            }

            if (!string.IsNullOrWhiteSpace(host))
            {
                suffixWriter.Write(",\"host\":");
                JsonValueFormatter.WriteQuotedJsonString(host, suffixWriter);
            }

            if (!string.IsNullOrWhiteSpace(index))
            {
                suffixWriter.Write(",\"index\":");
                JsonValueFormatter.WriteQuotedJsonString(index, suffixWriter);
            }
            if (customFields != null)
            {
                // "fields": {"club":"glee", "wins",["regionals","nationals"]}
                suffixWriter.Write(",\"fields\": {");
                var lastFieldIndex = customFields.CustomFieldList.Count;
                foreach (var customField in customFields.CustomFieldList)
                {
                    if (customField.ValueList.Count == 1)
                    {
                        //only one value e.g "club":"glee",       
                        suffixWriter.Write($"\"{customField.Name}\":");
                        suffixWriter.Write($"\"{customField.ValueList[0]}\"");
                    }
                    else
                    {
                        //array of values e.g "wins",["regionals","nationals"]
                        suffixWriter.Write($"\"{customField.Name}\":[");
                        var lastArrIndex = customField.ValueList.Count;
                        foreach (var cf in customField.ValueList)
                        {
                            suffixWriter.Write($"\"{cf}\"");
                            //Different behaviour if it is the last one
                            suffixWriter.Write(--lastArrIndex > 0 ? "," : "]");
                        }
                    }
                    suffixWriter.Write(--lastFieldIndex > 0 ? "," : "}");
                }
               
            }
            suffixWriter.Write('}'); // Terminates the payload
            _suffix = suffixWriter.ToString();
        }

        /// <inheritdoc/>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.Write("{\"time\":\"");
            output.Write(logEvent.Timestamp.ToEpoch().ToString(CultureInfo.InvariantCulture));
            output.Write("\",\"event\":{\"Level\":\"");
            output.Write(logEvent.Level);
            output.Write('"');

            if (_renderTemplate)
            {
                output.Write(",\"MessageTemplate\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);
            }

            output.Write(",\"RenderedMessage\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.RenderMessage(_formatProvider), output);

            if (logEvent.Exception != null)
            {
                output.Write(",\"Exception\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
            }

            if (logEvent.Properties.Count != 0)
                WriteProperties(logEvent.Properties, output);

            output.WriteLine(_suffix);
        }

        static void WriteProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
        {
            output.Write(",\"Properties\":{");

            var precedingDelimiter = "";
            foreach (var property in properties)
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                ValueFormatter.Format(property.Value, output);
            }

            output.Write('}');
        }
    }
}
