using System;
using System.Collections;
using System.Globalization;

namespace TextMonster.Xml.Xml_Reader
{
  class CaseInsensitiveKeyComparer : CaseInsensitiveComparer, IEqualityComparer
  {
    public CaseInsensitiveKeyComparer()
      : base(CultureInfo.CurrentCulture)
    {
    }

    bool IEqualityComparer.Equals(Object x, Object y)
    {
      return (Compare(x, y) == 0);
    }

    int IEqualityComparer.GetHashCode(Object obj)
    {
      string s = obj as string;
      if (s == null)
        throw new ArgumentException(null, "obj");

      return s.ToUpper(CultureInfo.CurrentCulture).GetHashCode();
    }
  }
}