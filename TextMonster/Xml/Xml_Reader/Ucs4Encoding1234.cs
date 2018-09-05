namespace TextMonster.Xml.Xml_Reader
{
  internal class Ucs4Encoding1234 : Ucs4Encoding
  {

    public Ucs4Encoding1234()
    {
      ucs4Decoder = new Ucs4Decoder1234();
    }

    public override string EncodingName
    {
      get
      {
        return "ucs-4 (Bigendian)";
      }
    }

    public override byte[] GetPreamble()
    {
      return new byte[4] { 0x00, 0x00, 0xfe, 0xff };
    }
  }
}