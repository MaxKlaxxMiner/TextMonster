namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\SoapSchemaMember.uex' path='docs/doc[@for="SoapSchemaMember"]/*' />
  /// <internalonly/>
  public class SoapSchemaMember
  {
    string memberName;
    XmlQualifiedName type = XmlQualifiedName.Empty;

    /// <include file='doc\SoapSchemaMember.uex' path='docs/doc[@for="SoapSchemaMember.MemberType"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlQualifiedName MemberType
    {
      get { return type; }
    }

    /// <include file='doc\SoapSchemaMember.uex' path='docs/doc[@for="SoapSchemaMember.MemberName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string MemberName
    {
      get { return memberName == null ? string.Empty : memberName; }
    }
  }
}