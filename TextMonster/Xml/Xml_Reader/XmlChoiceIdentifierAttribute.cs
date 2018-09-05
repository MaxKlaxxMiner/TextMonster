using System;
using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
  public class XmlChoiceIdentifierAttribute : System.Attribute
  {
    string name;
    MemberInfo memberInfo;

    /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute.XmlChoiceIdentifierAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlChoiceIdentifierAttribute()
    {
    }

    /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute.XmlChoiceIdentifierAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlChoiceIdentifierAttribute(string name)
    {
      this.name = name;
    }

    /// <include file='doc\XmlChoiceIdentifierAttribute.uex' path='docs/doc[@for="XmlChoiceIdentifierAttribute.Name"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string MemberName
    {
      get { return name == null ? string.Empty : name; }
      set { name = value; }
    }

    internal MemberInfo MemberInfo
    {
      get { return memberInfo; }
      set { memberInfo = value; }
    }
  }
}