using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Oceyra.Core.Generator.Tests.Helper;

public class InMemoryAdditionalText : AdditionalText
{
    public override string Path { get; }
    private SourceText Text { get; }

    public InMemoryAdditionalText(string path, string content)
    {
        Path = path;
        Text = SourceText.From(content, Encoding.UTF8);
    }

    public override SourceText GetText(CancellationToken cancellationToken = default) => Text;
}
