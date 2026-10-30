using Rodeo.PlainText.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodeo.PlainText.Core.Yaml;

public class YamlParser
{
    private const int LineOffset = Constants.LineOffset;
    public static DataNodeResult ParseYaml(string yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            // Empty or whitespace YAML string, return an empty DataNodeResult with no node and no message
            return new DataNodeResult { Node = null, Message = null, LineNumber = null };
        }
        else
        {
            try
            {
                var input = new StringReader(yaml);
                var yamlStream = new YamlDotNet.RepresentationModel.YamlStream();
                yamlStream.Load(input);
                var rootNode = yamlStream.Documents[0].RootNode;
                var dataNode = ParseElement(rootNode, null);
                return new DataNodeResult { Node = dataNode };
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                return new DataNodeResult { Message = ex.Message, LineNumber = ex.Start.Line + LineOffset };
            }
        }
    }
    private static DataNode ParseElement(YamlDotNet.RepresentationModel.YamlNode node, string? name)
    {
        switch (node.NodeType)
        {
            case YamlDotNet.RepresentationModel.YamlNodeType.Mapping:
                var mappingNode = (YamlDotNet.RepresentationModel.YamlMappingNode)node;
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
            case YamlDotNet.RepresentationModel.YamlNodeType.Sequence:
                var sequenceNode = (YamlDotNet.RepresentationModel.YamlSequenceNode)node;
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
            case YamlDotNet.RepresentationModel.YamlNodeType.Scalar:
                var scalarNode = (YamlDotNet.RepresentationModel.YamlScalarNode)node;
                var nodeType = IdentifyScalarType(scalarNode.Value);
                return new DataNode
                {
                    Name = name,
                    NodeType = nodeType,
                    Value = scalarNode.Value
                };
            default:
                throw new InvalidOperationException($"Unsupported YAML node type: {node.NodeType}");
        }
    }

    private static DataNodeType IdentifyScalarType(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return DataNodeType.Null;
        }
        else
            if (int.TryParse(value, out _))
        {
            return DataNodeType.Number;
        }
        else if (double.TryParse(value, out _))
        {
            return DataNodeType.Number;
        }
        else if (bool.TryParse(value, out _))
        {
            return DataNodeType.Boolean;
        }
        else
        {
            return DataNodeType.String;
        }
    }
}
