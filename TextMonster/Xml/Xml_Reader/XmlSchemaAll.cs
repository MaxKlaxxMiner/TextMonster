namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaAll.uex' path='docs/doc[@for="XmlSchemaAll"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaAll : XmlSchemaGroupBase
  {
    XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();

    /// <include file='doc\XmlSchemaAll.uex' path='docs/doc[@for="XmlSchemaAll.Items"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("element", typeof(XmlSchemaElement))]
    public override XmlSchemaObjectCollection Items
    {
      get { return items; }
    }

    internal override void SetItems(XmlSchemaObjectCollection newItems)
    {
      items = newItems;
    }
  }
}
