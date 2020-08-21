using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretAndConfig.WebSite.Services
{
    public class FakeFeatureManager : IFeatureManager
    {
        public IAsyncEnumerable<string> GetFeatureNamesAsync()
        {
            return null;
        }

        public Task<bool> IsEnabledAsync(string feature)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
        {
            return Task.FromResult(true);
        }
    }
}
