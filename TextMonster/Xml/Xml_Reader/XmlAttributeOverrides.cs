using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlAttributeOverrides.uex' path='docs/doc[@for="XmlAttributeOverrides"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlAttributeOverrides
  {
    Hashtable types = new Hashtable();

    /// <include file='doc\XmlAttributeOverrides.uex' path='docs/doc[@for="XmlAttributeOverrides.this"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlAttributes this[Type type]
    {
      get
      {
        return this[type, string.Empty];
      }
    }

    /// <include file='doc\XmlAttributeOverrides.uex' path='docs/doc[@for="XmlAttributeOverrides.this1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlAttributes this[Type type, string member]
    {
      get
      {
        Hashtable members = (Hashtable)types[type];
        if (members == null) return null;
        return (XmlAttributes)members[member];
      }
    }
  }
}