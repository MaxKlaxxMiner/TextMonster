using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class SortKey
  {
    private Int32 numKeys;
    private object[] keys;
    private int originalPosition;
    private XPathNavigator node;

    public SortKey(int numKeys, int originalPosition, XPathNavigator node)
    {
      this.numKeys = numKeys;
      this.keys = new object[numKeys];
      this.originalPosition = originalPosition;
      this.node = node;
    }

    public int NumKeys { get { return this.numKeys; } }
    public int OriginalPosition { get { return this.originalPosition; } }
  }
}