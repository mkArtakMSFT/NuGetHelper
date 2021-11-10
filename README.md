# NuGetHelper
A tool to help with NuGet package management

This utilitiy is used to help with unlisting NuGet packages.
It automates the process of unlisting individual versions of a given package and can also handle unlisting multiple packages at once.

The tool accepts the following set of parameters:

| Parameter name | Description | Required | Default value |
| --- | --- | --- | --- |
| apiKey | The API key used for package management | Yes | N / A |
| packages | Comma separated list of package names to unlist | Yes | N / A |
| force | By default (without this parameter) the command will execute in "read-only" mode, which will result in no packages being unlisted. It will simply print which packages and which versions would be unlisted. After you're sure about your intention, pass this flag explicitly as `--force` to run the actual unlisting. | No | false |

## Examples
This tool can be used to unlist all versions of one or more package. When using the tool, start without the `--force` parameter, to validate your intentions by running the following command:

### Unlisting a single pacakge
`NuGetPackageManager.exe --apiKey yourApiKeyGoesHere --packages yourPackageName`

If the list of package names and versions are indeed what you've expected to see, you can now append the `--force` argument in the end of the above command to unlist the `yourPackageName` package:

`NuGetPackageManager.exe --apiKey yourApiKeyGoesHere --packages yourPackageName --force`

### Unlisting multiple packages
`NuGetPackageManager.exe --apiKey yourApiKeyGoesHere --packages packageName1,packageName2,packageName3`

If the list of package names and versions are indeed what you've expected to see, you can now append the `--force` argument in the end of the above command to unlist the packages:

`NuGetPackageManager.exe --apiKey yourApiKeyGoesHere --packages packageName1,packageName2,packageName3 --force`
