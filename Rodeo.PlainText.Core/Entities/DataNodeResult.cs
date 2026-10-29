namespace Rodeo.PlainText.Core.Entities;

public record DataNodeResult
{
    public DataNode? Node { get; init; }
    public string? Message { get; init; }
    public long? LineNumber { get; init; }
}
