namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_positiveInteger : Datatype_nonNegativeInteger
  {
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(decimal.One, decimal.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.PositiveInteger; } }
  }
}
