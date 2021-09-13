using System.Collections.Generic;

namespace Kubedaemon
{
    public static class Extensions
    {
        public static IDictionary<string, string> Set(this IDictionary<string, string> selectors, string key, string value)
        {
            if (selectors == null)
            {
                selectors = new Dictionary<string, string>();   
            }
            if (!selectors.TryAdd(key, value))
            {
                selectors[key] = value;
            }
            
            return selectors;
        }
    }
}
