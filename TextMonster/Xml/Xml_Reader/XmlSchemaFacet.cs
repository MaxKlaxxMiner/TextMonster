using System.ComponentModel;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaFacet.uex' path='docs/doc[@for="XmlSchemaFacet"]/*' />
  public abstract class XmlSchemaFacet : XmlSchemaAnnotated
  {
    string value;
    bool isFixed;
    FacetType facetType;

    /// <include file='doc\XmlSchemaFacet.uex' path='docs/doc[@for="XmlSchemaFacet.Value"]/*' />
    [XmlAttribute("value")]
    public string Value
    {
      get { return this.value; }
      set { this.value = value; }
    }

    /// <include file='doc\XmlSchemaFacet.uex' path='docs/doc[@for="XmlSchemaFacet.IsFixed"]/*' />
    [XmlAttribute("fixed"), DefaultValue(false)]
    public virtual bool IsFixed
    {
      get { return isFixed; }
      set
      {
        if (!(this is XmlSchemaEnumerationFacet) && !(this is XmlSchemaPatternFacet))
        {
          isFixed = value;
        }
      }
    }

    internal FacetType FacetType
    {
      get
      {
        return facetType;
      }
      set
      {
        facetType = value;
      }
    }
  }
}
