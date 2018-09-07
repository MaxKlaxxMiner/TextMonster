namespace TextMonster.Xml.Xml_Reader
{
  //
  //  IncrementalReadDecoder abstract class
  //
  internal abstract class IncrementalReadDecoder
  {
    internal abstract bool IsFull { get; }
    internal abstract int Decode(char[] chars, int startPos, int len);
  }
}
