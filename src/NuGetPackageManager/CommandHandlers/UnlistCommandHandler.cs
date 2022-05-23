using NuGet.Common;
using NuGetPackageManager.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetPackageManager.CommandHandlers
{
    internal class UnlistCommandHandler : NugetCommandHandlerBase<UnlistOptions>
    {
        public UnlistCommandHandler(INugetApiOptions options, ILogger logger) : base(options, logger)
        {
        }

        protected override async Task Handle(NuGetPackageManager packageManager, UnlistOptions options)
        {
            foreach (var package in options.PackageNames.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var versions = await packageManager.GetPackageVersionsAsync(package, CancellationToken.None);
                foreach (var version in versions)
                {
                    if (options.Force)
                        await packageManager.DeletePackageAsync(version.Item1, version.Item2, CancellationToken.None);
                    else
                        Logger.LogInformation($"Package {package} version {version} will be removed");
                }
            }
        }
    }
}
