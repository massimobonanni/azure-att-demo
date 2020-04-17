using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretAndConfig.WebSite.Models
{
    public class SecretConfigModel
    {
        public string Error { get; set; }
        public ICollection<SecretConfigSectionModel> Sections { get; set; } = new List<SecretConfigSectionModel>();
    }

    public class SecretConfigSectionModel
    {
        public string Name { get; set; }

        public IDictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}
