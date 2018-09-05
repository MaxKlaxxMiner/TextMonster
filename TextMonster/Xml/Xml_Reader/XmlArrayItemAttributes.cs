using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlArrayItemAttributes.uex' path='docs/doc[@for="XmlArrayItemAttributes"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlArrayItemAttributes : CollectionBase
  {

    /// <include file='doc\XmlArrayItemAttributes.uex' path='docs/doc[@for="XmlArrayItemAttributes.this"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlArrayItemAttribute this[int index]
    {
      get { return (XmlArrayItemAttribute)List[index]; }
    }

    /// <include file='doc\XmlArrayItemAttributes.uex' path='docs/doc[@for="XmlArrayItemAttributes.Add"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int Add(XmlArrayItemAttribute attribute)
    {
      return List.Add(attribute);
    }
  }
}