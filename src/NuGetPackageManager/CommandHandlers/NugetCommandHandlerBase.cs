using NuGet.Common;
using NuGetPackageManager.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NuGetPackageManager.CommandHandlers
{
    internal abstract class NugetCommandHandlerBase<T>
    {
        private readonly INugetApiOptions options;
        protected ILogger Logger { get; }

        public NugetCommandHandlerBase(INugetApiOptions options, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(options);

            this.Logger = logger;
            this.options = options;
        }

        public async Task<bool> TryHandle(T args)
        {
            try
            {
                using var client = CreatePackageManager();
                await Handle(client, args);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }
        }

        protected abstract Task Handle(NuGetPackageManager packageManager, T args);

        private NuGetPackageManager CreatePackageManager()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-NuGet-ApiKey", this.options.ApiKey);

            return new NuGetPackageManager(client, Logger);
        }
    }
}
