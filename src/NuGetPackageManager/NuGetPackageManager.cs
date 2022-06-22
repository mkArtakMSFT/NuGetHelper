using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetPackageManager
{
    internal class NuGetPackageManager : IDisposable
    {
        private ILogger logger;
        private HttpClient client;

        public NuGetPackageManager(HttpClient client, ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.client.BaseAddress = new Uri("https://www.nuget.org/api/v2/package/", UriKind.Absolute);
        }

        public async Task<IEnumerable<Tuple<string, NuGetVersion>>> GetPackageVersionsAsync(string packageName, CancellationToken cancellationToken)
        {
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(
                packageName,
                cache,
                logger,
                cancellationToken);

            return versions.Select(version => Tuple.Create(packageName, version));
        }

        public async Task DeletePackageAsync(string packageName, NuGetVersion version, CancellationToken cancellationToken)
        {
            var package = $"{packageName} version {version}";
            this.logger.LogInformation($"Deleting package {package}");

            Uri deleteRequestUri = new Uri($"{packageName}/{version.ToString()}", UriKind.Relative);
            var response = await this.client.DeleteAsync(deleteRequestUri, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                this.logger.LogInformation($"Package {package} was removed successfully");
            }
            else
            {
                this.logger.LogWarning($"Removal failed for package {package} with code {response.StatusCode}");
            }
        }

        public async Task DeprecatePackagesAsync(string packageName, IEnumerable<string> versions, string deprecationMessage, CancellationToken cancellationToken)
        {
            var versionsString = String.Join(',', versions);
            logger.LogInformation($"Deprecating versions {versionsString} of package {packageName} ");
            var deprecationContext = new
            {
                versions = versions,
                isLegacy = true,
                hasCriticalBugs = false,
                isOther = true,
                //alternatePackageId = null,
                //alternatePackageVersion = context?.AlternatePackageVersion,
                message = deprecationMessage
            };

            var bodyJson = System.Text.Json.JsonSerializer.Serialize(deprecationContext);
            logger.LogInformation(bodyJson);
            var response = await this.client.PutAsync($"{packageName}/deprecations", new StringContent(bodyJson, System.Text.Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            this.client?.Dispose();
        }
    }
}
