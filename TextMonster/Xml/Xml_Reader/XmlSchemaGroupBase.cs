namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaGroupbase.uex' path='docs/doc[@for="XmlSchemaGroupBase"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public abstract class XmlSchemaGroupBase : XmlSchemaParticle
  {
    /// <include file='doc\XmlSchemaGroupbase.uex' path='docs/doc[@for="XmlSchemaGroupBase.Items"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public abstract XmlSchemaObjectCollection Items { get; }

    internal abstract void SetItems(XmlSchemaObjectCollection newItems);
  }
}
