namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_nonNegativeInteger : Datatype_integer
  {
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(decimal.Zero, decimal.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.NonNegativeInteger; } }

    internal override bool HasValueFacets
    {
      get
      {
        return true; //Built-in facet to check range
      }
    }
  }
}
