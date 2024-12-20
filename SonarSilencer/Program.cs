using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using SonarSilencer;
using System.ComponentModel.DataAnnotations;

public enum CategoriesListType
{
    Positive,
    Negative
}

public static class CategoriesListTypeExtensions
{
    public static bool IsPositive(this CategoriesListType type) => type == CategoriesListType.Positive;
    public static bool IsNegative(this CategoriesListType type) => type == CategoriesListType.Negative;
}

[HelpOption("-h|--help")]
class SonarSilencerCmdProgram
{
    public static Task<int> Main(string[] args) => new HostBuilder().RunCommandLineApplicationAsync<SonarSilencerCmdProgram>(args);

    [Required]
    [Argument(0, "Categories", "The list of categories. This argument is mandatory.")]
    public string[] Categories { get; }


    [Option("-l|--listtype <type>", "Are Categories positive or negative list?", CommandOptionType.SingleValue)]
    public CategoriesListType CategoriesListType { get; } = CategoriesListType.Positive;

    private readonly IHostEnvironment _env;
    public SonarSilencerCmdProgram(IHostEnvironment env)
    {
        _env = env;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
    private Task<int> OnExecuteAsync(CommandLineApplication app)
    {
        var analyzerAssemblyResult = SonarAnalyzerAssemblyLoader.Load();

        if (analyzerAssemblyResult.Status != ResultStatus.Success)
        {
            Console.WriteLine("Failed to load SonarAnalyzer.CSharp assembly.");
            Console.WriteLine(analyzerAssemblyResult.Message);
            return Task.FromResult(1);
        }

        var categorizedDiagnosticsResult = EnumerateAnalyzersInAssembly.Enumerate(analyzerAssemblyResult.Result!);

        if (categorizedDiagnosticsResult.Status != ResultStatus.Success)
        {
            Console.WriteLine("Failed to enumerate analyzers from assembly");
            Console.WriteLine(categorizedDiagnosticsResult.Message);
            return Task.FromResult(2);
        }

        var categorizedDiagnostics = categorizedDiagnosticsResult.Result;
        Console.WriteLine("-------------- REFERENCE --------------");

        foreach (var value in categorizedDiagnostics!.Keys)
        {
            var knownDiagnostics = categorizedDiagnostics[value];

            Console.WriteLine($"Category: {value} ({knownDiagnostics.Count})");

            foreach (var diagnostic in knownDiagnostics)
            {
                Console.WriteLine($"     {diagnostic.Id}: {diagnostic.Title} - {diagnostic.DefaultSeverity}");
            }
        }


        Console.WriteLine("-------------- .editorconfig --------------");

        // #### <Category> ####
        // # <Title> - <DefaultSeverity>
        // dotnet_diagnostic.<Id>.severity = none

        foreach (var value in categorizedDiagnostics!.Keys)
        {
            var knownDiagnostics = categorizedDiagnostics[value];

            if (CategoriesListType.IsPositive() && Categories.Contains(value, StringComparer.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (CategoriesListType.IsNegative() && !Categories.Contains(value, StringComparer.InvariantCultureIgnoreCase))
            {
                continue;
            }

            Console.WriteLine($"\r\n#### {value} ({knownDiagnostics.Count}) ####");
            foreach (var diagnostic in knownDiagnostics)
            {
                Console.WriteLine($"# {diagnostic.Title} - {diagnostic.DefaultSeverity}");
                Console.WriteLine($"dotnet_diagnostic.{diagnostic.Id}.severity = none");
            }
        }

        return Task.FromResult(0);
    }
}