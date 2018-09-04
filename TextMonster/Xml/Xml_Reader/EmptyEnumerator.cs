using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class EmptyEnumerator : IEnumerator
  {

    bool IEnumerator.MoveNext()
    {
      return false;
    }

    void IEnumerator.Reset()
    {
    }

    object IEnumerator.Current
    {
      get
      {
        throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
      }
    }
  }
}
