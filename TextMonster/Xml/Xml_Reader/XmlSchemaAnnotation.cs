﻿namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaAnnotation.uex' path='docs/doc[@for="XmlSchemaAnnotation"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaAnnotation : XmlSchemaObject
  {
    string id;
    XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();
    XmlAttribute[] moreAttributes;

    /// <include file='doc\XmlSchemaAnnotation.uex' path='docs/doc[@for="XmlSchemaAnnotation.Id"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("id", DataType = "ID")]
    public string Id
    {
      get { return id; }
      set { id = value; }
    }

    /// <include file='doc\XmlSchemaAnnotation.uex' path='docs/doc[@for="XmlSchemaAnnotation.Items"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("documentation", typeof(XmlSchemaDocumentation)),
     XmlElement("appinfo", typeof(XmlSchemaAppInfo))]
    public XmlSchemaObjectCollection Items
    {
      get { return items; }
    }

    /// <include file='doc\XmlSchemaAnnotation.uex' path='docs/doc[@for="XmlSchemaAnnotation.UnhandledAttributes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAnyAttribute]
    public XmlAttribute[] UnhandledAttributes
    {
      get { return moreAttributes; }
    }

    [XmlIgnore]
    internal override string IdAttribute
    {
      get { return Id; }
      set { Id = value; }
    }

    internal override void SetUnhandledAttributes(XmlAttribute[] moreAttributes)
    {
      this.moreAttributes = moreAttributes;
    }
  }
}
