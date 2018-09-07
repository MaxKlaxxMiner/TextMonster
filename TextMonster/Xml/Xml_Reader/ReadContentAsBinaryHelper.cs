namespace TextMonster.Xml.Xml_Reader
{
  internal partial class ReadContentAsBinaryHelper
  {

    // Private enums
    enum State
    {
      None,
      InReadElementContent,
    }

    // Fields 
    FastXmlReader reader;
    State state;
    int valueOffset;
    bool isEnd;

    bool canReadValueChunk;
    char[] valueChunk;
    int valueChunkLength;

    IncrementalReadDecoder decoder;
    Base64Decoder base64Decoder;
    BinHexDecoder binHexDecoder;

    // Constants
    const int ChunkSize = 256;

    // Constructor
    internal ReadContentAsBinaryHelper(FastXmlReader reader)
    {
      this.reader = reader;
      this.canReadValueChunk = reader.CanReadValueChunk;

      if (canReadValueChunk)
      {
        valueChunk = new char[ChunkSize];
      }
    }

    internal void Finish()
    {
      if (state != State.None)
      {
        while (MoveToNextContentNode(true))
          ;
        if (state == State.InReadElementContent)
        {
          if (reader.NodeType != XmlNodeType.EndElement)
          {
            throw new XmlException(Res.Xml_InvalidNodeType, reader.NodeType.ToString(), reader as IXmlLineInfo);
          }
          // move off the EndElement
          reader.Read();
        }
      }
      Reset();
    }

    internal void Reset()
    {
      state = State.None;
      isEnd = false;
      valueOffset = 0;
    }

    bool MoveToNextContentNode(bool moveIfOnContentNode)
    {
      do
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Attribute:
          return !moveIfOnContentNode;
          case XmlNodeType.Text:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          case XmlNodeType.CDATA:
          if (!moveIfOnContentNode)
          {
            return true;
          }
          break;
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.Comment:
          case XmlNodeType.EndEntity:
          // skip comments, pis and end entity nodes
          break;
          case XmlNodeType.EntityReference:
          if (reader.CanResolveEntity)
          {
            reader.ResolveEntity();
            break;
          }
          goto default;
          default:
          return false;
        }
        moveIfOnContentNode = false;
      } while (reader.Read());
      return false;
    }
  }
}
