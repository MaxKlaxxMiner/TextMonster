using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class UnionFacetsChecker : FacetsChecker
  {

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;

      if ((flags & RestrictionFlags.Enumeration) != 0)
      {
        if (!MatchEnumeration(value, restriction.Enumeration, datatype))
        {
          return new XmlSchemaException(Res.Sch_EnumerationConstraintFailed, string.Empty);
        }
      }
      return null;
    }

    internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (datatype.Compare(value, enumeration[i]) == 0)
        { //Compare on Datatype_union will compare two XsdSimpleValue
          return true;
        }
      }
      return false;
    }
  }
}
