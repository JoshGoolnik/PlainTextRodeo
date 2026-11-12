using Rodeo.PlainText.Core.Entities;
using Rodeo.PlainText.Core.Interfaces;
using System.Text;
using System.Xml;

namespace Rodeo.PlainText.Core.Xml;

public class XmlParser : IPlainTextDataParser
{
    // Attributes become child nodes, and the prefix keeps <a id="1"><id>2</id></a> readable: the attribute is "@id", the element is "id"
    private const string AttributePrefix = "@";

    // Upper limit on the text that DTD entities are allowed to expand into, so a few lines of nested entities can't balloon into gigabytes
    private const long MaxCharactersFromEntities = 1_000_000;

    public DataNodeResult Parse(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            // Empty or whitespace XML string, return an empty DataNodeResult with no node and no message
            return new DataNodeResult { Node = null, Message = null, LineNumber = null };
        }
        else
        {
            try
            {
                var xmlDoc = new XmlDocument { XmlResolver = null };
                using (var reader = XmlReader.Create(new StringReader(xml), CreateReaderSettings()))
                {
                    xmlDoc.Load(reader);
                }

                if (xmlDoc.DocumentElement == null)
                {
                    // Load() throws when the root element is missing, so this is only here to keep the compiler's null checks honest
                    return new DataNodeResult { Node = null, Message = null, LineNumber = null };
                }

                return new DataNodeResult { Node = ParseElement(xmlDoc.DocumentElement) };
            }
            catch (XmlException ex)
            {
                // XmlException line numbers are already 1-based (unlike JsonException), so no LineOffset here. Zero means "no line information"
                return new DataNodeResult { Message = ex.Message, LineNumber = ex.LineNumber > 0 ? ex.LineNumber : null };
            }
        }
    }

    private static XmlReaderSettings CreateReaderSettings()
    {
        return new XmlReaderSettings
        {
            // A DOCTYPE is legal XML, so we read it (entities declared inside the document still work)...
            DtdProcessing = DtdProcessing.Parse,
            // ...but we never follow it to a file or URL, and we cap how much text its entities can expand into
            XmlResolver = null,
            MaxCharactersFromEntities = MaxCharactersFromEntities
        };
    }

    private static DataNode ParseElement(XmlElement element)
    {
        var children = new List<DataNode>();

        // Attributes first, in the order they were written
        foreach (XmlAttribute attribute in element.Attributes)
        {
            children.Add(new DataNode
            {
                Name = AttributePrefix + attribute.Name,
                NodeType = DataNodeType.Text,
                Value = attribute.Value
            });
        }

        if (!element.ChildNodes.OfType<XmlElement>().Any())
        {
            // No child elements (e.g. <name>Josh</name>), so the text is the value of the element itself.
            // An element with no text at all (<a></a> or <a/>) gets a null value.
            return new DataNode
            {
                Name = element.Name,
                NodeType = DataNodeType.Element,
                Value = ReadText(element),
                Children = children
            };
        }

        // Child elements, plus any text sitting between them (e.g. <p>Hello <b>big</b> world</p>), in document order
        foreach (XmlNode child in element.ChildNodes)
        {
            if (child is XmlElement childElement)
            {
                children.Add(ParseElement(childElement));
            }
            else if (IsText(child))
            {
                children.Add(new DataNode
                {
                    Name = null,
                    NodeType = DataNodeType.Text,
                    Value = child.Value
                });
            }
            // Anything else (comments, processing instructions) carries no data, so it is skipped
        }

        return new DataNode
        {
            Name = element.Name,
            NodeType = DataNodeType.Element,
            Value = null,
            Children = children
        };
    }

    private static string? ReadText(XmlElement element)
    {
        // Text can arrive in several pieces (e.g. plain text followed by a CDATA section), so join them up
        StringBuilder? text = null;
        foreach (XmlNode child in element.ChildNodes)
        {
            if (IsText(child))
            {
                text ??= new StringBuilder();
                text.Append(child.Value);
            }
        }
        return text?.ToString();
    }

    // The indentation between elements never gets this far: XmlDocument drops it on load (PreserveWhitespace is false by default)
    private static bool IsText(XmlNode node) =>
        node.NodeType is XmlNodeType.Text or XmlNodeType.CDATA or XmlNodeType.SignificantWhitespace;
}
