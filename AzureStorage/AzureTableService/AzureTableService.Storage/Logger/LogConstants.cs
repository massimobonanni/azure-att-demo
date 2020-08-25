﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AzureTableService.Storage.Logger
{
    internal static class LogConstants
    {
        public const string QueryDurationMetricName = "QueryDuration";
        public const string QueryCountItemMetricName = "QueryItemCount";
        public const string QueryFilterName="QueryFilterString";
        public const string QueryTableName = "QueryTableName";
    }
}
