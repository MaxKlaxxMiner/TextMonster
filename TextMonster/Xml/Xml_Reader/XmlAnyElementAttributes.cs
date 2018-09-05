using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlAnyElementAttributes : CollectionBase
  {

    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.this"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlAnyElementAttribute this[int index]
    {
      get { return (XmlAnyElementAttribute)List[index]; }
    }

    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.Add"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int Add(XmlAnyElementAttribute attribute)
    {
      return List.Add(attribute);
    }

    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.Insert"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Insert(int index, XmlAnyElementAttribute attribute)
    {
      List.Insert(index, attribute);
    }

    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.IndexOf"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int IndexOf(XmlAnyElementAttribute attribute)
    {
      return List.IndexOf(attribute);
    }

    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.Contains"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool Contains(XmlAnyElementAttribute attribute)
    {
      return List.Contains(attribute);
    }

    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.Remove"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Remove(XmlAnyElementAttribute attribute)
    {
      List.Remove(attribute);
    }

    /// <include file='doc\XmlAnyElementAttributes.uex' path='docs/doc[@for="XmlAnyElementAttributes.CopyTo"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void CopyTo(XmlAnyElementAttribute[] array, int index)
    {
      List.CopyTo(array, index);
    }
  }
}