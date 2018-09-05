namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class XPathEmptyIterator : ResetableIterator
  {
    private XPathEmptyIterator() { }
    public override XPathNodeIterator Clone() { return this; }

    public override XPathNavigator Current
    {
      get { return null; }
    }

    public override int CurrentPosition
    {
      get { return 0; }
    }

    public override int Count
    {
      get { return 0; }
    }

    public override bool MoveNext()
    {
      return false;
    }

    public override void Reset() { }

    // -- Instance
    public static XPathEmptyIterator Instance = new XPathEmptyIterator();
  }
}