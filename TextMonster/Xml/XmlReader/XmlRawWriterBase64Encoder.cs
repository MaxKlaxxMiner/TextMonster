namespace TextMonster.Xml.XmlReader
{
  internal partial class XmlRawWriterBase64Encoder : Base64Encoder
  {

    XmlRawWriter rawWriter;

    internal XmlRawWriterBase64Encoder(XmlRawWriter rawWriter)
    {
      this.rawWriter = rawWriter;
    }

    internal override void WriteChars(char[] chars, int index, int count)
    {
      rawWriter.WriteRaw(chars, index, count);
    }
  }
}
