using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  class Positions
  {
    ArrayList positions = new ArrayList();

    public int Add(int symbol, object particle)
    {
      return positions.Add(new Position(symbol, particle));
    }

    public Position this[int pos]
    {
      get { return (Position)positions[pos]; }
    }

    public int Count
    {
      get { return positions.Count; }
    }
  }
}
