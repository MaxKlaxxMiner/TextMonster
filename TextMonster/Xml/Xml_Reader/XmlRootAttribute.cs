using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlRootAttribute.uex' path='docs/doc[@for="XmlRootAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
  public class XmlRootAttribute : Attribute
  {
    string elementName;
    string ns;
    string dataType;
    bool nullable = true;
    bool nullableSpecified;

    /// <include file='doc\XmlRootAttribute.uex' path='docs/doc[@for="XmlRootAttribute.XmlRootAttribute1"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlRootAttribute(string elementName)
    {
      this.elementName = elementName;
    }

    /// <include file='doc\XmlRootAttribute.uex' path='docs/doc[@for="XmlRootAttribute.Namespace"]/*' />
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
