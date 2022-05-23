using System.Collections.Generic;

namespace NuGetPackageManager.Options
{
    public class UnlistOptions : INugetApiOptions
    {
        public UnlistOptions(string apiKey, IEnumerable<string> packageNames, bool force)
        {
            this.ApiKey = apiKey;
            this.PackageNames = packageNames;
            this.Force = force;
        }

        public string ApiKey { get; }

        public IEnumerable<string> PackageNames { get; }

        public bool Force { get; }
    }
}
