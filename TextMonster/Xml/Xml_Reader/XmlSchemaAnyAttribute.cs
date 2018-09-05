using System.ComponentModel;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaAnyAttribute.uex' path='docs/doc[@for="XmlSchemaAnyAttribute"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaAnyAttribute : XmlSchemaAnnotated
  {
    string ns;
    XmlSchemaContentProcessing processContents = XmlSchemaContentProcessing.None;
    NamespaceList namespaceList;

    /// <include file='doc\XmlSchemaAnyAttribute.uex' path='docs/doc[@for="XmlSchemaAnyAttribute.Namespaces"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("namespace")]
    public string Namespace
    {
      set { ns = value; }
    }

    /// <include file='doc\XmlSchemaAnyAttribute.uex' path='docs/doc[@for="XmlSchemaAnyAttribute.ProcessContents"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("processContents"), DefaultValue(XmlSchemaContentProcessing.None)]
    public XmlSchemaContentProcessing ProcessContents
    {
      get { return processContents; }
      set { processContents = value; }
    }


    [XmlIgnore]
    internal NamespaceList NamespaceList
    {
      get { return namespaceList; }
    }

    [XmlIgnore]
    internal XmlSchemaContentProcessing ProcessContentsCorrect
    {
      get { return processContents == XmlSchemaContentProcessing.None ? XmlSchemaContentProcessing.Strict : processContents; }
    }

    internal void BuildNamespaceList(string targetNamespace)
    {
      if (ns != null)
      {
        namespaceList = new NamespaceList(ns, targetNamespace);
      }
      else
      {
        namespaceList = new NamespaceList();
      }
    }
  }
}
