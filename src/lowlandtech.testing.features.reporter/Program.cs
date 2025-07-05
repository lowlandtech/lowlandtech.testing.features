if (args.Length < 2)
{
    Console.WriteLine("Usage: coverage-tool <TestAssemblyPath> <OutputMarkdownFile> [ReportTitle]");
    return;
}

var assemblyPath = args[0];
var outputPath = args[1];
var reportTitle = args.Length >= 3 ? args[2] : null;

Console.WriteLine($"Loading assembly: {assemblyPath}");
var assembly = Assembly.LoadFrom(assemblyPath);

// This line is the extension method in action:
assembly.GenerateCoverageReport(outputPath, reportTitle);

Console.WriteLine($"✅ Coverage report generated at {outputPath}");