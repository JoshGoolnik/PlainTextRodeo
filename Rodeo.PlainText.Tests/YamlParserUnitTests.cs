using Rodeo.PlainText.Core.Entities;
using Rodeo.PlainText.Core.Yaml;

namespace Rodeo.PlainText.Tests;

public class YamlParserUnitTests
{
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
        var yaml = "name: Josh\nage: 42\ninvalid_yaml";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.NotNull(result.Message);
        Assert.Equal(3, result.LineNumber); // YamlDotNet counts lines from 1, so line 3 is line 3
    }

    [Fact]
    public void TestYamlParser_JustAComment_ReturnsEmptyDataNodeResult()
    {
        // Arrange
        var yaml = "# This is a comment";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.Null(result.Message);
        Assert.Null(result.LineNumber);
    }

    [Fact]
    public void TestYamlParser_InvalidYamlWithUnclosedBracketAndNewLine_ReturnsErrorMessage()
    {
        // Arrange
        var yaml = "name: Josh\nage: 42\ninvalid_yaml: [1, 2, 3\ncity: Aberdeen";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.NotNull(result.Message);
        Assert.Null(result.LineNumber); // This kind of error (InvalidOperationException) carries no line number
    }

    [Fact]
    public void TestYamlParser_QuotedNumber_ReturnsString()
    {
        // Arrange
        var yaml = "age: \"42\"";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        var ageNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "age");
        Assert.NotNull(ageNode);
        Assert.Equal(DataNodeType.String, ageNode.NodeType); // The quotes make it text, not a number
        Assert.Equal("42", ageNode.Value);
    }

    [Theory]
    [InlineData("name: null")]
    [InlineData("name: ~")]
    [InlineData("name:")]
    public void TestYamlParser_NullValue_ReturnsNullNode(string yaml)
    {
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        var nameNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "name");
        Assert.NotNull(nameNode);
        Assert.Equal(DataNodeType.Null, nameNode.NodeType);
        Assert.Null(nameNode.Value); // Same as the JSON parser: Null nodes have no value
    }

    [Fact]
    public void TestYamlParser_NumberWithComma_ReturnsString()
    {
        // Arrange
        var yaml = "count: 1,000";
        // Act
        var result = YamlParser.ParseYaml(yaml);
        // Assert
        var countNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "count");
        Assert.NotNull(countNode);
        Assert.Equal(DataNodeType.String, countNode.NodeType); // YAML numbers don't use thousands separators
    }
}
