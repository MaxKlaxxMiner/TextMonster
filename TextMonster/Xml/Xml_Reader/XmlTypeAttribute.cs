using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlTypeAttribute.uex' path='docs/doc[@for="XmlTypeAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
  public class XmlTypeAttribute : System.Attribute
  {
    bool includeInSchema = true;
    bool anonymousType;
    string ns;
    string typeName;

    /// <include file='doc\XmlTypeAttribute.uex' path='docs/doc[@for="XmlTypeAttribute.XmlTypeAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlTypeAttribute(string typeName)
    {
      this.typeName = typeName;
    }
  }
}