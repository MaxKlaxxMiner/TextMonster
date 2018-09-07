﻿using System.ComponentModel;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaAttribute : XmlSchemaAnnotated
  {
    string defaultValue;
    string fixedValue;
    string name;

    XmlSchemaForm form = XmlSchemaForm.None;
    XmlSchemaUse use = XmlSchemaUse.None;

    XmlQualifiedName refName = XmlQualifiedName.Empty;
    XmlQualifiedName typeName = XmlQualifiedName.Empty;
    XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;

    XmlSchemaSimpleType type;
    XmlSchemaSimpleType attributeType;

    SchemaAttDef attDef;

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.DefaultValue"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("default")]
    [DefaultValue(null)]
    public string DefaultValue
    {
      get { return defaultValue; }
      set { defaultValue = value; }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.FixedValue"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("fixed")]
    [DefaultValue(null)]
    public string FixedValue
    {
      get { return fixedValue; }
      set { fixedValue = value; }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.Form"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("form"), DefaultValue(XmlSchemaForm.None)]
    public XmlSchemaForm Form
    {
      get { return form; }
      set { form = value; }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.Name"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("name")]
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.RefName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("ref")]
    public XmlQualifiedName RefName
    {
      get { return refName; }
      set { refName = (value == null ? XmlQualifiedName.Empty : value); }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.SchemaTypeName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("type")]
    public XmlQualifiedName SchemaTypeName
    {
      get { return typeName; }
      set { typeName = (value == null ? XmlQualifiedName.Empty : value); }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.SchemaType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("simpleType")]
    public XmlSchemaSimpleType SchemaType
    {
      get { return type; }
      set { type = value; }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.Use"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("use"), DefaultValue(XmlSchemaUse.None)]
    public XmlSchemaUse Use
    {
      get { return use; }
      set { use = value; }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.QualifiedName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlQualifiedName QualifiedName
    {
      get { return qualifiedName; }
    }

    /// <include file='doc\XmlSchemaAttribute.uex' path='docs/doc[@for="XmlSchemaAttribute.AttributeSchemaType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public XmlSchemaSimpleType AttributeSchemaType
    {
      get { return attributeType; }
    }

    [XmlIgnore]
    internal XmlSchemaDatatype Datatype
    {
      get
      {
        if (attributeType != null)
        {
          return attributeType.Datatype;
        }
        return null;
      }
    }

    internal void SetQualifiedName(XmlQualifiedName value)
    {
      qualifiedName = value;
    }

    internal SchemaAttDef AttDef
    {
      get { return attDef; }
    }

    internal bool HasDefault
    {
      get { return defaultValue != null; }
    }

    [XmlIgnore]
    internal override string NameAttribute
    {
      get { return Name; }
      set { Name = value; }
    }

    internal override XmlSchemaObject Clone()
    {
      XmlSchemaAttribute newAtt = (XmlSchemaAttribute)MemberwiseClone();

      //Deep clone the QNames as these will be updated on chameleon includes
      newAtt.refName = this.refName.Clone();
      newAtt.typeName = this.typeName.Clone();
      newAtt.qualifiedName = this.qualifiedName.Clone();
      return newAtt;
    }
  }
}
