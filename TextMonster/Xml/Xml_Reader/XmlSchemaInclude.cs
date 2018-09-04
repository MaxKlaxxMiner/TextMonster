﻿namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaInclude.uex' path='docs/doc[@for="XmlSchemaInclude"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaInclude : XmlSchemaExternal
  {
    XmlSchemaAnnotation annotation;

    /// <include file='doc\XmlSchemaInclude.uex' path='docs/doc[@for="XmlSchemaInclude.XmlSchemaInclude"]/*' />
    public XmlSchemaInclude()
    {
      Compositor = Compositor.Include;
    }
    /// <include file='doc\XmlSchemaInclude.uex' path='docs/doc[@for="XmlSchemaInclude.Annotation"]/*' />
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
