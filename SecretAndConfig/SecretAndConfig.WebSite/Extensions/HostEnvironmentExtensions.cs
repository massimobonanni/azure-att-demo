using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    public static class HostEnvironmentExtensions
    {
        public const string AppConfigurationEnvironment = "AppConfiguration";

        public static bool IsAppConfigurationEnvironment(this IHostEnvironment environment)
        {
           return environment.IsEnvironment(AppConfigurationEnvironment);
        }
    }
}
