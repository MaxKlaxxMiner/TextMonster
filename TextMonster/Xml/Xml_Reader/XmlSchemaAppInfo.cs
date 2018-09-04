namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaAppInfo.uex' path='docs/doc[@for="XmlSchemaAppInfo"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaAppInfo : XmlSchemaObject
  {
    string source;
    XmlNode[] markup;

    /// <include file='doc\XmlSchemaAppInfo.uex' path='docs/doc[@for="XmlSchemaAppInfo.Source"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("source", DataType = "anyURI")]
    public string Source
    {
      get { return source; }
      set { source = value; }
    }

    /// <include file='doc\XmlSchemaAppInfo.uex' path='docs/doc[@for="XmlSchemaAppInfo.Markup"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlText(), XmlAnyElement]
    public XmlNode[] Markup
    {
      get { return markup; }
      set { markup = value; }
    }
  }
}
