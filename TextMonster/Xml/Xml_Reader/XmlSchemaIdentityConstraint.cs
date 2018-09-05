namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaIdentityConstraint"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaIdentityConstraint : XmlSchemaAnnotated
  {
    string name;
    XmlSchemaXPath selector;
    XmlSchemaObjectCollection fields = new XmlSchemaObjectCollection();
    XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;
    CompiledIdentityConstraint compiledConstraint = null;

    /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaIdentityConstraint.Name"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("name")]
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaIdentityConstraint.Selector"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("selector", typeof(XmlSchemaXPath))]
    public XmlSchemaXPath Selector
    {
      get { return selector; }
      set { selector = value; }
    }

    /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaIdentityConstraint.Fields"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("field", typeof(XmlSchemaXPath))]
    public XmlSchemaObjectCollection Fields
    {
      get { return fields; }
    }

    /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaIdentityConstraint.QualifiedName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlQualifiedName QualifiedName
    {
      get { return qualifiedName; }
    }

    internal void SetQualifiedName(XmlQualifiedName value)
    {
      qualifiedName = value;
    }

    [XmlIgnore]
    internal override string NameAttribute
    {
      get { return Name; }
      set { Name = value; }
    }
  }
}
