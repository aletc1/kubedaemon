using Kubedaemon.Models;
using Kubedaemon.Models.Statuses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kubedaemon
{
    public interface IPlatform
    {
        Task<IEnumerable<string>> GetNamespacesAsync();
        Task ApplyManifestAsync(string manifest, string ns = "default");
        Task<IList<AppStatus>> GetAppsAsync(IDictionary<string, string> selectors = null, string ns = "default");
        Task<AppStatus> GetAppAsync(string name, string ns = "default");
        Task<IList<JobStatus>> GetJobsAsync(IDictionary<string, string> selectors = null, string ns = "default");
        Task<ConfigStatus> GetConfigAsync(string name, string ns = "default");
        Task<IList<ConfigStatus>> GetConfigsAsync(IDictionary<string, string> selectors = null, string ns = "default");
        Task<JobStatus> GetJobAsync(string name, string ns = "default");
        Task DeleteConfigMapAsync(string name, string ns = "default");
        Task DeleteJobAsync(string name, string ns = "default");
        Task DeleteAppAsync(string name, string ns = "default");

    }
}
