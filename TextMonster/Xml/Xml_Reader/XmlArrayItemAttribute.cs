using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
  public class XmlArrayItemAttribute : System.Attribute
  {
    string elementName;
    Type type;
    string ns;
    string dataType;
    bool nullable;
    bool nullableSpecified = false;
    XmlSchemaForm form = XmlSchemaForm.None;
    int nestingLevel;

    /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute.Type"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public Type Type
    {
      get { return type; }
    }

    /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute.ElementName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string ElementName
    {
      get { return elementName == null ? string.Empty : elementName; }
    }

    /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Namespace
    {
      get { return ns; }
    }

    /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute.NestingLevel"]/*' />
    public int NestingLevel
    {
      get { return nestingLevel; }
      set { nestingLevel = value; }
    }

    /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute.DataType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string DataType
    {
      get { return dataType == null ? string.Empty : dataType; }
    }

    /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute.IsNullable"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool IsNullable
    {
      get { return nullable; }
    }

    internal bool IsNullableSpecified
    {
      get { return nullableSpecified; }
    }

    /// <include file='doc\XmlArrayItemAttribute.uex' path='docs/doc[@for="XmlArrayItemAttribute.Form"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSchemaForm Form
    {
      get { return form; }
    }
  }
}