using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\SoapAttributeAttribute.uex' path='docs/doc[@for="SoapAttributeAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
  public class SoapAttributeAttribute : System.Attribute
  {
    string attributeName;
    string ns;
    string dataType;

    /// <include file='doc\SoapAttributeAttribute.uex' path='docs/doc[@for="SoapAttributeAttribute.SoapAttributeAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public SoapAttributeAttribute(string attributeName)
    {
      this.attributeName = attributeName;
    }

    /// <include file='doc\SoapAttributeAttribute.uex' path='docs/doc[@for="SoapAttributeAttribute.ElementName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string AttributeName
    {
      get { return attributeName == null ? string.Empty : attributeName; }
      set { attributeName = value; }
    }

    /// <include file='doc\SoapAttributeAttribute.uex' path='docs/doc[@for="SoapAttributeAttribute.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Namespace
    {
      get { return ns; }
      set { ns = value; }
    }

    /// <include file='doc\SoapAttributeAttribute.uex' path='docs/doc[@for="SoapAttributeAttribute.DataType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string DataType
    {
      get { return dataType == null ? string.Empty : dataType; }
      set { dataType = value; }
    }
  }
}