namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_negativeInteger : Datatype_nonPositiveInteger
  {
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(decimal.MinValue, decimal.MinusOne);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.NegativeInteger; } }
  }
}
