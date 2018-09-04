namespace TextMonster.Xml.XmlReader
{
  /// <include file='doc\XmlSchemaFacet.uex' path='docs/doc[@for="XmlSchemaMaxLengthFacet"]/*' />
  public class XmlSchemaMaxLengthFacet : XmlSchemaNumericFacet
  {
    public XmlSchemaMaxLengthFacet()
    {
      FacetType = FacetType.MaxLength;
    }
  }
}
