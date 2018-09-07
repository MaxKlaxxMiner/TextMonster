namespace TextMonster.Xml.Xml_Reader
{
  internal class IncrementalReadDummyDecoder : IncrementalReadDecoder
  {
    internal override bool IsFull { get { return false; } }
    internal override int Decode(char[] chars, int startPos, int len) { return len; }
  }
}