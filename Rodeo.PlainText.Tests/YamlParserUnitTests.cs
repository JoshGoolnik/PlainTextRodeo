using Rodeo.PlainText.Core.Entities;
using Rodeo.PlainText.Core.Yaml;

namespace Rodeo.PlainText.Tests;

public class YamlParserUnitTests
{
    // Line number offset to convert from 0-based to 1-based line number (humans read line 1 as line 1, not line 0!)
    private const int LineOffset = 1;
    [Fact]
    public void TestYamlParser_ValidYamlWithNameProperty_ReturnsDataNode()
    {
        // Arrange
        var yaml = "name: Josh";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Josh", result.Node?.Children?.FirstOrDefault(n => n.Name == "name")?.Value);
    }

    [Fact]
    public void TestYamlParser_ValidYamlWithArray_ReturnsDataNode()
    {
        // Arrange
        var yaml = "numbers:\n  - 1\n  - 2";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        var numbersNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "numbers");
        Assert.NotNull(numbersNode);
        Assert.Equal(DataNodeType.Array, numbersNode.NodeType);
        Assert.Equal("1", numbersNode.Children?[0].Value);
        Assert.Equal("2", numbersNode.Children?[1].Value);
    }

    [Fact]
    public void TestYamlParser_ValidYamlWithNumberProperty_ReturnsDataNode()
    {
        // Arrange
        var yaml = "number: 1.10";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.10", result.Node?.Children?.FirstOrDefault(n => n.Name == "number")?.Value);
    }

    [Fact]
    public void TestYamlParser_ValidYamlWithObjectNodeAndZeroChildren_ReturnsDataNode()
    {
        // Arrange
        var yaml = "object: {}";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        var objectNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "object");
        Assert.NotNull(objectNode);
        Assert.Equal(DataNodeType.Object, objectNode.NodeType);
    }

    [Fact]
    public void TestYamlParser_EmptyYamlString_ReturnsEmptyDataNodeResult()
    {
        // Arrange
        var yaml = "";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.Null(result.Message);
        Assert.Null(result.LineNumber);
    }

    [Fact]
    public void TestYamlParser_InvalidYaml_ReturnsErrorMessageAndLineNumber()
    {
        // Arrange
        var yaml = "name: Josh\nage: 30\ninvalid_yaml";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.NotNull(result.Message);
        Assert.Equal(3 + LineOffset, result.LineNumber); // The error should be on line 3 (0-based index)
    }
}