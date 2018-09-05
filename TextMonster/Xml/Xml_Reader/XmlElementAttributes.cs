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

    /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes.Insert"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Insert(int index, XmlElementAttribute attribute)
    {
      List.Insert(index, attribute);
    }

    /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes.IndexOf"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int IndexOf(XmlElementAttribute attribute)
    {
      return List.IndexOf(attribute);
    }

    /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes.Contains"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool Contains(XmlElementAttribute attribute)
    {
      return List.Contains(attribute);
    }

    /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes.Remove"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Remove(XmlElementAttribute attribute)
    {
      List.Remove(attribute);
    }

    /// <include file='doc\XmlElementAttributes.uex' path='docs/doc[@for="XmlElementAttributes.CopyTo"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void CopyTo(XmlElementAttribute[] array, int index)
    {
      List.CopyTo(array, index);
    }

  }
}