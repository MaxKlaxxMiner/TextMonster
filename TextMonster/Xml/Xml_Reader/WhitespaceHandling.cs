namespace TextMonster.Xml.Xml_Reader
{
  public enum WhitespaceHandling
  {
    // Return all Whitespace and SignificantWhitespace nodes. This is the default.
    All = 0,

    // Return just SignificantWhitespace, i.e. whitespace nodes that are in scope of xml:space="preserve"
    Significant = 1,

    // Do not return any Whitespace or SignificantWhitespace nodes.
    None = 2
  }
}