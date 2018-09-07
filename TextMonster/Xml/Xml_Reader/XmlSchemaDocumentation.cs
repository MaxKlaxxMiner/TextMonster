namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaDocumentation : XmlSchemaObject
  {
    string source;
    string language;
    XmlNode[] markup;
    static XmlSchemaSimpleType languageType = DatatypeImplementation.GetSimpleTypeFromXsdType(new XmlQualifiedName("language", XmlReservedNs.NsXs));

    /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation.Source"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("source", DataType = "anyURI")]
    public string Source
    {
      get { return source; }
      set { source = value; }
    }

    /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation.Language"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("xml:lang")]
    public string Language
    {
      get { return language; }
      set { language = (string)languageType.Datatype.ParseValue(value, (NameTable)null, (IXmlNamespaceResolver)null); }
    }

    /// <include file='doc\XmlSchemaDocumentation.uex' path='docs/doc[@for="XmlSchemaDocumentation.Markup"]/*' />
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
