using Rodeo.PlainText.Core.Entities;
using Rodeo.PlainText.Core.Xml;

namespace Rodeo.PlainText.Tests;

public class XmlParserUnitTests
{
    [Fact]
    public void TestXmlParser_ValidXmlWithNameProperty_ReturnsDataNode()
    {
        // Arrange
        var xml = "<root><name>Josh</name></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Josh", result.Node?.Children?.FirstOrDefault(n => n.Name == "name")?.Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithArray_ReturnsDataNode()
    {
        // Arrange
        var xml = "<root><numbers><number>1</number><number>2</number></numbers></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        var numbersNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "numbers");
        Assert.NotNull(numbersNode);
        Assert.Equal(DataNodeType.Element, numbersNode.NodeType);
        Assert.Equal("1", numbersNode.Children?[0].Value);
        Assert.Equal("2", numbersNode.Children?[1].Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithNumberProperty_ReturnsDataNode()
    {
        // Arrange
        var xml = "<root><number>1.10</number></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.10", result.Node?.Children?.FirstOrDefault(n => n.Name == "number")?.Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithObjectNodeAndZeroChildren_ReturnsDataNode()
    {
        // Arrange
        var xml = "<root><object></object></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        var objectNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "object");
        Assert.NotNull(objectNode);
        Assert.Equal(DataNodeType.Element, objectNode.NodeType);
        Assert.Empty(objectNode.Children);
    }

    [Fact]
    public void TestXmlParser_EmptyXmlString_ReturnsEmptyDataNodeResult()
    {
        // Arrange
        var xml = "";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.Null(result.Message);
        Assert.Null(result.LineNumber);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithAttributes_ReturnsAttributesAsChildNodes()
    {
        // Arrange
        var xml = "<root><person id=\"1\" role=\"admin\" /></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        var personNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "person");
        Assert.NotNull(personNode);
        Assert.Null(personNode.Value);
        Assert.Equal(2, personNode.Children.Count);
        Assert.Equal("@id", personNode.Children[0].Name);
        Assert.Equal("1", personNode.Children[0].Value);
        Assert.Equal(DataNodeType.Text, personNode.Children[0].NodeType);
        Assert.Equal("@role", personNode.Children[1].Name);
        Assert.Equal("admin", personNode.Children[1].Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithAttributeAndText_ReturnsValueAndAttribute()
    {
        // Arrange
        var xml = "<root><price currency=\"GBP\">10</price></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        var priceNode = result.Node?.Children?.FirstOrDefault(n => n.Name == "price");
        Assert.NotNull(priceNode);
        Assert.Equal("10", priceNode.Value);
        Assert.Equal("GBP", priceNode.Children.FirstOrDefault(n => n.Name == "@currency")?.Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithCData_ReturnsCDataAsValue()
    {
        // Arrange
        var xml = "<root><code><![CDATA[a < b]]></code></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("a < b", result.Node?.Children?.FirstOrDefault(n => n.Name == "code")?.Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithMixedContent_ReturnsTextAndElementsInOrder()
    {
        // Arrange
        var xml = "<p>Hello <b>big</b> world</p>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Node);
        Assert.Null(result.Node.Value);
        Assert.Equal(3, result.Node.Children.Count);
        Assert.Equal(DataNodeType.Text, result.Node.Children[0].NodeType);
        Assert.Null(result.Node.Children[0].Name);
        Assert.Equal("Hello ", result.Node.Children[0].Value);
        Assert.Equal(DataNodeType.Element, result.Node.Children[1].NodeType);
        Assert.Equal("b", result.Node.Children[1].Name);
        Assert.Equal("big", result.Node.Children[1].Value);
        Assert.Equal(DataNodeType.Text, result.Node.Children[2].NodeType);
        Assert.Equal(" world", result.Node.Children[2].Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithIndentationAndComments_IgnoresThem()
    {
        // Arrange
        var xml = "<?xml version=\"1.0\"?>\n<!-- a comment -->\n<root>\n  <!-- another comment -->\n  <name>Josh</name>\n  <age>21</age>\n</root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Node);
        Assert.Equal("root", result.Node.Name);
        Assert.Equal(2, result.Node.Children.Count);
        Assert.Equal("Josh", result.Node.Children[0].Value);
        Assert.Equal("21", result.Node.Children[1].Value);
    }

    [Fact]
    public void TestXmlParser_ValidXmlWithInternalEntity_ReturnsExpandedValue()
    {
        // Arrange
        var xml = "<!DOCTYPE root [<!ENTITY me \"Josh\">]><root><name>&me; &amp; co</name></root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Josh & co", result.Node?.Children?.FirstOrDefault(n => n.Name == "name")?.Value);
    }

    [Fact]
    public void TestXmlParser_InvalidXmlWithUnclosedTag_ReturnsMessageAndLineNumber()
    {
        // Arrange
        var xml = "<root>\n<name>Josh\n</root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.NotNull(result.Message);
        Assert.Equal(3, result.LineNumber);
    }

    [Fact]
    public void TestXmlParser_InvalidXmlWithNoRootElement_ReturnsMessageAndNoLineNumber()
    {
        // Arrange
        var xml = "<?xml version=\"1.0\"?>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.NotNull(result.Message);
        Assert.Null(result.LineNumber);
    }

    [Fact]
    public void TestXmlParser_XmlWithRunawayEntityExpansion_ReturnsMessage()
    {
        // Arrange
        // Seven entities, each made of ten copies of the one before, so &g; would expand to 10,000,000 characters
        var xml = "<!DOCTYPE root [" +
            "<!ENTITY a \"aaaaaaaaaa\">" +
            "<!ENTITY b \"&a;&a;&a;&a;&a;&a;&a;&a;&a;&a;\">" +
            "<!ENTITY c \"&b;&b;&b;&b;&b;&b;&b;&b;&b;&b;\">" +
            "<!ENTITY d \"&c;&c;&c;&c;&c;&c;&c;&c;&c;&c;\">" +
            "<!ENTITY e \"&d;&d;&d;&d;&d;&d;&d;&d;&d;&d;\">" +
            "<!ENTITY f \"&e;&e;&e;&e;&e;&e;&e;&e;&e;&e;\">" +
            "<!ENTITY g \"&f;&f;&f;&f;&f;&f;&f;&f;&f;&f;\">" +
            "]><root>&g;</root>";
        var xmlParser = new XmlParser();
        // Act
        var result = xmlParser.Parse(xml);
        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Node);
        Assert.NotNull(result.Message);
    }
}
