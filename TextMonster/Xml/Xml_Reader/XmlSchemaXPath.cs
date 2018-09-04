using System.ComponentModel;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaXPath"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaXPath : XmlSchemaAnnotated
  {
    string xpath;
    /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaXPath.XPath"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("xpath"), DefaultValue("")]
    public string XPath
    {
      get { return xpath; }
      set { xpath = value; }
    }
  }
}
