using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  public class XmlSchemaEnumerator : IEnumerator<XmlSchema>
  {
    private XmlSchemas list;
    private int idx, end;

    public XmlSchemaEnumerator(XmlSchemas list)
    {
      this.list = list;
      this.idx = -1;
      this.end = list.Count - 1;
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
      if (this.idx >= this.end)
        return false;

      this.idx++;
      return true;
    }

    public XmlSchema Current
    {
      get { return this.list[this.idx]; }
    }

    object System.Collections.IEnumerator.Current
    {
      get { return this.list[this.idx]; }
    }

    void System.Collections.IEnumerator.Reset()
    {
      this.idx = -1;
    }
  }
}