namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class ResetableIterator : XPathNodeIterator
  {
    // the best place for this constructors to be is XPathNodeIterator, to avoid DCR at this time let's ground them here
    public ResetableIterator()
    {
      base.count = -1;
    }
    protected ResetableIterator(ResetableIterator other)
    {
      base.count = other.count;
    }

    public abstract void Reset();

    // Contruct extension: CurrentPosition should return 0 if MoveNext() wasn't called after Reset()
    // (behavior is not defined for XPathNodeIterator)
    public abstract override int CurrentPosition { get; }
  }
}
