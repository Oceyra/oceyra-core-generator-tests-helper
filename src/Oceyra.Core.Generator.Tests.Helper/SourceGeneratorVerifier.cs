using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Oceyra.Core.Generator.Tests.Helper;

public static class SourceGeneratorVerifier
{
    public static GeneratorTestResult CompileAndTest<T>(params string[] sources) where T : IIncrementalGenerator, new()
    {
        var syntaxTrees = sources.Select(source => CSharpSyntaxTree.ParseText(source)).ToArray();

        return CompileAndTest<T>(syntaxTrees: syntaxTrees);
    }

    public static GeneratorTestResult CompileAndTest<T>(AdditionalText[]? additionalTexts = null, params SyntaxTree[] syntaxTrees) where T : IIncrementalGenerator, new()
    {
        var references = new List<MetadataReference>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: syntaxTrees,
            references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new T();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()], // Explicitly pass as IEnumerable<ISourceGenerator>
            additionalTexts: additionalTexts,
            parseOptions: null,
            optionsProvider: null,
            driverOptions: default);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

        return new GeneratorTestResult(
            inputCompilation: compilation,
            outputCompilation: outputCompilation,
            generatorDiagnostics: generateDiagnostics,
            generatorDriver: driver);
    }
}
