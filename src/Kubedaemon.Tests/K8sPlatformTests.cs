using Xunit;
using Kubedaemon.Platforms;
using k8s;
using System.Threading.Tasks;

namespace Kubedaemon.Tests
{
    public class K8sPlatformTests
    {
        private KubernetesClientConfiguration Kubeconfig
        {
            get
            {
                var config = KubernetesClientConfiguration.BuildDefaultConfig();
                config.Host = Startup.Server;
                config.ClientCertificateData = Startup.ClientCertificateData;
                config.ClientCertificateKeyData = Startup.ClientCertificateKeyData;
                config.SkipTlsVerify = !string.IsNullOrWhiteSpace(Startup.ClientCertificateData);
                return config;
            }
        }

        [Fact]
        public async Task CanConnectToLocalK8s()
        {
            var platform = new K8sPlatform(Kubeconfig);
            var response = await platform.GetNamespacesAsync();
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task CanCreateAConfigMap()
        {
            var platform = new K8sPlatform(Kubeconfig);
            try
            {
                await platform.ApplyManifestAsync(@"
apiVersion: v1
kind: ConfigMap
metadata:
  name: kubedaemon-test
data:
  title: Hello world
");
            }
            finally
            {
                await platform.DeleteConfigMapAsync("kubedaemon-test");
            }
        }

        [Fact]
        public async Task CanCreateAndUpdateAConfigMap()
        {
            var platform = new K8sPlatform(Kubeconfig);
            try
            {
                await platform.ApplyManifestAsync(@"
apiVersion: v1
kind: ConfigMap
metadata:
  name: kubedaemon-test
data:
  title: Hello world
");
                await platform.ApplyManifestAsync(@"
apiVersion: v1
kind: ConfigMap
metadata:
  name: kubedaemon-test
data:
  title: Another Hello world
");
            }
            finally
            {
                await platform.DeleteConfigMapAsync("kubedaemon-test");
            }
        }

        [Fact]
        public async Task CanCreateAJob()
        {
            var platform = new K8sPlatform(Kubeconfig);
            try
            {
                await platform.ApplyManifestAsync(@"
apiVersion: batch/v1
kind: Job
metadata:
  name: kubedaemon-test-pi
spec:
  template:
    spec:
      containers:
      - name: pi
        image: perl
        command: [""perl"", ""-Mbignum=bpi"", ""-wle"", ""print bpi(2000)""]
      restartPolicy: Never
  backoffLimit: 4
");
                await Task.Delay(5000);
                var status = await platform.GetJobAsync("kubedaemon-test-pi");
                Assert.NotEmpty(status.Conditions);
                var jobsList = await platform.GetJobsAsync();
                Assert.NotEmpty(jobsList);
                Assert.Contains(jobsList, o => o.Name == "kubedaemon-test-pi");
            }
            finally
            {
                await platform.DeleteJobAsync("kubedaemon-test-pi");
            }
        }

        [Fact]
        public async Task CheckFailedCreationJobStatus()
        {
            var jobName = "kubedaemon-test-unknown-image";
            var platform = new K8sPlatform(Kubeconfig);
            try
            {
                await platform.ApplyManifestAsync(@"
apiVersion: batch/v1
kind: Job
metadata:
  name: kubedaemon-test-unknown-image
spec:
  template:
    spec:
      containers:
      - name: pi
        image: unknown-image-test
      restartPolicy: Never
  backoffLimit: 4
");

                var firstState =await platform.GetJobAsync(jobName);
                Assert.NotEmpty(firstState.Conditions);
                await Task.Delay(5000);
                var secondState = await platform.GetJobAsync(jobName);
                Assert.NotEmpty(firstState.Conditions);
            }
            finally
            {
                await platform.DeleteJobAsync(jobName);
            }
        }

        [Fact]
        public async Task CanCreateAnApp()
        {
            var platform = new K8sPlatform(Kubeconfig);
            try
            {
                await platform.ApplyManifestAsync(@"
apiVersion: apps/v1
kind: Deployment
metadata:
  name: kubedaemon-test-app
  labels:
    app: nginx
spec:
  replicas: 1
  selector:
    matchLabels:
      app: nginx
  template:
    metadata:
      labels:
        app: nginx
    spec:
      containers:
      - name: nginx
        image: nginx:1.7.9
        ports:
        - containerPort: 80
");
                var firstStatus = await platform.GetAppAsync("kubedaemon-test-app");
                Assert.NotEmpty(firstStatus.Conditions);
                await Task.Delay(5000);
                var lastStatus = await platform.GetAppAsync("kubedaemon-test-app");
                Assert.NotEmpty(lastStatus.Conditions);
                var appsList = await platform.GetAppsAsync();
                Assert.NotEmpty(appsList);
                Assert.Contains(appsList, o => o.Name == "kubedaemon-test-app");
            }
            finally
            {
                await platform.DeleteAppAsync("kubedaemon-test-app");
            }
        }
    }
}
