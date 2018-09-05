﻿using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
  public class XmlAttributeAttribute : System.Attribute
  {
    string attributeName;
    Type type;
    string ns;
    string dataType;
    XmlSchemaForm form = XmlSchemaForm.None;

    /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute.XmlAttributeAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlAttributeAttribute()
    {
    }

    /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute.XmlAttributeAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlAttributeAttribute(string attributeName)
    {
      this.attributeName = attributeName;
    }

    /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute.Type"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public Type Type
    {
      get { return type; }
    }

    /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute.AttributeName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string AttributeName
    {
      get { return attributeName == null ? string.Empty : attributeName; }
    }

    /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Namespace
    {
      get { return ns; }
    }

    /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute.DataType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string DataType
    {
      get { return dataType == null ? string.Empty : dataType; }
      set { dataType = value; }
    }

    /// <include file='doc\XmlAttributeAttribute.uex' path='docs/doc[@for="XmlAttributeAttribute.Form"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSchemaForm Form
    {
      get { return form; }
      set { form = value; }
    }
  }
}
