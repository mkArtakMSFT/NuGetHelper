using System.Collections.Generic;

namespace NuGetPackageManager.Options
{
    public class DeprecationOptions : INugetApiOptions
    {
        public DeprecationOptions(string apiKey, string packageId, IEnumerable<string> versions, bool force, bool undo)
        {
            this.ApiKey = apiKey;
            this.Versions = versions;
            this.PackageId = packageId;
            this.Force = force;
            this.Undo = undo;
        }

        public string ApiKey { get; set; }

        public string PackageId { get; set; }
        public bool Force { get; private set; }
        public IEnumerable<string> Versions { get; set; }

        public string Message { get; set; }

        public bool Undo { get; set; }
    }
}
