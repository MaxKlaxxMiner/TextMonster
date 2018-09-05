using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\SoapEnumAttribute.uex' path='docs/doc[@for="SoapEnumAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field)]
  public class SoapEnumAttribute : System.Attribute
  {
    string name;

    /// <include file='doc\SoapEnumAttribute.uex' path='docs/doc[@for="SoapEnumAttribute.SoapEnumAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public SoapEnumAttribute(string name)
    {
      this.name = name;
    }

    /// <include file='doc\SoapEnumAttribute.uex' path='docs/doc[@for="SoapEnumAttribute.Name"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Name
    {
      get { return name == null ? string.Empty : name; }
      set { name = value; }
    }
  }
}