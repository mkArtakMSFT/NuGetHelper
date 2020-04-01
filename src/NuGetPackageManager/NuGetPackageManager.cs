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
            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

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
                this.logger.LogInformation($"Package {package} removed successfully");
            }
            else
            {
                this.logger.LogWarning($"Removal failed for package {package} with code {response.StatusCode}");
            }
        }

        public void Dispose()
        {
            this.client?.Dispose();
        }
    }
}
