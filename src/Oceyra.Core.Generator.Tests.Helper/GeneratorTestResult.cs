using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using System.Collections.Immutable;
using System.Reflection;

namespace Oceyra.Core.Generator.Tests.Helper;

public class GeneratorTestResult(CSharpCompilation inputCompilation, Compilation outputCompilation, ImmutableArray<Diagnostic> generatorDiagnostics, GeneratorDriver generatorDriver)
{
    public CSharpCompilation InputCompilation { get; } = inputCompilation;
    public Compilation OutputCompilation { get; } = outputCompilation;
    public ImmutableArray<Diagnostic> GeneratorDiagnostics { get; } = generatorDiagnostics;
    public GeneratorDriver GeneratorDriver { get; } = generatorDriver;
    public Assembly? CompiledAssembly { get; private set; } = null;

    // Helper methods for common assertions
    public GeneratorTestResult ShouldHaveNoErrors()
    {
        var errors = OutputCompilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        errors.ShouldBeEmpty($"Compilation failed with errors: {string.Join(", ", errors.Select(e => e.GetMessage()))}");
        return this;
    }

    public GeneratorTestResult ShouldHaveError(string diagnosticId)
    {
        var error = OutputCompilation.GetDiagnostics().FirstOrDefault(d => d.Id == diagnosticId);
        error.ShouldNotBeNull($"Expected diagnostic {diagnosticId} was not found");
        return this;
    }

    public GeneratorTestResult ShouldHaveGeneratorError(string diagnosticId)
    {
        var error = GeneratorDiagnostics.FirstOrDefault(d => d.Id == diagnosticId);
        error.ShouldNotBeNull($"Expected generator diagnostic {diagnosticId} was not found");
        return this;
    }

    public GeneratorTestResult ShouldExecuteWithin(TimeSpan maxTime)
    {
        var totalTime = GeneratorDriver.GetTimingInfo().ElapsedTime;
        totalTime.ShouldBeLessThan(maxTime, $"Generator took {totalTime.TotalMilliseconds}ms, expected less than {maxTime.TotalMilliseconds}ms");
        return this;
    }

    // Access to detailed generator-specific timing
    public GeneratorTestResult ShouldHaveGeneratorTimeWithin<T>(TimeSpan maxTime) where T : IIncrementalGenerator
    {
        var generatorName = typeof(T).Name;
        var generatorTiming = GeneratorDriver.GetTimingInfo().GeneratorTimes.FirstOrDefault(gt => gt.Generator.GetType().Name == generatorName);

        generatorTiming.ElapsedTime.ShouldBeLessThan(maxTime,
            $"Generator {generatorName} took {generatorTiming.ElapsedTime.TotalMilliseconds}ms, expected less than {maxTime.TotalMilliseconds}ms");

        return this;
    }

    public GeneratorTestResult ShouldGenerateFiles(int expectedCount)
    {
        var generatedFiles = OutputCompilation.SyntaxTrees.Count(st => st.FilePath.EndsWith(".g.cs"));
        generatedFiles.ShouldBe(expectedCount);
        return this;
    }

    public Type? GetCompiledType(string typeName)
    {
        ShouldHaveNoErrors();

        if (CompiledAssembly == null)
        {
            using var ms = new MemoryStream();
            var emitResult = OutputCompilation.Emit(ms);
            emitResult.Success.ShouldBeTrue();

            ms.Seek(0, SeekOrigin.Begin);
            CompiledAssembly = Assembly.Load(ms.ToArray());
        }

        return CompiledAssembly?.GetType(typeName);
    }

    public ConstructorInfo? GetConstructor(string typeName, params Type[] parameterTypes)
    {
        var type = GetCompiledType(typeName);
        return type?.GetConstructor(parameterTypes);
    }
}