using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlAttributeComparer : IComparer
  {
    public int Compare(object o1, object o2)
    {
      XmlAttribute a1 = (XmlAttribute)o1;
      XmlAttribute a2 = (XmlAttribute)o2;
      int ns = String.Compare(a1.NamespaceURI, a2.NamespaceURI, StringComparison.Ordinal);
      if (ns == 0)
      {
        return String.Compare(a1.Name, a2.Name, StringComparison.Ordinal);
      }
      return ns;
    }
  }
}