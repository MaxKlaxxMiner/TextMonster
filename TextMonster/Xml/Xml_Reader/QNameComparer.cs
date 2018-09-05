using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class QNameComparer : IComparer
  {
    public int Compare(object o1, object o2)
    {
      XmlQualifiedName qn1 = (XmlQualifiedName)o1;
      XmlQualifiedName qn2 = (XmlQualifiedName)o2;
      int ns = String.Compare(qn1.Namespace, qn2.Namespace, StringComparison.Ordinal);
      if (ns == 0)
      {
        return String.Compare(qn1.Name, qn2.Name, StringComparison.Ordinal);
      }
      return ns;
    }
  }
}