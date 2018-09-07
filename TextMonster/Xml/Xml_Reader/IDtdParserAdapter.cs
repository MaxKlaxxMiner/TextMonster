using System;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal partial interface IDtdParserAdapter
  {

    NameTable NameTable { get; }
    IXmlNamespaceResolver NamespaceResolver { get; }

    Uri BaseUri { get; }

    char[] ParsingBuffer { get; }
    int ParsingBufferLength { get; }
    int CurrentPosition { get; set; }
    int LineNo { get; }
    int LineStartPosition { get; }
    bool IsEof { get; }
    int EntityStackLength { get; }
    bool IsEntityEolNormalized { get; }

    int ReadData();

    void OnNewLine(int pos);

    int ParseNumericCharRef(StringBuilder internalSubsetBuilder);
    int ParseNamedCharRef(bool expand, StringBuilder internalSubsetBuilder);
    void ParsePI();
    void ParseComment();

    bool PushEntity(IDtdEntityInfo entity, out int entityId);

    bool PopEntity(out IDtdEntityInfo oldEntity, out int newEntityId);

    bool PushExternalSubset(string systemId, string publicId);

    void PushInternalDtd(string baseUri, string internalDtd);
    void OnSystemId(string systemId, LineInfo keywordLineInfo, LineInfo systemLiteralLineInfo);
    void OnPublicId(string publicId, LineInfo keywordLineInfo, LineInfo publicLiteralLineInfo);

    void Throw(Exception e);

  }
}
