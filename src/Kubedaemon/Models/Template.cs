using System.Collections.Generic;

namespace Kubedaemon.Models
{
    public class Template
    {
        public string Name { get; private set; }
        public TemplateType Type { get; private set; }
        public IDictionary<Platform, string> Spec { get; private set; }

        public Template(string name, TemplateType type, IDictionary<Platform, string> spec)
        {
            Name = name;
            Type = type;
            Spec = spec;
        }
    }
}
