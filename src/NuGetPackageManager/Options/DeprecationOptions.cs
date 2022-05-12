using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace NuGetPackageManager.Options
{
    [Verb("deprecate", HelpText = "Deprecate specified versions of the specified package")]
    public class DeprecationOptions : INugetApiOptions
    {
        public DeprecationOptions(string apiKey, string packageName, IEnumerable<string> packageVersions, bool force)
        {
            this.ApiKey = apiKey;
            this.PackageVersions = packageVersions;
            this.PackageName = packageName;
            this.Force = force;
        }

        [Option("apiKey", HelpText = "Provide PAT for the NuGet API account", Required = true)]
        public string ApiKey { get; }

        [Option("packageId", HelpText = "The name of the package to deprecate", Required = true)]
        public string PackageName { get; }

        [Option("packages", HelpText = "Comma separated list of package versions to deprecate. Note, that all versions of the specified package will be deprecated.", Required = true, Separator = ',')]
        public IEnumerable<string> PackageVersions { get; }

        [Option("message", HelpText = "The deprecation message to show in NuGet.org for each of the versions to be deprecated.", Required = true)]
        public string Message { get; }

        [Option("force",
            HelpText = "Calls the underlying NuGet APIs to deprecate the package. Without this parameter (default) the command executes in `dry-run` mode.",
            Required = false,
            Default = false)]
        public bool Force { get; }

        [Usage(ApplicationAlias = "nugetPackageManager")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal scenario", new DeleteOptions("yourApiKeyFromNuGet", new[] { "pkg1", "1, 2.1, 3.3.4" }, false));
                yield return new Example("Normal scenario", new DeleteOptions("yourApiKeyFromNuGet", new[] { "pkg1", "3.3" }, true));
            }
        }
    }
}
