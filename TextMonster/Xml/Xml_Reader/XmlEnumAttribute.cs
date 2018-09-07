using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlEnumAttribute.uex' path='docs/doc[@for="XmlEnumAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field)]
  public class XmlEnumAttribute : System.Attribute
  {
    string name;

    /// <include file='doc\XmlEnumAttribute.uex' path='docs/doc[@for="XmlEnumAttribute.XmlEnumAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlEnumAttribute(string name)
    {
      this.name = name;
    }
  }
}
