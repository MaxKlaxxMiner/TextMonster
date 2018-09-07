using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlElementAttribute.uex' path='docs/doc[@for="XmlElementAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
  public class XmlElementAttribute : System.Attribute
  {
    string elementName;
    Type type;
    string ns;
    string dataType;
    bool nullable;
    bool nullableSpecified;
    XmlSchemaForm form = XmlSchemaForm.None;
    int order = -1;

    /// <include file='doc\XmlElementAttribute.uex' path='docs/doc[@for="XmlElementAttribute.XmlElementAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlElementAttribute(string elementName)
    {
      this.elementName = elementName;
    }

    /// <include file='doc\XmlElementAttribute.uex' path='docs/doc[@for="XmlElementAttribute.XmlElementAttribute3"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlElementAttribute(string elementName, Type type)
    {
      this.elementName = elementName;
      this.type = type;
    }
  }
}
