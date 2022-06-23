using NuGet.Common;
using NuGetPackageManager.Options;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetPackageManager
{
    class Program
    {
        private const int DelayInMinutes = 1;
        static ILogger logger = new CompositeLogger();

        static async Task Main(string[] args)
        {
            Console.WriteLine($"Delaying {DelayInMinutes} minutes before execution");
            await Task.Delay((int)System.TimeSpan.FromMinutes(DelayInMinutes).TotalMilliseconds);

            var rootCommand = new RootCommand("NuGet package manager command-line app");
            var apiKeyOption = new Option<string>("--apiKeys", "Provide comma-separated list of PATs for the NuGet API account");
            //rootCommand.AddGlobalOption(apiKeyOption);

            rootCommand.Add(BuildUnlistCommand());
            rootCommand.Add(BuildDeprecateCommand());

            await rootCommand.InvokeAsync(args);
        }

        private static Command BuildDeprecateCommand()
        {
            var apiKeysOption = new Option<string>("--apiKeys", "Provide comma-separated list of PATs for the NuGet API account");
            var packageIdOption = new Option<string>("--packageId", "The name of the package to deprecate");
            var versionsOptions = new Option<string>("--versions", "Comma separated list of package versions to deprecate. Note, that all versions of the specified package will be deprecated.");
            var messageOption = new Option<string>("--message", "The deprecation message to show in NuGet.org for each of the versions to be deprecated.");

            var result = new Command("deprecate", "Deprecate specific versions of a specified package")
            {
                apiKeysOption,
                packageIdOption,
                versionsOptions,
                messageOption
            };

            //AddForceOption(result);

            //var undoOption = new Option<bool>("--undo", "Calls the underlying NuGet APIs to undo deprecation of the specified package.");
            //result.AddOption(undoOption);

            result.SetHandler(async (string apiKeys, string packageId, string versions, string message/*, bool force, bool undo*/) =>
            {
                var keys = apiKeys.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var key in keys)
                {
                    var deprecateOptions = new DeprecationOptions(key.Trim(), packageId, versions.Split(',', StringSplitOptions.RemoveEmptyEntries), message, true, false /*force, undo*/);
                    var handler = new CommandHandlers.DeprecateCommandHandler(deprecateOptions, logger);
                    if (await handler.TryHandle(deprecateOptions))
                        break;
                }
            }, apiKeysOption, packageIdOption, versionsOptions, messageOption);

            return result;
        }

        private static Command BuildUnlistCommand()
        {
            var result = new Command("unlist", "Unlist all versions of the specified packages");

            var packageNamesOption = new Option<IEnumerable<string>>("--packages", "A comman-separated list of package names to unlist");
            result.AddOption(packageNamesOption);

            AddForceOption(result);

            result.SetHandler(async (string apiKey, IEnumerable<string> packageNames, bool force) =>
            {
                var unlistOptions = new UnlistOptions(apiKey, packageNames, force);
                var handler = new CommandHandlers.UnlistCommandHandler(unlistOptions, logger);
                await handler.TryHandle(unlistOptions);
            });

            return result;
        }

        private static void AddForceOption(Command result)
        {
            var forceOption = new Option<bool>("--force", "Calls the underlying NuGet APIs to deprecate the package. Without this parameter (default) the command executes in `dry-run` mode.");
            result.AddOption(forceOption);
        }
    }
}
