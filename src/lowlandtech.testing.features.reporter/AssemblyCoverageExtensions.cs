namespace LowlandTech.Testing.Features.Reporter;

/// <summary>
/// Provides extension methods for generating coverage reports from test assemblies.
/// </summary>
public static class AssemblyCoverageExtensions
{
    /// <summary>
    /// Generates a code coverage report for the specified assembly and writes it to the specified file path.
    /// </summary>
    /// <remarks>This method analyzes the provided assembly to generate a detailed code coverage report.  The
    /// report is written to the specified file path in plain text format.</remarks>
    /// <param name="assembly">The assembly to analyze for code coverage.</param>
    /// <param name="outputPath">The file path where the generated report will be saved. Must be a valid, writable path.</param>
    /// <param name="reportTitle">An optional title for the report. If not provided, a default title will be used.</param>
    public static void GenerateCoverageReport(this Assembly assembly, string outputPath, string? reportTitle = null)
    {
        var sb = BuildReport(assembly, reportTitle);
        File.WriteAllText(outputPath, sb.ToString());
    }

    /// <summary>
    /// Generates a code coverage report for the specified assembly and writes it to the specified file path.
    /// </summary>
    /// <remarks>The generated report provides an overview of code coverage for the specified assembly. 
    /// Ensure that the <paramref name="outputPath"/> is a valid and writable file path.</remarks>
    /// <param name="assembly">The assembly for which the coverage report will be generated. Cannot be <see langword="null"/>.</param>
    /// <param name="outputPath">The file path where the generated report will be saved. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="reportTitle">An optional title for the coverage report. If <see langword="null"/>, a default title will be used.</param>
    /// <returns>A task that represents the asynchronous operation of generating and saving the coverage report.</returns>
    public static async Task GenerateCoverageReportAsync(this Assembly assembly, string outputPath, string? reportTitle = null)
    {
        var sb = BuildReport(assembly, reportTitle);
        await File.WriteAllTextAsync(outputPath, sb.ToString());
    }

    private static StringBuilder BuildReport(Assembly assembly, string? reportTitle)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {reportTitle ?? "Feature Coverage Report"}");
        sb.AppendLine();

        var scenarioTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttributes().Any(attr => attr.GetType().Name == "ScenarioAttribute"))
            .OrderBy(t => t.Name)
            .ToList();

        foreach (var type in scenarioTypes)
        {
            var scenarioAttr = type.GetCustomAttributes()
                .First(attr => attr.GetType().Name == "ScenarioAttribute");

            var given = scenarioAttr.GetType().GetProperty("Given")?.GetValue(scenarioAttr)?.ToString() ?? "(No given)";
            var when = scenarioAttr.GetType().GetProperty("When")?.GetValue(scenarioAttr)?.ToString();
            var then = scenarioAttr.GetType().GetProperty("Then")?.GetValue(scenarioAttr)?.ToString();

            var code = scenarioAttr.GetType().GetProperty("Code")?.GetValue(scenarioAttr)?.ToString();
            var title = scenarioAttr.GetType().GetProperty("Title")?.GetValue(scenarioAttr)?.ToString();

            var nodeAttr = type.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name == "NodeIdAttribute");
            var nodeId = nodeAttr?.GetType().GetProperty("NodeId")?.GetValue(nodeAttr)?.ToString();

            var taskAttr = type.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name == "TaskIdAttribute");
            var taskId = taskAttr?.GetType().GetProperty("TaskId")?.GetValue(taskAttr)?.ToString();

            var useCaseAttr = type.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name == "UseCaseIdAttribute");
            var useCaseId = useCaseAttr?.GetType().GetProperty("UseCaseId")?.GetValue(useCaseAttr)?.ToString();

            sb.AppendLine($"## {(string.IsNullOrEmpty(title) ? given : title)}");

            // Show Scenario Code if it exists
            if (!string.IsNullOrEmpty(code))
                sb.AppendLine($"**Scenario Code:** `{code}`");

            sb.AppendLine();

            if (!string.IsNullOrEmpty(given))
                sb.AppendLine($"> **Given:** {given}");
            if (!string.IsNullOrEmpty(when))
                sb.AppendLine($"> **When:** {when}");
            if (!string.IsNullOrEmpty(then))
                sb.AppendLine($"> **Then:** {then}");

            sb.AppendLine();

            sb.AppendLine($"**Test Class:** `{type.Name}`");

            // Write tags
            var tags = new List<string>();
            if (!string.IsNullOrEmpty(code)) tags.Add(code);
            if (!string.IsNullOrEmpty(taskId)) tags.Add(taskId);
            if (!string.IsNullOrEmpty(useCaseId)) tags.Add(useCaseId);
            if (!string.IsNullOrEmpty(nodeId)) tags.Add(nodeId);

            if (tags.Any())
            {
                sb.AppendLine();
                sb.AppendLine($"**Tags:** {string.Join(", ", tags.Select(t => $"`{t}`"))}");
            }

            sb.AppendLine();

            var factMethods = type.GetMethods()
                .Where(m => m.GetCustomAttributes().Any(a => a.GetType().Name == "FactAttribute"))
                .OrderBy(m => m.Name)
                .ToList();

            foreach (var method in factMethods)
            {
                var thenAttr = method.GetCustomAttributes()
                    .FirstOrDefault(a => a.GetType().Name == "ThenAttribute");

                var description = thenAttr?.GetType().GetProperty("Description")?.GetValue(thenAttr)?.ToString()
                    ?? "(No description)";
                var uacCode = thenAttr?.GetType().GetProperty("Code")?.GetValue(thenAttr)?.ToString();

                var factLine = $"- ✅ `{method.Name}` — {description}";
                if (!string.IsNullOrEmpty(uacCode))
                    factLine += $" (`{uacCode}`)";

                sb.AppendLine(factLine);
            }

            sb.AppendLine();
        }

        return sb;
    }
}
