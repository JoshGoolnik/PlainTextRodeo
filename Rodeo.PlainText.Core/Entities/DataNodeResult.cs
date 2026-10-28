namespace Rodeo.PlainText.Core.Entities;

public record DataNodeResult
{
    public DataNode? Node { get; set; }
    public string? Message { get; set; }
    public long? LineNumber { get; set; }
}
