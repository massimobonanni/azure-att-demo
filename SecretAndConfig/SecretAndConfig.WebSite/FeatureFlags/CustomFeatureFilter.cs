using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretAndConfig.WebSite.FeatureFlags
{
    public class CustomFeatureFilter : IFeatureFilter
    {
        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {


            return Task.FromResult(true);
        }
    }
}
