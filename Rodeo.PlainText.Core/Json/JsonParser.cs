using Rodeo.PlainText.Core.Entities;
using System.Text.Json;

namespace Rodeo.PlainText.Core.Json;

public class JsonParser
{
    public static DataNodeResult ParseJson(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                DataNode node = ParseElement(doc.RootElement, null);
                return new DataNodeResult { Node = node };
            }
        }
        catch (JsonException ex)
        {
            // Line Numer + 1 to convert from 0-based to 1-based line number (humans read line 1 as line 1, not line 0!)
            return new DataNodeResult { Message = ex.Message, LineNumber = ex.LineNumber + 1 }; 
        }
    }

    private static DataNode ParseElement(JsonElement element, string? name)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                List<DataNode> objectChildren = new List<DataNode>();
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    var childNode = ParseElement(property.Value, property.Name);
                    objectChildren.Add(childNode);
                }
                var objectNode = new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.Object,
                    Children = objectChildren   
                };
                return objectNode;
            case JsonValueKind.Array:
                List<DataNode> arrayChildren = new List<DataNode>();
                foreach (JsonElement item in element.EnumerateArray())
                {
                    var childNode = ParseElement(item, null);
                    arrayChildren.Add(childNode);
                }
                var arrayNode = new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.Array,
                    Children = arrayChildren
                };
                return arrayNode;
            case JsonValueKind.String:
                return new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.String,
                    Value = element.GetString()
                };
            case JsonValueKind.Number:
                return new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.Number,
                    Value = element.GetRawText()
                };
            case JsonValueKind.True:
            case JsonValueKind.False:
                return new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.Boolean,
                    Value = element.GetBoolean().ToString().ToLowerInvariant()
                };
            case JsonValueKind.Null:
                return new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.Null
                };
            default:
                throw new NotSupportedException($"Unsupported JSON value kind: {element.ValueKind}");
        }
    }
}
