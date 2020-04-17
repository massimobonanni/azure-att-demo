using SecretAndConfig.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static SecretConfigModel RetrieveModel(this IConfiguration configuration)
        {
            if (configuration == null)
                throw new NullReferenceException(nameof(configuration));

            var model = new SecretConfigModel();

            var section = configuration.GetSection("PublicSection");
            var modelSection = new SecretConfigSectionModel() { Name = "PublicSection" };
            foreach (var item in section.GetChildren())
            {
                modelSection.Values[item.Key] = item.Value;
            }
            model.Sections.Add(modelSection);

            section = configuration.GetSection("SecretSection");
            modelSection = new SecretConfigSectionModel() { Name = "SecretSection" };
            foreach (var item in section.GetChildren())
            {
                modelSection.Values[item.Key] = item.Value;
            }
            model.Sections.Add(modelSection);

            section = configuration.GetSection("ConnectionStrings");
            modelSection = new SecretConfigSectionModel() { Name = "ConnectionStrings" };
            foreach (var item in section.GetChildren())
            {
                modelSection.Values[item.Key] = item.Value;
            }
            model.Sections.Add(modelSection);

            return model;
        }
    }
}
