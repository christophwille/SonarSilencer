using SonarSilencer;

var analyzerAssemblyResult = SonarAnalyzerAssemblyLoader.Load();

if (analyzerAssemblyResult.Status != ResultStatus.Success)
{
    Console.WriteLine("Failed to load SonarAnalyzer.CSharp assembly.");
    Console.WriteLine(analyzerAssemblyResult.Message);
    return;
}

var categorizedDiagnosticsResult = EnumerateAnalyzersInAssembly.Enumerate(analyzerAssemblyResult.Result!);

if (categorizedDiagnosticsResult.Status != ResultStatus.Success)
{
    Console.WriteLine("Failed to enumerate analyzers from assembly");
    Console.WriteLine(categorizedDiagnosticsResult.Message);
    return;
}

var categorizedDiagnostics = categorizedDiagnosticsResult.Result;
Console.WriteLine("--------------");

foreach (var value in categorizedDiagnostics!.Keys)
{
    var knownDiagnostics = categorizedDiagnostics[value];

    Console.WriteLine("Category: " + value);

    foreach (var diagnostic in knownDiagnostics)
    {
        Console.WriteLine($"     {diagnostic.Id}: {diagnostic.Title} - {diagnostic.DefaultSeverity}");
    }
}

// #### <Category> ####
// # <Title> - <DefaultSeverity>
// dotnet_diagnostic.<Id>.severity = none

// TODO: Take command line argument for positive list of categories to keep (and disable all else)