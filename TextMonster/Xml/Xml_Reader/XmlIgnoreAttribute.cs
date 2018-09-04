using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlIgnoreAttribute.uex' path='docs/doc[@for="XmlIgnoreAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
  public class XmlIgnoreAttribute : System.Attribute
  {
    /// <include file='doc\XmlIgnoreAttribute.uex' path='docs/doc[@for="XmlIgnoreAttribute.XmlIgnoreAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlIgnoreAttribute()
    {
    }
  }
}
