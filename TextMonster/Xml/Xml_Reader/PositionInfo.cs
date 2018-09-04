using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class PositionInfo : IXmlLineInfo
  {
    public virtual bool HasLineInfo() { return false; }
    public virtual int LineNumber { get { return 0; } }
    public virtual int LinePosition { get { return 0; } }

    public static PositionInfo GetPositionInfo(Object o)
    {
      IXmlLineInfo li = o as IXmlLineInfo;
      if (li != null)
      {
        return new ReaderPositionInfo(li);
      }
      else
      {
        return new PositionInfo();
      }
    }
  }
}
