using CommandLine;
using NuGet.Common;
using NuGetPackageManager.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetPackageManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new CompositeLogger();

            //await RunUnlistingAsync(args, logger);

            await RunDeprecationAsync(args, logger);
        }

        private static async Task RunUnlistingAsync(string[] args, CompositeLogger logger)
        {
            var parsedOptions = Parser.Default.ParseArguments<DeleteOptions>(args);
            await parsedOptions.MapResult(async options =>
            {
                using NuGetPackageManager pkgManager = CreatePackageManager(options, logger);
                foreach (var package in options.PackageNames.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    var versions = await pkgManager.GetPackageVersionsAsync(package, CancellationToken.None);
                    foreach (var version in versions)
                    {
                        if (options.Force)
                            await pkgManager.DeletePackageAsync(version.Item1, version.Item2, CancellationToken.None);
                        else
                            logger.LogInformation($"Package {package} version {version} will be removed");
                    }
                }
            },
               errors =>
               {
                   HandleErrors(errors, logger);
                   return Task.FromResult(0);
               });
        }

        private static async Task RunDeprecationAsync(string[] args, CompositeLogger logger)
        {
            var deprecationOptions = Parser.Default.ParseArguments<DeprecationOptions>(args);
            await deprecationOptions.MapResult(
                async options =>
                {
                    using NuGetPackageManager pkgManager = CreatePackageManager(options, logger);
                    await pkgManager.DeprecatePackagesAsync(options.PackageName, options.PackageVersions, options.Message, CancellationToken.None);
                },
               errors =>
               {
                   HandleErrors(errors, logger);
                   return Task.FromResult(0);
               });
        }

        private static void HandleErrors(IEnumerable<Error> errors, ILogger logger)
        {
            foreach (var error in errors)
            {
                logger.LogError(error.ToString());
            }
        }

        private static NuGetPackageManager CreatePackageManager(INugetApiOptions options, ILogger logger)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-NuGet-ApiKey", options.ApiKey);

            using NuGetPackageManager pkgManager = new NuGetPackageManager(client, logger);
            return pkgManager;
        }
    }
}
