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

    /// <include file='doc\XmlAttributeOverrides.uex' path='docs/doc[@for="XmlAttributeOverrides.Add1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Add(Type type, string member, XmlAttributes attributes)
    {
      Hashtable members = (Hashtable)types[type];
      if (members == null)
      {
        members = new Hashtable();
        types.Add(type, members);
      }
      else if (members[member] != null)
      {
        throw new InvalidOperationException(Res.GetString(Res.XmlAttributeSetAgain, type.FullName, member));
      }
      members.Add(member, attributes);
    }

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