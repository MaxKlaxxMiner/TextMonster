using System.IO;

namespace TextMonster.Xml.Xml_Reader
{
  internal class HtmlEncodedRawTextWriterIndent : HtmlEncodedRawTextWriter
  {
    //
    // Fields
    //
    int indentLevel;

    // for detecting SE SC sitution
    int endBlockPos;

    // settings
    string indentChars;
    bool newLineOnAttributes;

    //
    // Constructors
    //


    public HtmlEncodedRawTextWriterIndent(TextWriter writer, XmlWriterSettings settings)
      : base(writer, settings)
    {
      Init(settings);
    }


    public HtmlEncodedRawTextWriterIndent(Stream stream, XmlWriterSettings settings)
      : base(stream, settings)
    {
      Init(settings);
    }

    //
    // XmlRawWriter overrides
    //
    /// <summary>
    /// Serialize the document type declaration.
    /// </summary>
    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      base.WriteDocType(name, pubid, sysid, subset);

      // Allow indentation after DocTypeDecl
      endBlockPos = base.bufPos;
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      base.elementScope.Push((byte)base.currentElementProperties);

      if (ns.Length == 0)
      {
        base.currentElementProperties = (ElementProperties)elementPropertySearch.FindCaseInsensitiveString(localName);

        if (endBlockPos == base.bufPos && (base.currentElementProperties & ElementProperties.BLOCK_WS) != 0)
        {
          WriteIndent();
        }
        indentLevel++;

        base.bufChars[bufPos++] = (char)'<';
      }
      else
      {
        base.currentElementProperties = ElementProperties.HAS_NS | ElementProperties.BLOCK_WS;

        if (endBlockPos == base.bufPos)
        {
          WriteIndent();
        }
        indentLevel++;

        base.bufChars[base.bufPos++] = (char)'<';
        if (prefix.Length != 0)
        {
          base.RawText(prefix);
          base.bufChars[base.bufPos++] = (char)':';
        }
      }
      base.RawText(localName);
      base.attrEndPos = bufPos;
    }

    internal override void StartElementContent()
    {
      base.bufChars[base.bufPos++] = (char)'>';

      // Detect whether content is output
      base.contentPos = base.bufPos;

      if ((currentElementProperties & ElementProperties.HEAD) != 0)
      {
        WriteIndent();
        WriteMetaElement();
        endBlockPos = base.bufPos;
      }
      else if ((base.currentElementProperties & ElementProperties.BLOCK_WS) != 0)
      {
        // store the element block position
        endBlockPos = base.bufPos;
      }
    }

    internal override void WriteEndElement(string prefix, string localName, string ns)
    {
      bool isBlockWs;

      indentLevel--;

      // If this element has block whitespace properties,
      isBlockWs = (base.currentElementProperties & ElementProperties.BLOCK_WS) != 0;
      if (isBlockWs)
      {
        // And if the last node to be output had block whitespace properties,
        // And if content was output within this element,
        if (endBlockPos == base.bufPos && base.contentPos != base.bufPos)
        {
          // Then indent
          WriteIndent();
        }
      }

      base.WriteEndElement(prefix, localName, ns);

      // Reset contentPos in case of empty elements
      base.contentPos = 0;

      // Mark end of element in buffer for element's with block whitespace properties
      if (isBlockWs)
      {
        endBlockPos = base.bufPos;
      }
    }

    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      if (newLineOnAttributes)
      {
        RawText(base.newLineChars);
        indentLevel++;
        WriteIndent();
        indentLevel--;
      }
      base.WriteStartAttribute(prefix, localName, ns);
    }

    protected override void FlushBuffer()
    {
      // Make sure the buffer will reset the block position
      endBlockPos = (endBlockPos == base.bufPos) ? 1 : 0;
      base.FlushBuffer();
    }

    //
    // Private methods
    //
    private void Init(XmlWriterSettings settings)
    {
      indentLevel = 0;
      indentChars = settings.IndentChars;
      newLineOnAttributes = settings.NewLineOnAttributes;
    }

    private void WriteIndent()
    {
      // <block><inline>  -- suppress ws betw <block> and <inline>
      // <block><block>   -- don't suppress ws betw <block> and <block>
      // <block>text      -- suppress ws betw <block> and text (handled by wcharText method)
      // <block><?PI?>    -- suppress ws betw <block> and PI
      // <block><!-- -->  -- suppress ws betw <block> and comment

      // <inline><block>  -- suppress ws betw <inline> and <block>
      // <inline><inline> -- suppress ws betw <inline> and <inline>
      // <inline>text     -- suppress ws betw <inline> and text (handled by wcharText method)
      // <inline><?PI?>   -- suppress ws betw <inline> and PI
      // <inline><!-- --> -- suppress ws betw <inline> and comment

      RawText(base.newLineChars);
      for (int i = indentLevel; i > 0; i--)
      {
        RawText(indentChars);
      }
    }
  }
}
