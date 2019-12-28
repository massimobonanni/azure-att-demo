using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static void LogMetric(this ILogger logger, string name, double value, IDictionary<string, object> properties = null)
        {
            IDictionary<string, object> state = properties == null ? new Dictionary<string, object>() : new Dictionary<string, object>(properties);

            state[LogConstants.NameKey] = name;
            state[LogConstants.MetricValueKey] = value;

            IDictionary<string, object> payload = new ReadOnlyDictionary<string, object>(state);
            logger?.Log(LogLevel.Information, LogConstants.MetricEventId, payload, null, (s, e) => null);
        }
    }
}
