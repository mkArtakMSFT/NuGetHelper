using NuGet.Common;
using NuGetPackageManager.Options;
using System;
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
            try
            {
                await packageManager.DeprecatePackagesAsync(options.PackageId, options.Versions, options.Message, CancellationToken.None);
                this.Logger.LogInformation($"Successfully deprecated versions {string.Join(',', options.Versions)} of package {options.PackageId}");
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Failed to deprecate versions {string.Join(',', options.Versions)} of package {options.PackageId}. Reason: {ex.Message}");
                throw;
            }
        }
    }
}
