using System.Diagnostics;

namespace TextMonster.Xml.Xml_Reader
{
  [DebuggerDisplay("{ToString()}")]
  internal struct DebuggerDisplayXmlNodeProxy
  {
    private XmlNode node;

    public DebuggerDisplayXmlNodeProxy(XmlNode node)
    {
      this.node = node;
    }

    public override string ToString()
    {
      XmlNodeType nodeType = node.NodeType;
      string result = nodeType.ToString();
      switch (nodeType)
      {
        case XmlNodeType.Element:
        case XmlNodeType.EntityReference:
          result += ", Name=\"" + node.Name + "\"";
          break;
        case XmlNodeType.Attribute:
        case XmlNodeType.ProcessingInstruction:
          result += ", Name=\"" + node.Name + "\", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(node.Value) + "\"";
          break;
        case XmlNodeType.Text:
        case XmlNodeType.CDATA:
        case XmlNodeType.Comment:
        case XmlNodeType.Whitespace:
        case XmlNodeType.SignificantWhitespace:
        case XmlNodeType.XmlDeclaration:
          result += ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(node.Value) + "\"";
          break;
        case XmlNodeType.DocumentType:
          XmlDocumentType documentType = (XmlDocumentType)node;
          result += ", Name=\"" + documentType.Name + "\", SYSTEM=\"" + documentType.SystemId + "\", PUBLIC=\"" + documentType.PublicId + "\", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(documentType.InternalSubset) + "\"";
          break;
        default:
          break;
      }
      return result;
    }
  }
}