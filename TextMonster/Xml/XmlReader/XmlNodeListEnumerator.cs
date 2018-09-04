using System.Collections;

namespace TextMonster.Xml.XmlReader
{
  internal class XmlNodeListEnumerator : IEnumerator
  {
    XPathNodeList list;
    int index;
    bool valid;

    public XmlNodeListEnumerator(XPathNodeList list)
    {
      this.list = list;
      this.index = -1;
      this.valid = false;
    }

    public void Reset()
    {
      index = -1;
    }

    public bool MoveNext()
    {
      index++;
      int count = list.ReadUntil(index + 1);   // read past for delete-node case
      if (count - 1 < index)
      {
        return false;
      }
      valid = (list[index] != null);
      return valid;
    }

    public object Current
    {
      get
      {
        if (valid)
        {
          return list[index];
        }
        return null;
      }
    }
  }
}
