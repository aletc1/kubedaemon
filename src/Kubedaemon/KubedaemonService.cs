using Kubedaemon.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kubedaemon
{
    public class KubedaemonService
    {
        private readonly IPlatform _platform;

        public KubedaemonService(IPlatform platform)
        {
            _platform = platform;
        }

        public async Task<IEnumerable<Template>> GetTemplatesAsync(IDictionary<string, string> selectors, string? ns = null)
        {
            var response = await _platform.GetConfigsAsync(selectors, ns);
            throw new NotImplementedException();
        }

        public Task RegisterTemplateAsync(Template template, string? ns = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTemplateAsync(string templateName, string? ns = null)
        {
            throw new NotImplementedException();
        }

        public Task DeployAsync(string name, string templateName, IDictionary<string, object> properties, string? ns = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> GetDeploymentsAsync(string? namePrefix = null, IDictionary<string, string> tags = null, string? ns = null)
        {
            throw new NotImplementedException();
        }
    }
}
