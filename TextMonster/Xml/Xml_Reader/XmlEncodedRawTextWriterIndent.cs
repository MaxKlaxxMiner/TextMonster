using System.IO;

namespace TextMonster.Xml.Xml_Reader
{
  // Same as base text writer class except that elements, attributes, comments, and pi's are indented.
  internal partial class XmlEncodedRawTextWriterIndent : XmlEncodedRawTextWriter
  {

    //
    // Fields
    //
    protected int indentLevel;
    protected bool newLineOnAttributes;
    protected string indentChars;

    protected bool mixedContent;
    private BitStack mixedContentStack;

    protected ConformanceLevel conformanceLevel = ConformanceLevel.Auto;

    //
    // Constructors
    //

    public XmlEncodedRawTextWriterIndent(TextWriter writer, XmlWriterSettings settings)
      : base(writer, settings)
    {
      Init(settings);
    }

    public XmlEncodedRawTextWriterIndent(Stream stream, XmlWriterSettings settings)
      : base(stream, settings)
    {
      Init(settings);
    }

    //
    // XmlWriter methods
    //
    public override XmlWriterSettings Settings
    {
      get
      {
        XmlWriterSettings settings = base.Settings;

        settings.ReadOnly = false;
        settings.Indent = true;
        settings.IndentChars = indentChars;
        settings.NewLineOnAttributes = newLineOnAttributes;
        settings.ReadOnly = true;

        return settings;
      }
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      // Add indentation
      if (!mixedContent && base.textPos != base.bufPos)
      {
        WriteIndent();
      }
      base.WriteDocType(name, pubid, sysid, subset);
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      // Add indentation
      if (!mixedContent && base.textPos != base.bufPos)
      {
        WriteIndent();
      }
      indentLevel++;
      mixedContentStack.PushBit(mixedContent);

      base.WriteStartElement(prefix, localName, ns);
    }

    internal override void StartElementContent()
    {
      // If this is the root element and we're writing a document
      //   do not inherit the mixedContent flag into the root element.
      //   This is to allow for whitespace nodes on root level
      //   without disabling indentation for the whole document.
      if (indentLevel == 1 && conformanceLevel == ConformanceLevel.Document)
      {
        mixedContent = false;
      }
      else
      {
        mixedContent = mixedContentStack.PeekBit();
      }
      base.StartElementContent();
    }

    internal override void OnRootElement(ConformanceLevel currentConformanceLevel)
    {
      // Just remember the current conformance level
      conformanceLevel = currentConformanceLevel;
    }

    internal override void WriteEndElement(string prefix, string localName, string ns)
    {
      // Add indentation
      indentLevel--;
      if (!mixedContent && base.contentPos != base.bufPos)
      {
        // There was content, so try to indent
        if (base.textPos != base.bufPos)
        {
          WriteIndent();
        }
      }
      mixedContent = mixedContentStack.PopBit();

      base.WriteEndElement(prefix, localName, ns);
    }

    internal override void WriteFullEndElement(string prefix, string localName, string ns)
    {
      // Add indentation
      indentLevel--;
      if (!mixedContent && base.contentPos != base.bufPos)
      {
        // There was content, so try to indent
        if (base.textPos != base.bufPos)
        {
          WriteIndent();
        }
      }
      mixedContent = mixedContentStack.PopBit();

      base.WriteFullEndElement(prefix, localName, ns);
    }

    // Same as base class, plus possible indentation.
    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      // Add indentation
      if (newLineOnAttributes)
      {
        WriteIndent();
      }

      base.WriteStartAttribute(prefix, localName, ns);
    }

    public override void WriteCData(string text)
    {
      mixedContent = true;
      base.WriteCData(text);
    }

    public override void WriteComment(string text)
    {
      if (!mixedContent && base.textPos != base.bufPos)
      {
        WriteIndent();
      }

      base.WriteComment(text);
    }

    public override void WriteProcessingInstruction(string target, string text)
    {
      if (!mixedContent && base.textPos != base.bufPos)
      {
        WriteIndent();
      }

      base.WriteProcessingInstruction(target, text);
    }

    public override void WriteEntityRef(string name)
    {
      mixedContent = true;
      base.WriteEntityRef(name);
    }

    public override void WriteCharEntity(char ch)
    {
      mixedContent = true;
      base.WriteCharEntity(ch);
    }

    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      mixedContent = true;
      base.WriteSurrogateCharEntity(lowChar, highChar);
    }

    public override void WriteWhitespace(string ws)
    {
      mixedContent = true;
      base.WriteWhitespace(ws);
    }

    public override void WriteString(string text)
    {
      mixedContent = true;
      base.WriteString(text);
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      mixedContent = true;
      base.WriteChars(buffer, index, count);
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      mixedContent = true;
      base.WriteRaw(buffer, index, count);
    }

    public override void WriteRaw(string data)
    {
      mixedContent = true;
      base.WriteRaw(data);
    }

    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      mixedContent = true;
      base.WriteBase64(buffer, index, count);
    }

    //
    // Private methods
    //
    private void Init(XmlWriterSettings settings)
    {
      indentLevel = 0;
      indentChars = settings.IndentChars;
      newLineOnAttributes = settings.NewLineOnAttributes;
      mixedContentStack = new BitStack();

      // check indent characters that they are valid XML characters
      if (base.checkCharacters)
      {
        if (newLineOnAttributes)
        {
          base.ValidateContentChars(indentChars, "IndentChars", true);
          base.ValidateContentChars(newLineChars, "NewLineChars", true);
        }
        else
        {
          base.ValidateContentChars(indentChars, "IndentChars", false);
          if (base.newLineHandling != NewLineHandling.Replace)
          {
            base.ValidateContentChars(newLineChars, "NewLineChars", false);
          }
        }
      }
    }

    // Add indentation to output.  Write newline and then repeat IndentChars for each indent level.
    private void WriteIndent()
    {
      RawText(base.newLineChars);
      for (int i = indentLevel; i > 0; i--)
      {
        RawText(indentChars);
      }
    }
  }
}
