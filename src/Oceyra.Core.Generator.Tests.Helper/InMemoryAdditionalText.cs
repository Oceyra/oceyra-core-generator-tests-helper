using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Oceyra.Core.Generator.Tests.Helper;

public class InMemoryAdditionalText(string path, string content) : AdditionalText
{
    public override string Path { get; } = path;
    private SourceText Text { get; } = SourceText.From(content, Encoding.UTF8);

    public override SourceText GetText(CancellationToken cancellationToken = default) => Text;
}
