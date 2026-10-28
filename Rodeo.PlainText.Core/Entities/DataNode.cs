namespace Rodeo.PlainText.Core.Entities;

public record DataNode
{
    // All the properties are init-only to ensure immutability after creation (you can set the values when you create the object, but not afterwards!)
    public string? Name { get; init; }
    public DataNodeType NodeType { get; init; }
    public string? Value { get; init; }
    public IReadOnlyList<DataNode> Children { get; init; } = [];
}
