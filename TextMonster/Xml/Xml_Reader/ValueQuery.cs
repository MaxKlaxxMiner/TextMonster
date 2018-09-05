namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class ValueQuery : Query
  {
    public ValueQuery() { }
    protected ValueQuery(ValueQuery other) : base(other) { }
    public sealed override void Reset() { }
    public sealed override XPathNavigator Current { get { throw XPathException.Create(Res.Xp_NodeSetExpected); } }
    public sealed override int CurrentPosition { get { throw XPathException.Create(Res.Xp_NodeSetExpected); } }
    public sealed override int Count { get { throw XPathException.Create(Res.Xp_NodeSetExpected); } }
    public sealed override XPathNavigator Advance() { throw XPathException.Create(Res.Xp_NodeSetExpected); }
  }
}
