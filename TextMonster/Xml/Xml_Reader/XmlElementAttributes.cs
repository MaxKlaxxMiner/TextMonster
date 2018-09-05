using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlElementAttributes : CollectionBase
  {

    /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes.this"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlElementAttribute this[int index]
    {
      get { return (XmlElementAttribute)List[index]; }
    }

    /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes.Add"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int Add(XmlElementAttribute attribute)
    {
      return List.Add(attribute);
    }
  }
}