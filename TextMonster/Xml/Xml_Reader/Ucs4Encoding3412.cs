namespace TextMonster.Xml.Xml_Reader
{
  internal class Ucs4Encoding3412 : Ucs4Encoding
  {
    public Ucs4Encoding3412()
    {
      ucs4Decoder = new Ucs4Decoder3412();
    }

    public override string EncodingName
    {
      get
      {
        return "ucs-4 (order 3412)";
      }
    }

    public override byte[] GetPreamble()
    {
      return new byte[4] { 0xfe, 0xff, 0x00, 0x00 };
    }
  }
}