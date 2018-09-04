namespace TextMonster.Xml.XmlReader
{
  public enum XmlOutputMethod
  {
    Xml = 0,    // Use Xml 1.0 rules to serialize
    Html = 1,    // Use Html rules specified by Xslt specification to serialize
    Text = 2,    // Only serialize text blocks
    AutoDetect = 3,    // Choose between Xml and Html output methods at runtime (using Xslt rules to do so)
  }
}
