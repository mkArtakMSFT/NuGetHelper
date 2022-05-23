using NuGet.Common;
using NuGetPackageManager.Options;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetPackageManager.CommandHandlers
{
    internal class DeprecateCommandHandler : NugetCommandHandlerBase<DeprecationOptions>
    {
        public DeprecateCommandHandler(INugetApiOptions options, ILogger logger) : base(options, logger)
        {
        }

        protected override async Task Handle(NuGetPackageManager packageManager, DeprecationOptions options)
        {
            await packageManager.DeprecatePackagesAsync(options.PackageId, options.Versions, options.Message, CancellationToken.None);
        }
    }
}
