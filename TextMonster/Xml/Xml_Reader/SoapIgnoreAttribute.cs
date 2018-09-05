using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\SoapIgnoreAttribute.uex' path='docs/doc[@for="SoapIgnoreAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
  public class SoapIgnoreAttribute : System.Attribute
  {
    /// <include file='doc\SoapIgnoreAttribute.uex' path='docs/doc[@for="SoapIgnoreAttribute.SoapIgnoreAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public SoapIgnoreAttribute()
    {
    }
  }
}