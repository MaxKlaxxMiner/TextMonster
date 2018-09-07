using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlAnyElementAttributes : CollectionBase
  {
    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.Add"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int Add(XmlAnyElementAttribute attribute)
    {
      return List.Add(attribute);
    }
  }
}