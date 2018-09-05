namespace TextMonster.Xml.Xml_Reader
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

    internal override void AddAnnotation(XmlSchemaAnnotation annotation)
    {
      this.annotation = annotation;
    }
  }
}
