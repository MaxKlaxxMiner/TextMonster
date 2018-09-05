namespace TextMonster.Xml.Xml_Reader
{
  internal partial class XmlTextWriterBase64Encoder : Base64Encoder
  {

    XmlTextEncoder xmlTextEncoder;

    internal XmlTextWriterBase64Encoder(XmlTextEncoder xmlTextEncoder)
    {
      this.xmlTextEncoder = xmlTextEncoder;
    }

    internal override void WriteChars(char[] chars, int index, int count)
    {
      xmlTextEncoder.WriteRaw(chars, index, count);
    }
  }
}