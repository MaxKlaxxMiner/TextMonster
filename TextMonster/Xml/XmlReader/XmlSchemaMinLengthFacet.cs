namespace TextMonster.Xml.XmlReader
{
  /// <include file='doc\XmlSchemaFacet.uex' path='docs/doc[@for="XmlSchemaMinLengthFacet"]/*' />
  public class XmlSchemaMinLengthFacet : XmlSchemaNumericFacet
  {
    public XmlSchemaMinLengthFacet()
    {
      FacetType = FacetType.MinLength;
    }
  }
}
