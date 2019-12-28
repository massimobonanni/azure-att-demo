using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AzureTableService.Core.Logger
{
    public class StopWatcher : IDisposable
    {
        public StopWatcher(ILogger logger, string metricName)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (string.IsNullOrWhiteSpace(metricName))
                throw new ArgumentException(nameof(metricName));

            this.logger = logger;
            this.MetricName = metricName;
            this.startTime = DateTime.Now;
            this.stopWatch = Stopwatch.StartNew();
        }

        private IDictionary<string, object> properties = new Dictionary<string, object>();
        private readonly ILogger logger;
        private readonly Stopwatch stopWatch;

        private DateTime startTime;
        private DateTime endTime;

        public string MetricName { get; private set; }
        public string Message { get; set; }

        public void AddProperty(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            this.properties[name] = value;
        }

        public void Dispose()
        {
            this.stopWatch.Stop();
            this.endTime = DateTime.Now;

            this.properties[LogConstants.DurationKey] = this.stopWatch.ElapsedMilliseconds;
            this.properties[LogConstants.StartTimeKey] = this.startTime;
            this.properties[LogConstants.EndTimeKey] = this.endTime;
            this.properties[LogConstants.FormattedMessageKey] = this.Message;
            this.properties[LogConstants.MessageKey] = this.Message;

            this.logger.LogMetric(this.MetricName, this.stopWatch.ElapsedMilliseconds, properties);
        }
    }
}
