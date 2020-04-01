using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace NuGetPackageManager
{
    [Verb("delete", HelpText = "Unlist all versions of the specified packages")]
    public class DeleteOptions
    {
        public DeleteOptions(string apiKey, IEnumerable<string> packageNames, bool force)
        {
            this.ApiKey = apiKey;
            this.PackageNames = packageNames;
            this.Force = force;
        }

        [Option("apiKey", HelpText = "Provide PAT for the NuGet API account", Required = true)]
        public string ApiKey { get; }

        [Option("packages", HelpText = "Comma separated list of packages to unlist. Note, that all versions of the specified packages will be unlisted.", Required = true, Separator = ',')]
        public IEnumerable<string> PackageNames { get; }

        [Option("force",
            HelpText = "Calls the underlying NuGet APIs to unlist the package. Without this parameter (default) the command executes in `dry-run` mode.",
            Required = false,
            Default = false)]
        public bool Force { get; }

        [Usage(ApplicationAlias = "nugetPackageDeprecator")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal scenario", new DeleteOptions("yourApiKeyFromNuGet", new[] { "pkg1", "pkg2" }, false));
                yield return new Example("Normal scenario", new DeleteOptions("yourApiKeyFromNuGet", new[] { "pkg1", "pkg2" }, true));
            }
        }
    }
}
