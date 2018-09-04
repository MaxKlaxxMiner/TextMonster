namespace TextMonster.Xml.Xml_Reader
{
  internal class ReaderPositionInfo : PositionInfo
  {
    private IXmlLineInfo lineInfo;

    public ReaderPositionInfo(IXmlLineInfo lineInfo)
    {
      this.lineInfo = lineInfo;
    }

    public override bool HasLineInfo()
    {
      return lineInfo.HasLineInfo();
    }

    public override int LineNumber
    {
      get
      {
        return lineInfo.LineNumber;
      }
    }

    public override int LinePosition
    {
      get
      {
        return lineInfo.LinePosition;
      }
    }
  }
}
