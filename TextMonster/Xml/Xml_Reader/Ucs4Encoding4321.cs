namespace TextMonster.Xml.Xml_Reader
{
  internal class Ucs4Encoding4321 : Ucs4Encoding
  {
    public Ucs4Encoding4321()
    {
      ucs4Decoder = new Ucs4Decoder4321();
    }

    public override string EncodingName
    {
      get
      {
        return "ucs-4";
      }
    }

    public override byte[] GetPreamble()
    {
      return new byte[4] { 0xff, 0xfe, 0x00, 0x00 };
    }
  }
}