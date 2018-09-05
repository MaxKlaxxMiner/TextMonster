namespace TextMonster.Xml.Xml_Reader
{
  internal class IteratorFilter : XPathNodeIterator
  {
    private XPathNodeIterator innerIterator;
    private string name;
    private int position = 0;

    internal IteratorFilter(XPathNodeIterator innerIterator, string name)
    {
      this.innerIterator = innerIterator;
      this.name = name;
    }

    private IteratorFilter(IteratorFilter it)
    {
      this.innerIterator = it.innerIterator.Clone();
      this.name = it.name;
      this.position = it.position;
    }

    public override XPathNodeIterator Clone() { return new IteratorFilter(this); }
    public override XPathNavigator Current { get { return innerIterator.Current; } }
    public override int CurrentPosition { get { return this.position; } }

    public override bool MoveNext()
    {
      while (innerIterator.MoveNext())
      {
        if (innerIterator.Current.LocalName == this.name)
        {
          this.position++;
          return true;
        }
      }
      return false;
    }
  }
}