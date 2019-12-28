using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public static class LogConstants
    {
        public const string FullNameKey = "FullName";
        public const string NameKey = "Name";
        public const string CountKey = "Count";
        public const string SuccessesKey = "Successes";
        public const string FailuresKey = "Failures";
        public const string SuccessRateKey = "SuccessRate";
        public const string AverageDurationKey = "AvgDurationMs";
        public const string MaxDurationKey = "MaxDurationMs";
        public const string MinDurationKey = "MinDurationMs";
        public const string TimestampKey = "Timestamp";
        public const string InvocationIdKey = "InvocationId";
        public const string TriggerReasonKey = "TriggerReason";
        public const string StartTimeKey = "StartTime";
        public const string EndTimeKey = "EndTime";
        public const string DurationKey = "Duration";
        public const string SucceededKey = "Succeeded";
        public const string FormattedMessageKey = "FormattedMessage";
        public const string MessageKey = "Message";
        public const string CategoryNameKey = "Category";
        public const string HttpMethodKey = "HttpMethod";
        public const string CustomPropertyPrefix = "prop__";
        public const string ParameterPrefix = "param__";
        public const string OriginalFormatKey = "{OriginalFormat}";
        public const string LogLevelKey = "LogLevel";
        public const string EventIdKey = "EventId";
        public const string FunctionStartEvent = "FunctionStart";
        public const int MetricEventId = 1;
        public const string MetricValueKey = "Value";
        public const string FunctionExecutionTimeKey = "FunctionExecutionTimeMs";
    }
}
