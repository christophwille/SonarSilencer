using System.Reflection;

namespace SonarSilencer
{
    public static class SonarAnalyzerAssemblyLoader
    {
        public static ResultResponse<Assembly> Load()
        {
            string nugetPackagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nuget", "packages", "sonaranalyzer.csharp");

            // Get the latest version directory
            var latestVersionDir = new DirectoryInfo(nugetPackagePath)
                .GetDirectories()
                .OrderByDescending(d => d.Name)
                .FirstOrDefault();

            if (latestVersionDir == null)
            {
                return new ResultResponse<Assembly>(ResultStatus.Error, null, "Analyzer package not found.");
            }

            string analyzerDllPath = Path.Combine(latestVersionDir.FullName, "analyzers", "SonarAnalyzer.CSharp.dll");

            if (!File.Exists(analyzerDllPath))
            {
                return new ResultResponse<Assembly>(ResultStatus.Error, null, "Analyzer DLL not found.");
            }

            var analyzerAssembly = Assembly.LoadFrom(analyzerDllPath);

            return new ResultResponse<Assembly>(ResultStatus.Success, analyzerAssembly, string.Empty);
        }
    }
}
