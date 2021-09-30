using k8s;
using k8s.Models;
using Kubedaemon.Models.Statuses;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kubedaemon.Platforms
{
    public class K8sPlatform : IPlatform
    {
        private readonly Kubernetes _client;

        public K8sPlatform(KubernetesClientConfiguration kubernetesClientConfiguration)
        {
            _client = new Kubernetes(kubernetesClientConfiguration);
        }

        public async Task<IEnumerable<string>> GetNamespacesAsync()
        {
            var response = await _client.ListNamespaceAsync();
            return response.Items.Select(o => o.Metadata.Name);
        }

        public async Task ApplyManifestAsync(string manifest, string ns = "default")
        {
            var typeMap = new Dictionary<String, Type>();
            typeMap.Add("v1/ConfigMap", typeof(V1ConfigMap));
            typeMap.Add("v1/Job", typeof(V1Job));
            typeMap.Add("v1/Deployment", typeof(V1Deployment));
            typeMap.Add("v1/Service", typeof(V1Service));
            typeMap.Add("v1/ServiceAccount", typeof(V1ServiceAccount));

            var objects = Yaml.LoadAllFromString(manifest, typeMap);
            foreach (var obj in objects)
            {
                if (obj is V1ConfigMap)
                {
                    var body = (V1ConfigMap)obj;
                    ApplyInternalLabels(body);
                    try
                    {
                        await _client.CreateNamespacedConfigMapAsync(body, ns);
                    }
                    catch (HttpOperationException ex)
                    {
                        if (ex.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                            throw;
                        await _client.ReplaceNamespacedConfigMapAsync(body, body.Name(), ns);
                    }
                }
                if (obj is V1Secret)
                {
                    var body = (V1Secret)obj;
                    ApplyInternalLabels(body);
                    try
                    {
                        await _client.CreateNamespacedSecretAsync(body, ns);
                    }
                    catch (HttpOperationException ex)
                    {
                        if (ex.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                            throw;
                        await _client.ReplaceNamespacedSecretAsync(body, body.Name(), ns);
                    }
                }
                else if (obj is V1Job)
                {
                    var body = (V1Job)obj;
                    ApplyInternalLabels(body);
                    try
                    {
                        await _client.CreateNamespacedJobAsync(body, ns);
                    }
                    catch (HttpOperationException ex)
                    {
                        if (ex.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                            throw;
                        await _client.ReplaceNamespacedJobAsync(body, body.Name(), ns);
                    }
                }
                else if (obj is V1Deployment)
                {
                    var body = (V1Deployment)obj;
                    ApplyInternalLabels(body);
                    try
                    {
                        await _client.CreateNamespacedDeploymentAsync(body, ns);
                    }
                    catch (HttpOperationException ex)
                    {
                        if (ex.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                            throw;
                        await _client.ReplaceNamespacedDeploymentAsync(body, body.Name(), ns);
                    }
                }
                else if (obj is V1Service)
                {
                    var body = (V1Service)obj;
                    ApplyInternalLabels(body);
                    try
                    {
                        await _client.CreateNamespacedServiceAsync(body, ns);
                    }
                    catch (HttpOperationException ex)
                    {
                        if (ex.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                            throw;
                        await _client.ReplaceNamespacedServiceAsync(body, body.Name(), ns);
                    }
                }
                else if (obj is V1ServiceAccount)
                {
                    var body = (V1ServiceAccount)obj;
                    ApplyInternalLabels(body);
                    try
                    {
                        await _client.CreateNamespacedServiceAccountAsync(body, ns);
                    }
                    catch (HttpOperationException ex)
                    {
                        if (ex.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                            throw;
                        await _client.ReplaceNamespacedServiceAccountAsync(body, body.Name(), ns);
                    }
                }
                else
                    throw new ArgumentException("Manifest type not supported");
            }
        }

        public async Task DeleteConfigMapAsync(string name, string ns = "default")
        {
            var result = await _client.DeleteNamespacedConfigMapAsync(name, ns);
            if (result.Status != "Success")
            {
                throw new Exception($"{result.Status}: {result.Message}");
            }
        }

        public async Task DeleteJobAsync(string name, string ns = "default")
        {
            var result = await _client.DeleteNamespacedJobAsync(name, ns);
            if (result.Status != "Success" && result.Status != null)
            {
                throw new Exception($"{result.Status}: {result.Message}");
            }
        }

        public async Task DeleteAppAsync(string name, string ns = "default")
        {
            var result = await _client.DeleteNamespacedDeploymentAsync(name, ns);
            if (result.Status != "Success" && result.Status != null)
            {
                throw new Exception($"{result.Status}: {result.Message}");
            }
        }

        public async Task<JobStatus> GetJobAsync(string name, string ns = "default")
        {
            var result = await _client.ReadNamespacedJobStatusAsync(name, ns);
            return MapJobStatus(result);
        }

        public async Task<IList<JobStatus>> GetJobsAsync(IDictionary<string, string> selectors = null, string ns = "default")
        {
            ApplyInternaSelectors(ref selectors);
            var result = await _client.ListNamespacedJobAsync(ns, labelSelector: string.Join(",", selectors.Select(o => $"{o.Key}={o.Value}")));
            return result.Items.Select(o => MapJobStatus(o)).ToList();
        }

        public async Task<IList<AppStatus>> GetAppsAsync(IDictionary<string, string> selectors = null, string ns = "default")
        {
            ApplyInternaSelectors(ref selectors);
            var result = await _client.ListNamespacedDeploymentAsync(ns, labelSelector: string.Join(",", selectors.Select(o => $"{o.Key}={o.Value}")));
            return result.Items.Select(o => MapAppStatus(o)).ToList();
        }

        public async Task<IList<ConfigStatus>> GetConfigsAsync(IDictionary<string, string> selectors = null, string ns = "default")
        {
            ApplyInternaSelectors(ref selectors);
            var result = await _client.ListNamespacedConfigMapAsync(ns, labelSelector: string.Join(",", selectors.Select(o => $"{o.Key}={o.Value}")));
            return result.Items.Select(o => MapConfigMapStatus(o)).ToList();
        }

        public async Task<AppStatus> GetAppAsync(string name, string ns = "default")
        {
            var result = await _client.ReadNamespacedDeploymentAsync(name, ns);
            return MapAppStatus(result);
        }

        public Task<ConfigStatus> GetConfigAsync(string name, string ns = "default")
        {
            throw new NotImplementedException();
        }

        private void ApplyInternaSelectors(ref IDictionary<string, string> selectors)
        {
            selectors = selectors.Set("kubedaemon/managed", "true");
        }

        private void ApplyInternalLabels(IMetadata<V1ObjectMeta> @object)
        {
            @object.SetLabel("kubedaemon/managed", "true");
        }

        private static JobStatus MapJobStatus(V1Job result)
        {
            var name = result.Name();
            var ns = result.Namespace();
            if (result.Status.Conditions == null)
            {
                if (result.Status.Active.HasValue && result.Status.Active > 0)
                    return new JobStatus(name, ns, JobStatus.Condition.Pending, result.CreationTimestamp(), result.Status.StartTime, result.Status.CompletionTime);
                if (result.Status.Succeeded.HasValue && result.Status.Succeeded > 0 && result.Status.Failed.HasValue && result.Status.Failed > 0)
                    return new JobStatus(name, ns, JobStatus.Condition.PartiallyComplete, result.CreationTimestamp(), result.Status.StartTime, result.Status.CompletionTime);
                if (result.Status.Succeeded.HasValue && result.Status.Succeeded > 0)
                    return new JobStatus(name, ns, JobStatus.Condition.Complete, result.CreationTimestamp(), result.Status.StartTime, result.Status.CompletionTime);
                return new JobStatus(name, ns, JobStatus.Condition.Failed, result.CreationTimestamp(), result.Status.StartTime, result.Status.CompletionTime);
            }
            return new JobStatus(name, ns, result.Status.Conditions.OrderByDescending(o => o.LastTransitionTime).Select(o => new JobStatus.Condition(o.Type, $"{o.Status} {o.Message}".Trim(), o.LastTransitionTime ?? DateTimeOffset.Now)).ToList(), result.CreationTimestamp(), result.Status.StartTime, result.Status.CompletionTime);
        }

        private static AppStatus MapAppStatus(V1Deployment result)
        {
            var name = result.Name();
            var ns = result.Namespace();
            if (result.Status.Conditions == null)
            {
                if (result.Status.UnavailableReplicas.HasValue && result.Status.UnavailableReplicas > 0)
                    return new AppStatus(name, ns, AppStatus.Condition.Pending, result.CreationTimestamp());
                if (result.Status.ReadyReplicas.HasValue && result.Status.ReadyReplicas > result.Status.Replicas)
                    return new AppStatus(name, ns, AppStatus.Condition.Available, result.CreationTimestamp());
                return new AppStatus(name, ns, AppStatus.Condition.Failed, result.CreationTimestamp());
            }
            return new AppStatus(name, ns, result.Status.Conditions.OrderByDescending(o => o.LastTransitionTime).Select(o => new AppStatus.Condition(o.Type, $"{o.Status} {o.Message}".Trim(), o.LastTransitionTime ?? DateTimeOffset.Now)).ToList(), result.CreationTimestamp());
        }

        private static ConfigStatus MapConfigMapStatus(V1ConfigMap result)
        {
            return new ConfigStatus(result.Name(), result.Namespace(), result.Data, result.CreationTimestamp());
        }
    }
}
