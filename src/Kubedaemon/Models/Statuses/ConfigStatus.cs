using System;
using System.Collections.Generic;

namespace Kubedaemon.Models.Statuses
{
    public class ConfigStatus: Status
    {
        public IDictionary<string, string> Content { get; private set; }

        public ConfigStatus(string name, string ns, IDictionary<string, string> content, DateTimeOffset? creationTime): base(name, ns, (Status.Condition)null, creationTime)
        {
            Content = content;
        }
    }
}
