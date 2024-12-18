using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Reflection;
using System.Text;

namespace SonarSilencer
{

    public record DiagnosticAnalyzerInfo(string Category, string Id, string Title, DiagnosticSeverity DefaultSeverity);
    public class CategorizedDiagnosticAnalyzers : Dictionary<string, List<DiagnosticAnalyzerInfo>> { }
    public static class EnumerateAnalyzersInAssembly
    {
        public static ResultResponse<CategorizedDiagnosticAnalyzers> Enumerate(Assembly analyzerAssembly)
        {
            IEnumerable<DiagnosticAnalyzer> diagnosticAnalyzers = null;
            try
            {
                diagnosticAnalyzers = analyzerAssembly.GetTypes()
                      .Where(t => typeof(DiagnosticAnalyzer).IsAssignableFrom(t) && !t.IsAbstract)
                      .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t));
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder stb = new();
                stb.AppendLine("Failed to load some types:");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    stb.AppendLine(loaderException.Message);
                }

                return new ResultResponse<CategorizedDiagnosticAnalyzers>(ResultStatus.Error, null, stb.ToString());
            }

            // List all supported diagnostics
            CategorizedDiagnosticAnalyzers categorizedDiagnostics = new();
            foreach (var analyzer in diagnosticAnalyzers)
            {
                foreach (var diagnostic in analyzer.SupportedDiagnostics)
                {
                    categorizedDiagnostics.TryGetValue(diagnostic.Category, out var knownDiagnostics);
                    if (null == knownDiagnostics)
                    {
                        knownDiagnostics = new();
                        categorizedDiagnostics.Add(diagnostic.Category, knownDiagnostics);
                    }

                    knownDiagnostics.Add(new(diagnostic.Category, diagnostic.Id, diagnostic.Title.ToString(), diagnostic.DefaultSeverity));
                }
            }

            return new ResultResponse<CategorizedDiagnosticAnalyzers>(ResultStatus.Success, categorizedDiagnostics, string.Empty);
        }
    }
}
