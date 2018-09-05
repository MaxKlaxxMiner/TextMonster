namespace TextMonster.Xml.Xml_Reader
{
  internal class Ucs4Encoding2143 : Ucs4Encoding
  {
    public Ucs4Encoding2143()
    {
      ucs4Decoder = new Ucs4Decoder2143();
    }

    public override string EncodingName
    {
      get
      {
        return "ucs-4 (order 2143)";
      }
    }
    public override byte[] GetPreamble()
    {
      return new byte[4] { 0x00, 0x00, 0xff, 0xfe };
    }
  }
}