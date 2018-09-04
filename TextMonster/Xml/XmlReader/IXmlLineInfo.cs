namespace TextMonster.Xml.XmlReader
{
  /// <include file='doc\IXmlLineInfo.uex' path='docs/doc[@for="IXmlLineInfo"]/*' />
  public interface IXmlLineInfo
  {
    /// <include file='doc\IXmlLineInfo.uex' path='docs/doc[@for="IXmlLineInfo.HasLineInfo"]/*' />
    bool HasLineInfo();
    /// <include file='doc\IXmlLineInfo.uex' path='docs/doc[@for="IXmlLineInfo.LineNumber"]/*' />
    int LineNumber { get; }
    /// <include file='doc\IXmlLineInfo.uex' path='docs/doc[@for="IXmlLineInfo.LinePosition"]/*' />
    int LinePosition { get; }
  }
}
