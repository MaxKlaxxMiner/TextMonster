﻿namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaSimpleContentExtension : XmlSchemaContent
  {
    XmlSchemaObjectCollection attributes = new XmlSchemaObjectCollection();
    XmlSchemaAnyAttribute anyAttribute;
    XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;

    /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension.BaseTypeName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("base")]
    public XmlQualifiedName BaseTypeName
    {
      get { return baseTypeName; }
      set { baseTypeName = (value == null ? XmlQualifiedName.Empty : value); }
    }

    /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension.Attributes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("attribute", typeof(XmlSchemaAttribute)),
     XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
    public XmlSchemaObjectCollection Attributes
    {
      get { return attributes; }
    }

    /// <include file='doc\XmlSchemaSimpleContentExtension.uex' path='docs/doc[@for="XmlSchemaSimpleContentExtension.AnyAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("anyAttribute")]
    public XmlSchemaAnyAttribute AnyAttribute
    {
      get { return anyAttribute; }
      set { anyAttribute = value; }
    }

    internal void SetAttributes(XmlSchemaObjectCollection newAttributes)
    {
      attributes = newAttributes;
    }
  }
}
