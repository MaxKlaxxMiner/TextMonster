﻿namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaImport : XmlSchemaExternal
  {
    string ns;
    XmlSchemaAnnotation annotation;

    /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport.XmlSchemaImport"]/*' />
    public XmlSchemaImport()
    {
      Compositor = Compositor.Import;
    }

    /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport.Namespace"]/*' />
    [XmlAttribute("namespace", DataType = "anyURI")]
    public string Namespace
    {
      get { return ns; }
      set { ns = value; }
    }

    /// <include file='doc\XmlSchemaImport.uex' path='docs/doc[@for="XmlSchemaImport.Annotation"]/*' />
    [XmlElement("annotation", typeof(XmlSchemaAnnotation))]
    public XmlSchemaAnnotation Annotation
    {
      get { return annotation; }
      set { annotation = value; }
    }

    internal override void AddAnnotation(XmlSchemaAnnotation annotation)
    {
      this.annotation = annotation;
    }
  }
}
