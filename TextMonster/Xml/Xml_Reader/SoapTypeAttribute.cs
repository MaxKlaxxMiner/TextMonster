using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\SoapTypeAttribute.uex' path='docs/doc[@for="SoapTypeAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
  public class SoapTypeAttribute : System.Attribute
  {
    string ns;
    string typeName;
    bool includeInSchema = true;

    /// <include file='doc\SoapTypeAttribute.uex' path='docs/doc[@for="SoapTypeAttribute.SoapTypeAttribute2"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public SoapTypeAttribute(string typeName, string ns)
    {
      this.typeName = typeName;
      this.ns = ns;
    }

    /// <include file='doc\SoapTypeAttribute.uex' path='docs/doc[@for="SoapTypeAttribute.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Namespace
    {
      get { return ns; }
      set { ns = value; }
    }
  }
}