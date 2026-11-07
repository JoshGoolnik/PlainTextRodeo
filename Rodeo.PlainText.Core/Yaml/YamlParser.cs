using Rodeo.PlainText.Core.Entities;
using Rodeo.PlainText.Core.Intefaces;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
namespace Rodeo.PlainText.Core.Yaml;

public class YamlParser : IPlainTextDataParser
{
    public DataNodeResult Parse(string yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            // Empty or whitespace YAML string, return an empty DataNodeResult with no node and no message
            return EmptyDataNodeResult();
        }
        else
        {
            try
            {
                var input = new StringReader(yaml);
                var yamlStream = new YamlStream();
                yamlStream.Load(input);

                if (yamlStream.Documents.Count == 0)
                {
                    // No documents found in the YAML string, return an empty DataNodeResult with no node and no message
                    return EmptyDataNodeResult();
                }

                var rootNode = yamlStream.Documents[0].RootNode;
                var dataNode = ParseElement(rootNode, null);
                return new DataNodeResult { Node = dataNode };
            }
            // We have two catches here, for two different issues - first if there's a normal YamlException...
            catch (YamlException ex)
            {
                return new DataNodeResult { Message = ex.Message, LineNumber = ex.Start.Line };
            }
            // ... second if you get specific errors, like an unclosed bracket and a new line. I'm not sure why it doesn't use Yaml Exception...
            catch (InvalidOperationException ex)
            {
                return new DataNodeResult { Message = ex.Message };
            }
        }
    }
    private DataNode ParseElement(YamlNode node, string? name)
    {
        switch (node.NodeType)
        {
            case YamlNodeType.Mapping:
                var mappingNode = (YamlMappingNode)node;
                List<DataNode> mappingChildren = new List<DataNode>();
                foreach (var entry in mappingNode.Children)
                {
                    var childNode = ParseElement(entry.Value, entry.Key.ToString());
                    mappingChildren.Add(childNode);
                }
                return new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.Object,
                    Children = mappingChildren
                };
            case YamlNodeType.Sequence:
                var sequenceNode = (YamlSequenceNode)node;
                List<DataNode> sequenceChildren = new List<DataNode>();
                foreach (var item in sequenceNode.Children)
                {
                    var childNode = ParseElement(item, null);
                    sequenceChildren.Add(childNode);
                }
                return new DataNode
                {
                    Name = name,
                    NodeType = DataNodeType.Array,
                    Children = sequenceChildren
                };
            case YamlNodeType.Scalar:
                var scalarNode = (YamlScalarNode)node;
                var nodeType = IdentifyScalarType(scalarNode);
                return new DataNode
                {
                    Name = name,
                    NodeType = nodeType,
                    Value = nodeType == DataNodeType.Null ? null : scalarNode.Value
                };
            default:
                throw new NotSupportedException($"Unsupported YAML node type: {node.NodeType}");
        }
    }

    private DataNodeType IdentifyScalarType(YamlScalarNode scalar)
    {
        var value = scalar.Value;

        if (scalar.Style != ScalarStyle.Plain)
            return DataNodeType.String;

        if (string.IsNullOrEmpty(value) || value == "null" || value == "~")
            return DataNodeType.Null;

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            return DataNodeType.Number;

        if (bool.TryParse(value, out _))
            return DataNodeType.Boolean;

        return DataNodeType.String;
    }

    private DataNodeResult EmptyDataNodeResult() => new DataNodeResult { Node = null, Message = null, LineNumber = null };
}
