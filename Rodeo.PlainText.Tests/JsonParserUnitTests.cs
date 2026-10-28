using Rodeo.PlainText.Core.Entities;
using Rodeo.PlainText.Core.Json;

namespace Rodeo.PlainText.Tests;

public class JsonParserUnitTests
{
    // Unit tests for the Json Parser
    [Fact]
    public void TestJsonParser_ValidJsonWithNameProperty_ReturnDataNode()
    {
        // Arrange
        var json = "{\"name\":\"Josh\"}";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Josh", result.Node?.Children?.Find(n => n.Name == "name")?.Value);
    }

    [Fact]
    public void TestJsonParser_ValidJsonWithArray_ReturnsDataNode()
    {
        // Arrange
        var json = "{\"numbers\":[1,2]}";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        var numbersNode = result.Node?.Children?.Find(n => n.Name == "numbers");
        Assert.NotNull(numbersNode);
        Assert.Equal(DataNodeType.Array, numbersNode.NodeType);
        Assert.Equal("1", numbersNode.Children?[0].Value);
        Assert.Equal("2", numbersNode.Children?[1].Value);
    }

    [Fact]
    public void TestJsonParser_ValidJsonWithNumberProperty_ReturnsDataNode()
    {
        // Arrange
        var json = "{\"number\":1.10}";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.10", result.Node?.Children?.Find(n => n.Name == "number")?.Value);
    }

    [Fact]
    public void TestJsonParser_ValidJsonWithObjectNodeAndZeroChildren_ReturnsDataNode()
    {
        // Arrange
        var json = "{\"object\":{}}";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        var objectNode = result.Node?.Children?.Find(n => n.Name == "object");
        Assert.NotNull(objectNode);
        Assert.Equal(DataNodeType.Object, objectNode.NodeType);
        Assert.Empty(objectNode.Children);
    }

    [Fact]
    public void TestJsonParser_InvalidJsonEmptyString_ReturnsDataNodeResultWithErrorMessage()
    {
        // Arrange
        var json = "";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.NotNull(result.Message);
        Assert.Equal(1, result.LineNumber);
    }

    [Fact]
    public void TestJsonParser_ValidJson_ReturnsDataNode()
    {
        // Arrange
        var json = "{\"name\":\"Josh\",\"age\":42,\"isStudent\":false}";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Josh", result.Node?.Children?.Find(n => n.Name == "name")?.Value);
        Assert.Equal("42", result.Node?.Children?.Find(n => n.Name == "age")?.Value);
        Assert.Equal("false", result.Node?.Children?.Find(n => n.Name == "isStudent")?.Value);
    }

    [Fact]
    public void TestJsonParser_EmptyJson_ReturnsEmptyDataNode()
    {
        // Arrange
        var json = "{}";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(DataNodeType.Object, result.Node?.NodeType);
        Assert.Empty(result.Node?.Children);
    }

    [Fact]
    public void TestJsonParser_NestedJson_ReturnsCorrectStructure()
    {
        // Arrange
        var json = "{\"person\":{\"name\":\"John\",\"age\":30}}";
        // Act
        var result = JsonParser.ParseJson(json);
        // Assert
        Assert.NotNull(result);
        var personNode = result.Node?.Children?.Find(n => n.Name == "person");
        Assert.NotNull(personNode);
        Assert.Equal(DataNodeType.Object, personNode.NodeType);
        Assert.Equal("John", personNode.Children?.Find(n => n.Name == "name")?.Value);
        Assert.Equal("30", personNode.Children?.Find(n => n.Name == "age")?.Value);
    }
}
