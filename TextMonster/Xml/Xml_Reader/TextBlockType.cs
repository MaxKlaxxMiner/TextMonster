namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Although the XPath data model does not differentiate between text and whitespace, Managed Xml 1.0
  /// does.  Therefore, when building from an XmlReader, we must preserve these designations in order
  /// to remain backwards-compatible.
  /// </summary>
  internal enum TextBlockType
  {
    None = 0,
    Text = XPathNodeType.Text,
    SignificantWhitespace = XPathNodeType.SignificantWhitespace,
    Whitespace = XPathNodeType.Whitespace,
  };
}
