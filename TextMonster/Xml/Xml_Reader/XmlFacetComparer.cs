using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlFacetComparer : IComparer
  {
    public int Compare(object o1, object o2)
    {
      XmlSchemaFacet f1 = (XmlSchemaFacet)o1;
      XmlSchemaFacet f2 = (XmlSchemaFacet)o2;
      return String.Compare(f1.GetType().Name + ":" + f1.Value, f2.GetType().Name + ":" + f2.Value, StringComparison.Ordinal);
    }
  }
}
