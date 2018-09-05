namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\SoapAttributes.uex' path='docs/doc[@for="SoapAttributes"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class SoapAttributes
  {
    bool soapIgnore;
    SoapTypeAttribute soapType;
    SoapElementAttribute soapElement;
    SoapAttributeAttribute soapAttribute;
    SoapEnumAttribute soapEnum;
    object soapDefaultValue = null;

    /// <include file='doc\SoapAttributes.uex' path='docs/doc[@for="SoapAttributes.SoapAttributes"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public SoapAttributes()
    {
    }

    /// <include file='doc\SoapAttributes.uex' path='docs/doc[@for="SoapAttributes.SoapElement"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public SoapElementAttribute SoapElement
    {
      get { return soapElement; }
      set { soapElement = value; }
    }

    /// <include file='doc\SoapAttributes.uex' path='docs/doc[@for="SoapAttributes.SoapAttribute"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public SoapAttributeAttribute SoapAttribute
    {
      get { return soapAttribute; }
      set { soapAttribute = value; }
    }

    /// <include file='doc\SoapAttributes.uex' path='docs/doc[@for="SoapAttributes.SoapDefaultValue"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public object SoapDefaultValue
    {
      get { return soapDefaultValue; }
      set { soapDefaultValue = value; }
    }
  }
}