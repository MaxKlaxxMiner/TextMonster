using System;
using System.Runtime.Versioning;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  //
  // DtdParserProxy: IDtdParserAdapter proxy for XmlTextReaderImpl
  //
  internal partial class DtdParserProxy : IDtdParserAdapterV1
  {

    // Fields
    private XmlTextReaderImpl reader;

    // Constructors
    internal DtdParserProxy(XmlTextReaderImpl reader)
    {
      this.reader = reader;
    }

    // IDtdParserAdapter proxies
    XmlNameTable IDtdParserAdapter.NameTable
    {
      get { return reader.DtdParserProxy_NameTable; }
    }

    IXmlNamespaceResolver IDtdParserAdapter.NamespaceResolver
    {
      get { return reader.DtdParserProxy_NamespaceResolver; }
    }

    Uri IDtdParserAdapter.BaseUri
    {
      // SxS: DtdParserProxy_BaseUri property on the reader may expose machine scope resources. This property
      // is just returning the value of the other property, so it may expose machine scope resource as well.
      [ResourceConsumption(ResourceScope.Machine)]
      [ResourceExposure(ResourceScope.Machine)]
      get { return reader.DtdParserProxy_BaseUri; }
    }

    bool IDtdParserAdapter.IsEof
    {
      get { return reader.DtdParserProxy_IsEof; }
    }

    char[] IDtdParserAdapter.ParsingBuffer
    {
      get { return reader.DtdParserProxy_ParsingBuffer; }
    }

    int IDtdParserAdapter.ParsingBufferLength
    {
      get { return reader.DtdParserProxy_ParsingBufferLength; }
    }

    int IDtdParserAdapter.CurrentPosition
    {
      get { return reader.DtdParserProxy_CurrentPosition; }
      set { reader.DtdParserProxy_CurrentPosition = value; }
    }

    int IDtdParserAdapter.EntityStackLength
    {
      get { return reader.DtdParserProxy_EntityStackLength; }
    }

    bool IDtdParserAdapter.IsEntityEolNormalized
    {
      get { return reader.DtdParserProxy_IsEntityEolNormalized; }
    }

    void IDtdParserAdapter.OnNewLine(int pos)
    {
      reader.DtdParserProxy_OnNewLine(pos);
    }

    int IDtdParserAdapter.LineNo
    {
      get { return reader.DtdParserProxy_LineNo; }
    }

    int IDtdParserAdapter.LineStartPosition
    {
      get { return reader.DtdParserProxy_LineStartPosition; }
    }

    int IDtdParserAdapter.ReadData()
    {
      return reader.DtdParserProxy_ReadData();
    }

    int IDtdParserAdapter.ParseNumericCharRef(StringBuilder internalSubsetBuilder)
    {
      return reader.DtdParserProxy_ParseNumericCharRef(internalSubsetBuilder);
    }

    int IDtdParserAdapter.ParseNamedCharRef(bool expand, StringBuilder internalSubsetBuilder)
    {
      return reader.DtdParserProxy_ParseNamedCharRef(expand, internalSubsetBuilder);
    }

    void IDtdParserAdapter.ParsePI()
    {
      reader.DtdParserProxy_ParsePI();
    }

    void IDtdParserAdapter.ParseComment()
    {
      reader.DtdParserProxy_ParseComment();
    }

    bool IDtdParserAdapter.PushEntity(IDtdEntityInfo entity, out int entityId)
    {

      return reader.DtdParserProxy_PushEntity(entity, out entityId);

    }

    bool IDtdParserAdapter.PopEntity(out IDtdEntityInfo oldEntity, out int newEntityId)
    {
      return reader.DtdParserProxy_PopEntity(out oldEntity, out newEntityId);
    }

    bool IDtdParserAdapter.PushExternalSubset(string systemId, string publicId)
    {
      return reader.DtdParserProxy_PushExternalSubset(systemId, publicId);
    }

    void IDtdParserAdapter.PushInternalDtd(string baseUri, string internalDtd)
    {
      reader.DtdParserProxy_PushInternalDtd(baseUri, internalDtd);
    }

    void IDtdParserAdapter.Throw(Exception e)
    {
      reader.DtdParserProxy_Throw(e);
    }

    void IDtdParserAdapter.OnSystemId(string systemId, LineInfo keywordLineInfo, LineInfo systemLiteralLineInfo)
    {
      reader.DtdParserProxy_OnSystemId(systemId, keywordLineInfo, systemLiteralLineInfo);
    }

    void IDtdParserAdapter.OnPublicId(string publicId, LineInfo keywordLineInfo, LineInfo publicLiteralLineInfo)
    {
      reader.DtdParserProxy_OnPublicId(publicId, keywordLineInfo, publicLiteralLineInfo);
    }

    bool IDtdParserAdapterWithValidation.DtdValidation
    {
      get { return reader.DtdParserProxy_DtdValidation; }
    }

    IValidationEventHandling IDtdParserAdapterWithValidation.ValidationEventHandling
    {
      get { return reader.DtdParserProxy_ValidationEventHandling; }
    }

    bool IDtdParserAdapterV1.Normalization
    {
      get { return reader.DtdParserProxy_Normalization; }
    }

    bool IDtdParserAdapterV1.Namespaces
    {
      get { return reader.DtdParserProxy_Namespaces; }
    }

    bool IDtdParserAdapterV1.V1CompatibilityMode
    {
      get { return reader.DtdParserProxy_V1CompatibilityMode; }
    }
  }
}
