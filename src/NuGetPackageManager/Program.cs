using NuGet.Common;
using NuGetPackageManager.Options;
using System.CommandLine;
using System.Threading.Tasks;

namespace NuGetPackageManager
{
    class Program
    {
        static ILogger logger = new CompositeLogger();

        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("NuGet package manager command-line app");
            var apiKeyOption = new Option<string>("--apiKey", "Provide PAT for the NuGet API account");
            rootCommand.AddGlobalOption(apiKeyOption);

            rootCommand.Add(BuildUnlistCommand());
            rootCommand.Add(BuildDeprecateCommand());

            await rootCommand.InvokeAsync(args);
        }

        private static Command BuildDeprecateCommand()
        {
            var result = new Command("deprecate", "Deprecate specific versions of a specified package");

            var packageIdOption = new Option<string>("packageId", "The name of the package to deprecate");
            result.AddOption(packageIdOption);

            var versionsOption = new Option<string[]>("versions", "Comma separated list of package versions to deprecate. Note, that all versions of the specified package will be deprecated.");
            result.AddOption(versionsOption);

            var messageOption = new Option<string>("message", "The deprecation message to show in NuGet.org for each of the versions to be deprecated.");
            result.AddOption(messageOption);

            AddForceOption(result);

            var undoOption = new Option<bool>("undo", "Calls the underlying NuGet APIs to undo deprecation of the specified package.");
            result.AddOption(undoOption);

            result.SetHandler(async (string apiKey, string packageId, string[] versions, string message, bool force, bool undo) =>
            {
                var deprecateOptions = new DeprecationOptions(apiKey, packageId, versions, force, undo);
                var handler = new CommandHandlers.DeprecateCommandHandler(deprecateOptions, logger);
                await handler.TryHandle(deprecateOptions);
            });

            return result;
        }

        private static Command BuildUnlistCommand()
        {
            var result = new Command("unlist", "Unlist all versions of the specified packages");

            var packageNamesOption = new Option<string[]>("packages", "A comman-separated list of package names to unlist");
            result.AddOption(packageNamesOption);

            AddForceOption(result);

            result.SetHandler(async (string apiKey, string[] packageNames, bool force) =>
            {
                var unlistOptions = new UnlistOptions(apiKey, packageNames, force);
                var handler = new CommandHandlers.UnlistCommandHandler(unlistOptions, logger);
                await handler.TryHandle(unlistOptions);
            });

            return result;
        }

        private static void AddForceOption(Command result)
        {
            var forceOption = new Option<bool>("force", "Calls the underlying NuGet APIs to deprecate the package. Without this parameter (default) the command executes in `dry-run` mode.");
            result.AddOption(forceOption);
        }
    }
}
