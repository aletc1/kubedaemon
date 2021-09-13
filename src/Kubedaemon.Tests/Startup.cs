using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace Kubedaemon.Tests
{
    class Startup
    {
        private static IConfiguration _configuration;
        public static IConfiguration Configuration
        {
            get
            {
                return _configuration ??= new ConfigurationBuilder()
                    .SetBasePath(CurrentDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
#if DEBUG
                    .AddJsonFile($"appsettings.{ Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
#endif
                    .AddEnvironmentVariables()
                    .Build();
            }
        }

        public static string Server => Configuration["Server"];
        public static string ClientCertificateKeyData => Configuration["ClientCertificateKeyData"];
        public static string ClientCertificateData => Configuration["ClientCertificateData"];
        public static string CurrentDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
