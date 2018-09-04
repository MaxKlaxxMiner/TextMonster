﻿using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  // Concrete implementation of XmlWriter abstract class that serializes events as encoded XML
  // text.  The general-purpose XmlEncodedTextWriter uses the Encoder class to output to any
  // encoding.  The XmlUtf8TextWriter class combined the encoding operation with serialization
  // in order to achieve better performance.
  internal partial class XmlEncodedRawTextWriter : XmlRawWriter
  {

    //
    // Fields
    //

    // main buffer
    protected byte[] bufBytes;

    // output stream
    protected Stream stream;

    // encoding of the stream or text writer 
    protected Encoding encoding;

    // char type tables
    protected XmlCharType xmlCharType = XmlCharType.Instance;

    // buffer positions
    protected int bufPos = 1;     // buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
    // close an empty element or in CDATA section detection of double ]; _BUFFER[0] will always be 0
    protected int textPos = 1;    // text end position; don't indent first element, pi, or comment
    protected int contentPos;     // element content end position
    protected int cdataPos;       // cdata end position
    protected int attrEndPos;     // end of the last attribute
    protected int bufLen = BUFSIZE;

    // flags
    protected bool writeToNull;
    protected bool hadDoubleBracket;
    protected bool inAttributeValue;

    protected int bufBytesUsed;
    protected char[] bufChars;

    // encoder for encoding chars in specified encoding when writing to stream
    protected Encoder encoder;

    // output text writer
    protected TextWriter writer;

    // escaping of characters invalid in the output encoding
    protected bool trackTextContent;
    protected bool inTextContent;
    private int lastMarkPos;
    private int[] textContentMarks;   // even indices contain text content start positions
    // odd indices contain markup start positions 
    private CharEntityEncoderFallback charEntityFallback;

    // writer settings
    protected NewLineHandling newLineHandling;
    protected bool closeOutput;
    protected bool omitXmlDeclaration;
    protected string newLineChars;
    protected bool checkCharacters;

    protected XmlStandalone standalone;
    protected XmlOutputMethod outputMethod;

    protected bool autoXmlDeclaration;
    protected bool mergeCDataSections;

    //
    // Constants
    //
    private const int BUFSIZE = 2048 * 3;       // Should be greater than default FileStream size (4096), otherwise the FileStream will try to cache the data
    private const int ASYNCBUFSIZE = 64 * 1024; // Set async buffer size to 64KB
    private const int OVERFLOW = 32;            // Allow overflow in order to reduce checks when writing out constant size markup
    private const int INIT_MARKS_COUNT = 64;

    //
    // Constructors
    //
    // Construct and initialize an instance of this class.
    protected XmlEncodedRawTextWriter(XmlWriterSettings settings)
    {

#if ASYNC
            useAsync = settings.Async;
#endif

      // copy settings
      newLineHandling = settings.NewLineHandling;
      omitXmlDeclaration = settings.OmitXmlDeclaration;
      newLineChars = settings.NewLineChars;
      checkCharacters = settings.CheckCharacters;
      closeOutput = settings.CloseOutput;

      standalone = settings.Standalone;
      outputMethod = settings.OutputMethod;
      mergeCDataSections = settings.MergeCDataSections;

      if (checkCharacters && newLineHandling == NewLineHandling.Replace)
      {
        ValidateContentChars(newLineChars, "NewLineChars", false);
      }
    }

    // Construct an instance of this class that outputs text to the TextWriter interface.
    public XmlEncodedRawTextWriter(TextWriter writer, XmlWriterSettings settings)
      : this(settings)
    {
      this.writer = writer;
      this.encoding = writer.Encoding;
      // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
      this.bufChars = new char[bufLen + OVERFLOW];

      // Write the xml declaration
      if (settings.AutoXmlDeclaration)
      {
        WriteXmlDeclaration(standalone);
        autoXmlDeclaration = true;
      }

    }

    // Construct an instance of this class that serializes to a Stream interface.
    public XmlEncodedRawTextWriter(Stream stream, XmlWriterSettings settings)
      : this(settings)
    {
      this.stream = stream;
      this.encoding = settings.Encoding;

      // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
      bufChars = new char[bufLen + OVERFLOW];

      bufBytes = new byte[bufChars.Length];
      bufBytesUsed = 0;

      // Init escaping of characters not fitting into the target encoding
      trackTextContent = true;
      inTextContent = false;
      lastMarkPos = 0;
      textContentMarks = new int[INIT_MARKS_COUNT];
      textContentMarks[0] = 1;

      charEntityFallback = new CharEntityEncoderFallback();
      this.encoding = (Encoding)settings.Encoding.Clone();
      encoding.EncoderFallback = charEntityFallback;

      encoder = encoding.GetEncoder();

      if (!stream.CanSeek || stream.Position == 0)
      {
        byte[] bom = encoding.GetPreamble();
        if (bom.Length != 0)
        {
          this.stream.Write(bom, 0, bom.Length);
        }
      }

      // Write the xml declaration
      if (settings.AutoXmlDeclaration)
      {
        WriteXmlDeclaration(standalone);
        autoXmlDeclaration = true;
      }

    }

    //
    // XmlWriter implementation
    //
    // Returns settings the writer currently applies.
    public override XmlWriterSettings Settings
    {
      get
      {
        XmlWriterSettings settings = new XmlWriterSettings();

        settings.Encoding = this.encoding;
        settings.OmitXmlDeclaration = this.omitXmlDeclaration;
        settings.NewLineHandling = this.newLineHandling;
        settings.NewLineChars = this.newLineChars;
        settings.CloseOutput = this.closeOutput;
        settings.ConformanceLevel = ConformanceLevel.Auto;
        settings.CheckCharacters = checkCharacters;

        settings.AutoXmlDeclaration = autoXmlDeclaration;
        settings.Standalone = standalone;
        settings.OutputMethod = outputMethod;

        settings.ReadOnly = true;
        return settings;

      }
    }

    // Write the xml declaration.  This must be the first call.  
    internal override void WriteXmlDeclaration(XmlStandalone standalone)
    {
      // Output xml declaration only if user allows it and it was not already output
      if (!omitXmlDeclaration && !autoXmlDeclaration)
      {

        if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

        RawText("<?xml version=\"");

        // Version
        RawText("1.0");

        // Encoding
        if (encoding != null)
        {
          RawText("\" encoding=\"");
          RawText(encoding.WebName);
        }

        // Standalone
        if (standalone != XmlStandalone.Omit)
        {
          RawText("\" standalone=\"");
          RawText(standalone == XmlStandalone.Yes ? "yes" : "no");
        }

        RawText("\"?>");
      }
    }

    internal override void WriteXmlDeclaration(string xmldecl)
    {
      // Output xml declaration only if user allows it and it was not already output
      if (!omitXmlDeclaration && !autoXmlDeclaration)
      {
        WriteProcessingInstruction("xml", xmldecl);
      }

    }

    // Serialize the document type declaration.
    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      RawText("<!DOCTYPE ");
      RawText(name);
      if (pubid != null)
      {
        RawText(" PUBLIC \"");
        RawText(pubid);
        RawText("\" \"");
        if (sysid != null)
        {
          RawText(sysid);
        }
        bufChars[bufPos++] = (char)'"';
      }
      else if (sysid != null)
      {
        RawText(" SYSTEM \"");
        RawText(sysid);
        bufChars[bufPos++] = (char)'"';
      }
      else
      {
        bufChars[bufPos++] = (char)' ';
      }

      if (subset != null)
      {
        bufChars[bufPos++] = (char)'[';
        RawText(subset);
        bufChars[bufPos++] = (char)']';
      }

      bufChars[this.bufPos++] = (char)'>';
    }

    // Serialize the beginning of an element start tag: "<prefix:localName"
    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      bufChars[bufPos++] = (char)'<';
      if (prefix != null && prefix.Length != 0)
      {
        RawText(prefix);
        bufChars[this.bufPos++] = (char)':';
      }

      RawText(localName);

      attrEndPos = bufPos;
    }

    // Serialize the end of an element start tag in preparation for content serialization: ">"
    internal override void StartElementContent()
    {
      bufChars[bufPos++] = (char)'>';

      // StartElementContent is always called; therefore, in order to allow shortcut syntax, we save the
      // position of the '>' character.  If WriteEndElement is called and no other characters have been
      // output, then the '>' character can be be overwritten with the shortcut syntax " />".
      contentPos = bufPos;
    }

    // Serialize an element end tag: "</prefix:localName>", if content was output.  Otherwise, serialize
    // the shortcut syntax: " />".
    internal override void WriteEndElement(string prefix, string localName, string ns)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      if (contentPos != bufPos)
      {
        // Content has been output, so can't use shortcut syntax
        bufChars[bufPos++] = (char)'<';
        bufChars[bufPos++] = (char)'/';

        if (prefix != null && prefix.Length != 0)
        {
          RawText(prefix);
          bufChars[bufPos++] = (char)':';
        }
        RawText(localName);
        bufChars[bufPos++] = (char)'>';
      }
      else
      {
        // Use shortcut syntax; overwrite the already output '>' character
        bufPos--;
        bufChars[bufPos++] = (char)' ';
        bufChars[bufPos++] = (char)'/';
        bufChars[bufPos++] = (char)'>';
      }
    }

    // Serialize a full element end tag: "</prefix:localName>"
    internal override void WriteFullEndElement(string prefix, string localName, string ns)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      bufChars[bufPos++] = (char)'<';
      bufChars[bufPos++] = (char)'/';

      if (prefix != null && prefix.Length != 0)
      {
        RawText(prefix);
        bufChars[bufPos++] = (char)':';
      }
      RawText(localName);
      bufChars[bufPos++] = (char)'>';
    }

    // Serialize an attribute tag using double quotes around the attribute value: 'prefix:localName="'
    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      if (attrEndPos == bufPos)
      {
        bufChars[bufPos++] = (char)' ';
      }

      if (prefix != null && prefix.Length > 0)
      {
        RawText(prefix);
        bufChars[bufPos++] = (char)':';
      }
      RawText(localName);
      bufChars[bufPos++] = (char)'=';
      bufChars[bufPos++] = (char)'"';

      inAttributeValue = true;
    }

    // Serialize the end of an attribute value using double quotes: '"'
    public override void WriteEndAttribute()
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }
      bufChars[bufPos++] = (char)'"';
      inAttributeValue = false;
      attrEndPos = bufPos;

    }

    internal override void WriteNamespaceDeclaration(string prefix, string namespaceName)
    {
      this.WriteStartNamespaceDeclaration(prefix);
      this.WriteString(namespaceName);
      this.WriteEndNamespaceDeclaration();
    }

    internal override bool SupportsNamespaceDeclarationInChunks
    {
      get
      {
        return true;
      }
    }

    internal override void WriteStartNamespaceDeclaration(string prefix)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      // VSTFDEVDIV bug #583965: Inconsistency between Silverlight 2 and Dev10 in the way a single xmlns attribute is serialized    
      // Resolved as: Won't fix (breaking change)

      if (prefix.Length == 0)
      {
        RawText(" xmlns=\"");
      }
      else
      {
        RawText(" xmlns:");
        RawText(prefix);
        bufChars[bufPos++] = (char)'=';
        bufChars[bufPos++] = (char)'"';
      }

      inAttributeValue = true;
      if (trackTextContent && inTextContent != true) { ChangeTextContentMark(true); }
    }

    internal override void WriteEndNamespaceDeclaration()
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }
      inAttributeValue = false;

      bufChars[bufPos++] = (char)'"';
      attrEndPos = bufPos;

    }

    // Serialize a CData section.  If the "]]>" pattern is found within
    // the text, replace it with "]]><![CDATA[>".
    public override void WriteCData(string text)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      if (mergeCDataSections && bufPos == cdataPos)
      {
        // Merge adjacent cdata sections - overwrite the "]]>" characters
        bufPos -= 3;
      }
      else
      {
        // Start a new cdata section
        bufChars[bufPos++] = (char)'<';
        bufChars[bufPos++] = (char)'!';
        bufChars[bufPos++] = (char)'[';
        bufChars[bufPos++] = (char)'C';
        bufChars[bufPos++] = (char)'D';
        bufChars[bufPos++] = (char)'A';
        bufChars[bufPos++] = (char)'T';
        bufChars[bufPos++] = (char)'A';
        bufChars[bufPos++] = (char)'[';
      }

      WriteCDataSection(text);

      bufChars[bufPos++] = (char)']';
      bufChars[bufPos++] = (char)']';
      bufChars[bufPos++] = (char)'>';

      textPos = bufPos;
      cdataPos = bufPos;
    }

    // Serialize a comment.
    public override void WriteComment(string text)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      bufChars[bufPos++] = (char)'<';
      bufChars[bufPos++] = (char)'!';
      bufChars[bufPos++] = (char)'-';
      bufChars[bufPos++] = (char)'-';

      WriteCommentOrPi(text, '-');

      bufChars[bufPos++] = (char)'-';
      bufChars[bufPos++] = (char)'-';
      bufChars[bufPos++] = (char)'>';
    }

    // Serialize a processing instruction.
    public override void WriteProcessingInstruction(string name, string text)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      bufChars[bufPos++] = (char)'<';
      bufChars[bufPos++] = (char)'?';
      RawText(name);

      if (text.Length > 0)
      {
        bufChars[bufPos++] = (char)' ';
        WriteCommentOrPi(text, '?');
      }

      bufChars[bufPos++] = (char)'?';
      bufChars[bufPos++] = (char)'>';
    }

    // Serialize an entity reference.
    public override void WriteEntityRef(string name)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      bufChars[bufPos++] = (char)'&';
      RawText(name);
      bufChars[bufPos++] = (char)';';

      if (bufPos > bufLen)
      {
        FlushBuffer();
      }

      textPos = bufPos;
    }

    // Serialize a character entity reference.
    public override void WriteCharEntity(char ch)
    {
      string strVal = ((int)ch).ToString("X", NumberFormatInfo.InvariantInfo);

      if (checkCharacters && !xmlCharType.IsCharData(ch))
      {
        // we just have a single char, not a surrogate, therefore we have to pass in '\0' for the second char
        throw XmlConvert.CreateInvalidCharException(ch, '\0');
      }

      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      bufChars[bufPos++] = (char)'&';
      bufChars[bufPos++] = (char)'#';
      bufChars[bufPos++] = (char)'x';
      RawText(strVal);
      bufChars[bufPos++] = (char)';';

      if (bufPos > bufLen)
      {
        FlushBuffer();
      }

      textPos = bufPos;
    }

    // Serialize a whitespace node.

    public override unsafe void WriteWhitespace(string ws)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      fixed (char* pSrc = ws)
      {
        char* pSrcEnd = pSrc + ws.Length;
        if (inAttributeValue)
        {
          WriteAttributeTextBlock(pSrc, pSrcEnd);
        }
        else
        {
          WriteElementTextBlock(pSrc, pSrcEnd);
        }
      }

    }

    // Serialize either attribute or element text using XML rules.

    public override unsafe void WriteString(string text)
    {
      if (trackTextContent && inTextContent != true) { ChangeTextContentMark(true); }

      fixed (char* pSrc = text)
      {
        char* pSrcEnd = pSrc + text.Length;
        if (inAttributeValue)
        {
          WriteAttributeTextBlock(pSrc, pSrcEnd);
        }
        else
        {
          WriteElementTextBlock(pSrc, pSrcEnd);
        }
      }

    }

    // Serialize surrogate character entity.
    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }
      int surrogateChar = XmlCharType.CombineSurrogateChar(lowChar, highChar);

      bufChars[bufPos++] = (char)'&';
      bufChars[bufPos++] = (char)'#';
      bufChars[bufPos++] = (char)'x';
      RawText(surrogateChar.ToString("X", NumberFormatInfo.InvariantInfo));
      bufChars[bufPos++] = (char)';';
      textPos = bufPos;
    }

    // Serialize either attribute or element text using XML rules.
    // Arguments are validated in the XmlWellformedWriter layer.

    public override unsafe void WriteChars(char[] buffer, int index, int count)
    {
      if (trackTextContent && inTextContent != true) { ChangeTextContentMark(true); }

      fixed (char* pSrcBegin = &buffer[index])
      {
        if (inAttributeValue)
        {
          WriteAttributeTextBlock(pSrcBegin, pSrcBegin + count);
        }
        else
        {
          WriteElementTextBlock(pSrcBegin, pSrcBegin + count);
        }
      }

    }

    // Serialize raw data.
    // Arguments are validated in the XmlWellformedWriter layer

    public override unsafe void WriteRaw(char[] buffer, int index, int count)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      fixed (char* pSrcBegin = &buffer[index])
      {
        WriteRawWithCharChecking(pSrcBegin, pSrcBegin + count);
      }

      textPos = bufPos;
    }

    // Serialize raw data.

    public override unsafe void WriteRaw(string data)
    {
      if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

      fixed (char* pSrcBegin = data)
      {
        WriteRawWithCharChecking(pSrcBegin, pSrcBegin + data.Length);
      }

      textPos = bufPos;
    }

    // Flush all bytes in the buffer to output and close the output stream or writer.
    public override void Close()
    {
      try
      {
        FlushBuffer();
        FlushEncoder();
      }
      finally
      {
        // Future calls to Close or Flush shouldn't write to Stream or Writer
        writeToNull = true;

        if (stream != null)
        {
          try
          {
            stream.Flush();
          }
          finally
          {
            try
            {
              if (closeOutput)
              {
                stream.Close();
              }
            }
            finally
            {
              stream = null;
            }
          }
        }

        else if (writer != null)
        {
          try
          {
            writer.Flush();
          }
          finally
          {
            try
            {
              if (closeOutput)
              {
                writer.Close();
              }
            }
            finally
            {
              writer = null;
            }
          }
        }

      }
    }

    // Flush all characters in the buffer to output and call Flush() on the output object.
    public override void Flush()
    {
      FlushBuffer();
      FlushEncoder();

      if (stream != null)
      {
        stream.Flush();
      }
      else if (writer != null)
      {
        writer.Flush();
      }

    }

    //
    // Implementation methods
    //
    // Flush all characters in the buffer to output.  Do not flush the output object.
    protected virtual void FlushBuffer()
    {
      try
      {
        // Output all characters (except for previous characters stored at beginning of buffer)
        if (!writeToNull)
        {
          if (stream != null)
          {
            if (trackTextContent)
            {
              charEntityFallback.Reset(textContentMarks, lastMarkPos);
              // reset text content tracking

              if ((lastMarkPos & 1) != 0)
              {
                // If the previous buffer ended inside a text content we need to preserve that info
                //   which means the next index to which we write has to be even
                textContentMarks[1] = 1;
                lastMarkPos = 1;
              }
              else
              {
                lastMarkPos = 0;
              }
            }
            EncodeChars(1, bufPos, true);
          }
          else
          {
            // Write text to TextWriter
            writer.Write(bufChars, 1, bufPos - 1);
          }

        }
      }
      catch
      {
        // Future calls to flush (i.e. when Close() is called) don't attempt to write to stream
        writeToNull = true;
        throw;
      }
      finally
      {
        // Move last buffer character to the beginning of the buffer (so that previous character can always be determined)
        bufChars[0] = bufChars[bufPos - 1];

        // Reset buffer position
        textPos = (textPos == bufPos) ? 1 : 0;
        attrEndPos = (attrEndPos == bufPos) ? 1 : 0;
        contentPos = 0;    // Needs to be zero, since overwriting '>' character is no longer possible
        cdataPos = 0;      // Needs to be zero, since overwriting ']]>' characters is no longer possible
        bufPos = 1;        // Buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
        // close an empty element or in CDATA section detection of double ]; _BUFFER[0] will always be 0
      }
    }

    private void EncodeChars(int startOffset, int endOffset, bool writeAllToStream)
    {
      // Write encoded text to stream
      int chEnc;
      int bEnc;
      bool completed;
      while (startOffset < endOffset)
      {
        if (charEntityFallback != null)
        {
          charEntityFallback.StartOffset = startOffset;
        }
        encoder.Convert(bufChars, startOffset, endOffset - startOffset, bufBytes, bufBytesUsed, bufBytes.Length - bufBytesUsed, false, out chEnc, out bEnc, out completed);
        startOffset += chEnc;
        bufBytesUsed += bEnc;
        if (bufBytesUsed >= (bufBytes.Length - 16))
        {
          stream.Write(bufBytes, 0, bufBytesUsed);
          bufBytesUsed = 0;
        }
      }
      if (writeAllToStream && bufBytesUsed > 0)
      {
        stream.Write(bufBytes, 0, bufBytesUsed);
        bufBytesUsed = 0;
      }
    }

    private void FlushEncoder()
    {
      if (stream != null)
      {
        int chEnc;
        int bEnc;
        bool completed;
        // decode no chars, just flush
        encoder.Convert(bufChars, 1, 0, bufBytes, 0, bufBytes.Length, true, out chEnc, out bEnc, out completed);
        if (bEnc != 0)
        {
          stream.Write(bufBytes, 0, bEnc);
        }
      }

    }

    // Serialize text that is part of an attribute value.  The '&', '<', '>', and '"' characters
    // are entitized.

    protected unsafe void WriteAttributeTextBlock(char* pSrc, char* pSrcEnd)
    {

      fixed (char* pDstBegin = bufChars)
      {
        char* pDst = pDstBegin + this.bufPos;

        int ch = 0;
        for (; ; )
        {
          char* pDstEnd = pDst + (pSrcEnd - pSrc);
          if (pDstEnd > pDstBegin + bufLen)
          {
            pDstEnd = pDstBegin + bufLen;
          }

          while (pDst < pDstEnd && (((xmlCharType.charProperties[(ch = *pSrc)] & XmlCharType.fAttrValue) != 0)))
          {

            *pDst = (char)ch;
            pDst++;
            pSrc++;
          }

          // end of value
          if (pSrc >= pSrcEnd)
          {
            break;
          }

          // end of buffer
          if (pDst >= pDstEnd)
          {

            bufPos = (int)(pDst - pDstBegin);
            FlushBuffer();
            pDst = pDstBegin + 1;
            continue;

          }

          // some character needs to be escaped
          switch (ch)
          {
            case '&':
            pDst = AmpEntity(pDst);
            break;
            case '<':
            pDst = LtEntity(pDst);
            break;
            case '>':
            pDst = GtEntity(pDst);
            break;
            case '"':
            pDst = QuoteEntity(pDst);
            break;
            case '\'':
            *pDst = (char)ch;
            pDst++;
            break;
            case (char)0x9:
            if (newLineHandling == NewLineHandling.None)
            {
              *pDst = (char)ch;
              pDst++;
            }
            else
            {
              // escape tab in attributes
              pDst = TabEntity(pDst);
            }
            break;
            case (char)0xD:
            if (newLineHandling == NewLineHandling.None)
            {
              *pDst = (char)ch;
              pDst++;
            }
            else
            {
              // escape new lines in attributes
              pDst = CarriageReturnEntity(pDst);
            }
            break;
            case (char)0xA:
            if (newLineHandling == NewLineHandling.None)
            {
              *pDst = (char)ch;
              pDst++;
            }
            else
            {
              // escape new lines in attributes
              pDst = LineFeedEntity(pDst);
            }
            break;
            default:
            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, true); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
            continue;
          }
          pSrc++;
        }
        bufPos = (int)(pDst - pDstBegin);
      }

    }

    // Serialize text that is part of element content.  The '&', '<', and '>' characters
    // are entitized.

    protected unsafe void WriteElementTextBlock(char* pSrc, char* pSrcEnd)
    {

      fixed (char* pDstBegin = bufChars)
      {
        char* pDst = pDstBegin + this.bufPos;

        int ch = 0;
        for (; ; )
        {
          char* pDstEnd = pDst + (pSrcEnd - pSrc);
          if (pDstEnd > pDstBegin + bufLen)
          {
            pDstEnd = pDstBegin + bufLen;
          }

          while (pDst < pDstEnd && (((xmlCharType.charProperties[(ch = *pSrc)] & XmlCharType.fAttrValue) != 0)))
          {

            *pDst = (char)ch;
            pDst++;
            pSrc++;
          }

          // end of value
          if (pSrc >= pSrcEnd)
          {
            break;
          }

          // end of buffer
          if (pDst >= pDstEnd)
          {

            bufPos = (int)(pDst - pDstBegin);
            FlushBuffer();
            pDst = pDstBegin + 1;
            continue;

          }

          // some character needs to be escaped
          switch (ch)
          {
            case '&':
            pDst = AmpEntity(pDst);
            break;
            case '<':
            pDst = LtEntity(pDst);
            break;
            case '>':
            pDst = GtEntity(pDst);
            break;
            case '"':
            case '\'':
            case (char)0x9:
            *pDst = (char)ch;
            pDst++;
            break;
            case (char)0xA:
            if (newLineHandling == NewLineHandling.Replace)
            {

              pDst = WriteNewLine(pDst);

            }
            else
            {
              *pDst = (char)ch;
              pDst++;
            }
            break;
            case (char)0xD:
            switch (newLineHandling)
            {
              case NewLineHandling.Replace:
              // Replace "\r\n", or "\r" with NewLineChars
              if (pSrc[1] == '\n')
              {
                pSrc++;
              }

              pDst = WriteNewLine(pDst);
              break;

              case NewLineHandling.Entitize:
              // Entitize 0xD
              pDst = CarriageReturnEntity(pDst);
              break;
              case NewLineHandling.None:
              *pDst = (char)ch;
              pDst++;
              break;
            }
            break;
            default:
            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, true); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
            continue;
          }
          pSrc++;
        }
        bufPos = (int)(pDst - pDstBegin);
        textPos = bufPos;
        contentPos = 0;
      }

    }

    protected unsafe void RawText(string s)
    {
      fixed (char* pSrcBegin = s)
      {
        RawText(pSrcBegin, pSrcBegin + s.Length);
      }
    }

    protected unsafe void RawText(char* pSrcBegin, char* pSrcEnd)
    {

      fixed (char* pDstBegin = bufChars)
      {
        char* pDst = pDstBegin + this.bufPos;
        char* pSrc = pSrcBegin;

        int ch = 0;
        for (; ; )
        {
          char* pDstEnd = pDst + (pSrcEnd - pSrc);
          if (pDstEnd > pDstBegin + this.bufLen)
          {
            pDstEnd = pDstBegin + this.bufLen;
          }

          while (pDst < pDstEnd && ((ch = *pSrc) < XmlCharType.SurHighStart))
          {

            pSrc++;
            *pDst = (char)ch;
            pDst++;
          }

          // end of value
          if (pSrc >= pSrcEnd)
          {
            break;
          }

          // end of buffer
          if (pDst >= pDstEnd)
          {

            bufPos = (int)(pDst - pDstBegin);
            FlushBuffer();
            pDst = pDstBegin + 1;
            continue;

          }

          if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
        }

        bufPos = (int)(pDst - pDstBegin);
      }

    }

    protected unsafe void WriteRawWithCharChecking(char* pSrcBegin, char* pSrcEnd)
    {

      fixed (char* pDstBegin = bufChars)
      {
        char* pSrc = pSrcBegin;
        char* pDst = pDstBegin + bufPos;

        int ch = 0;
        for (; ; )
        {
          char* pDstEnd = pDst + (pSrcEnd - pSrc);
          if (pDstEnd > pDstBegin + bufLen)
          {
            pDstEnd = pDstBegin + bufLen;
          }

          while (pDst < pDstEnd && (((xmlCharType.charProperties[(ch = *pSrc)] & XmlCharType.fText) != 0)))
          {

            *pDst = (char)ch;
            pDst++;
            pSrc++;
          }

          // end of value
          if (pSrc >= pSrcEnd)
          {
            break;
          }

          // end of buffer
          if (pDst >= pDstEnd)
          {

            bufPos = (int)(pDst - pDstBegin);
            FlushBuffer();
            pDst = pDstBegin + 1;
            continue;

          }

          // handle special characters
          switch (ch)
          {
            case ']':
            case '<':
            case '&':
            case (char)0x9:
            *pDst = (char)ch;
            pDst++;
            break;
            case (char)0xD:
            if (newLineHandling == NewLineHandling.Replace)
            {
              // Normalize "\r\n", or "\r" to NewLineChars
              if (pSrc[1] == '\n')
              {
                pSrc++;
              }

              pDst = WriteNewLine(pDst);

            }
            else
            {
              *pDst = (char)ch;
              pDst++;
            }
            break;
            case (char)0xA:
            if (newLineHandling == NewLineHandling.Replace)
            {

              pDst = WriteNewLine(pDst);

            }
            else
            {
              *pDst = (char)ch;
              pDst++;
            }
            break;
            default:
            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
            continue;
          }
          pSrc++;
        }
        bufPos = (int)(pDst - pDstBegin);
      }

    }

    protected unsafe void WriteCommentOrPi(string text, int stopChar)
    {

      if (text.Length == 0)
      {
        if (bufPos >= bufLen)
        {
          FlushBuffer();
        }
        return;
      }
      // write text
      fixed (char* pSrcBegin = text)

      fixed (char* pDstBegin = bufChars)
      {
        char* pSrc = pSrcBegin;

        char* pSrcEnd = pSrcBegin + text.Length;

        char* pDst = pDstBegin + bufPos;

        int ch = 0;
        for (; ; )
        {
          char* pDstEnd = pDst + (pSrcEnd - pSrc);
          if (pDstEnd > pDstBegin + bufLen)
          {
            pDstEnd = pDstBegin + bufLen;
          }

          while (pDst < pDstEnd && (((xmlCharType.charProperties[(ch = *pSrc)] & XmlCharType.fText) != 0) && ch != stopChar))
          {

            *pDst = (char)ch;
            pDst++;
            pSrc++;
          }

          // end of value
          if (pSrc >= pSrcEnd)
          {
            break;
          }

          // end of buffer
          if (pDst >= pDstEnd)
          {

            bufPos = (int)(pDst - pDstBegin);
            FlushBuffer();
            pDst = pDstBegin + 1;
            continue;

          }

          // handle special characters
          switch (ch)
          {
            case '-':
            *pDst = (char)'-';
            pDst++;
            if (ch == stopChar)
            {
              // Insert space between adjacent dashes or before comment's end dashes
              if (pSrc + 1 == pSrcEnd || *(pSrc + 1) == '-')
              {
                *pDst = (char)' ';
                pDst++;
              }
            }
            break;
            case '?':
            *pDst = (char)'?';
            pDst++;
            if (ch == stopChar)
            {
              // Processing instruction: insert space between adjacent '?' and '>' 
              if (pSrc + 1 < pSrcEnd && *(pSrc + 1) == '>')
              {
                *pDst = (char)' ';
                pDst++;
              }
            }
            break;
            case ']':
            *pDst = (char)']';
            pDst++;
            break;
            case (char)0xD:
            if (newLineHandling == NewLineHandling.Replace)
            {
              // Normalize "\r\n", or "\r" to NewLineChars
              if (pSrc[1] == '\n')
              {
                pSrc++;
              }

              pDst = WriteNewLine(pDst);

            }
            else
            {
              *pDst = (char)ch;
              pDst++;
            }
            break;
            case (char)0xA:
            if (newLineHandling == NewLineHandling.Replace)
            {

              pDst = WriteNewLine(pDst);

            }
            else
            {
              *pDst = (char)ch;
              pDst++;
            }
            break;
            case '<':
            case '&':
            case (char)0x9:
            *pDst = (char)ch;
            pDst++;
            break;
            default:
            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
            continue;
          }
          pSrc++;
        }
        bufPos = (int)(pDst - pDstBegin);
      }

    }

    protected unsafe void WriteCDataSection(string text)
    {
      if (text.Length == 0)
      {
        if (bufPos >= bufLen)
        {
          FlushBuffer();
        }
        return;
      }

      // write text

      fixed (char* pSrcBegin = text)

      fixed (char* pDstBegin = bufChars)
      {
        char* pSrc = pSrcBegin;

        char* pSrcEnd = pSrcBegin + text.Length;

        char* pDst = pDstBegin + bufPos;

        int ch = 0;
        for (; ; )
        {
          char* pDstEnd = pDst + (pSrcEnd - pSrc);
          if (pDstEnd > pDstBegin + bufLen)
          {
            pDstEnd = pDstBegin + bufLen;
          }

          while (pDst < pDstEnd && (((xmlCharType.charProperties[(ch = *pSrc)] & XmlCharType.fAttrValue) != 0) && ch != ']'))
          {

            *pDst = (char)ch;
            pDst++;
            pSrc++;
          }

          // end of value
          if (pSrc >= pSrcEnd)
          {
            break;
          }

          // end of buffer
          if (pDst >= pDstEnd)
          {

            bufPos = (int)(pDst - pDstBegin);
            FlushBuffer();
            pDst = pDstBegin + 1;
            continue;

          }

          // handle special characters
          switch (ch)
          {
            case '>':
            if (hadDoubleBracket && pDst[-1] == (char)']')
            {   // pDst[-1] will always correct - there is a padding character at _BUFFER[0]
              // The characters "]]>" were found within the CData text
              pDst = RawEndCData(pDst);
              pDst = RawStartCData(pDst);
            }
            *pDst = (char)'>';
            pDst++;
            break;
            case ']':
            if (pDst[-1] == (char)']')
            {   // pDst[-1] will always correct - there is a padding character at _BUFFER[0]
              hadDoubleBracket = true;
            }
            else
            {
              hadDoubleBracket = false;
            }
            *pDst = (char)']';
            pDst++;
            break;
            case (char)0xD:
            if (newLineHandling == NewLineHandling.Replace)
            {
              // Normalize "\r\n", or "\r" to NewLineChars
              if (pSrc[1] == '\n')
              {
                pSrc++;
              }

              pDst = WriteNewLine(pDst);

            }
            else
            {
              *pDst = (char)ch;
              pDst++;
            }
            break;
            case (char)0xA:
            if (newLineHandling == NewLineHandling.Replace)
            {

              pDst = WriteNewLine(pDst);

            }
            else
            {
              *pDst = (char)ch;
              pDst++;
            }
            break;
            case '&':
            case '<':
            case '"':
            case '\'':
            case (char)0x9:
            *pDst = (char)ch;
            pDst++;
            break;
            default:
            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
            continue;
          }
          pSrc++;
        }
        bufPos = (int)(pDst - pDstBegin);
      }

    }

    private static unsafe char* EncodeSurrogate(char* pSrc, char* pSrcEnd, char* pDst)
    {
      int ch = *pSrc;
      if (ch <= XmlCharType.SurHighEnd)
      {
        if (pSrc + 1 < pSrcEnd)
        {
          int lowChar = pSrc[1];
          if (lowChar >= XmlCharType.SurLowStart)
          {

            pDst[0] = (char)ch;
            pDst[1] = (char)lowChar;
            pDst += 2;

            return pDst;
          }
          throw XmlConvert.CreateInvalidSurrogatePairException((char)lowChar, (char)ch);
        }
        throw new ArgumentException(Res.GetString(Res.Xml_InvalidSurrogateMissingLowChar));
      }
      throw XmlConvert.CreateInvalidHighSurrogateCharException((char)ch);
    }

    private unsafe char* InvalidXmlChar(int ch, char* pDst, bool entitize)
    {
      if (checkCharacters)
      {
        // This method will never be called on surrogates, so it is ok to pass in '\0' to the CreateInvalidCharException
        throw XmlConvert.CreateInvalidCharException((char)ch, '\0');
      }
      else
      {
        if (entitize)
        {
          return CharEntity(pDst, (char)ch);
        }
        else
        {

          *pDst = (char)ch;
          pDst++;

          return pDst;
        }
      }
    }

    internal unsafe void EncodeChar(ref char* pSrc, char* pSrcEnd, ref char* pDst)
    {
      int ch = *pSrc;
      if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
    }

    protected void ChangeTextContentMark(bool value)
    {
      inTextContent = value;
      if (lastMarkPos + 1 == textContentMarks.Length)
      {
        GrowTextContentMarks();
      }
      textContentMarks[++lastMarkPos] = this.bufPos;
    }

    private void GrowTextContentMarks()
    {
      int[] newTextContentMarks = new int[textContentMarks.Length * 2];
      Array.Copy(textContentMarks, newTextContentMarks, textContentMarks.Length);
      textContentMarks = newTextContentMarks;
    }

    // Write NewLineChars to the specified buffer position and return an updated position.

    protected unsafe char* WriteNewLine(char* pDst)
    {
      fixed (char* pDstBegin = bufChars)
      {
        bufPos = (int)(pDst - pDstBegin);
        // Let RawText do the real work
        RawText(newLineChars);
        return pDstBegin + bufPos;
      }
    }

    // Following methods do not check whether pDst is beyond the bufSize because the buffer was allocated with a OVERFLOW to accomodate
    // for the writes of small constant-length string as below.

    // Entitize '<' as "&lt;".  Return an updated pointer.

    protected static unsafe char* LtEntity(char* pDst)
    {
      pDst[0] = (char)'&';
      pDst[1] = (char)'l';
      pDst[2] = (char)'t';
      pDst[3] = (char)';';
      return pDst + 4;
    }

    // Entitize '>' as "&gt;".  Return an updated pointer.

    protected static unsafe char* GtEntity(char* pDst)
    {
      pDst[0] = (char)'&';
      pDst[1] = (char)'g';
      pDst[2] = (char)'t';
      pDst[3] = (char)';';
      return pDst + 4;
    }

    // Entitize '&' as "&amp;".  Return an updated pointer.

    protected static unsafe char* AmpEntity(char* pDst)
    {
      pDst[0] = (char)'&';
      pDst[1] = (char)'a';
      pDst[2] = (char)'m';
      pDst[3] = (char)'p';
      pDst[4] = (char)';';
      return pDst + 5;
    }

    // Entitize '"' as "&quot;".  Return an updated pointer.

    protected static unsafe char* QuoteEntity(char* pDst)
    {
      pDst[0] = (char)'&';
      pDst[1] = (char)'q';
      pDst[2] = (char)'u';
      pDst[3] = (char)'o';
      pDst[4] = (char)'t';
      pDst[5] = (char)';';
      return pDst + 6;
    }

    // Entitize '\t' as "&#x9;".  Return an updated pointer.

    protected static unsafe char* TabEntity(char* pDst)
    {
      pDst[0] = (char)'&';
      pDst[1] = (char)'#';
      pDst[2] = (char)'x';
      pDst[3] = (char)'9';
      pDst[4] = (char)';';
      return pDst + 5;
    }

    // Entitize 0xa as "&#xA;".  Return an updated pointer.

    protected static unsafe char* LineFeedEntity(char* pDst)
    {
      pDst[0] = (char)'&';
      pDst[1] = (char)'#';
      pDst[2] = (char)'x';
      pDst[3] = (char)'A';
      pDst[4] = (char)';';
      return pDst + 5;
    }

    // Entitize 0xd as "&#xD;".  Return an updated pointer.

    protected static unsafe char* CarriageReturnEntity(char* pDst)
    {
      pDst[0] = (char)'&';
      pDst[1] = (char)'#';
      pDst[2] = (char)'x';
      pDst[3] = (char)'D';
      pDst[4] = (char)';';
      return pDst + 5;
    }

    private static unsafe char* CharEntity(char* pDst, char ch)
    {
      string s = ((int)ch).ToString("X", NumberFormatInfo.InvariantInfo);
      pDst[0] = (char)'&';
      pDst[1] = (char)'#';
      pDst[2] = (char)'x';
      pDst += 3;

      fixed (char* pSrc = s)
      {
        char* pS = pSrc;
        while ((*pDst++ = (char)*pS++) != 0) ;
      }

      pDst[-1] = (char)';';
      return pDst;
    }

    // Write "<![CDATA[" to the specified buffer.  Return an updated pointer.

    protected static unsafe char* RawStartCData(char* pDst)
    {
      pDst[0] = (char)'<';
      pDst[1] = (char)'!';
      pDst[2] = (char)'[';
      pDst[3] = (char)'C';
      pDst[4] = (char)'D';
      pDst[5] = (char)'A';
      pDst[6] = (char)'T';
      pDst[7] = (char)'A';
      pDst[8] = (char)'[';
      return pDst + 9;
    }

    // Write "]]>" to the specified buffer.  Return an updated pointer.

    protected static unsafe char* RawEndCData(char* pDst)
    {
      pDst[0] = (char)']';
      pDst[1] = (char)']';
      pDst[2] = (char)'>';
      return pDst + 3;
    }

    protected unsafe void ValidateContentChars(string chars, string propertyName, bool allowOnlyWhitespace)
    {

      if (allowOnlyWhitespace)
      {
        if (!xmlCharType.IsOnlyWhitespace(chars))
        {
          throw new ArgumentException(Res.GetString(Res.Xml_IndentCharsNotWhitespace, propertyName));
        }
      }
      else
      {
        string error = null;
        for (int i = 0; i < chars.Length; i++)
        {
          if (!xmlCharType.IsTextChar(chars[i]))
          {
            switch (chars[i])
            {
              case '\n':
              case '\r':
              case '\t':
              continue;
              case '<':
              case '&':
              case ']':
              error = Res.GetString(Res.Xml_InvalidCharacter, XmlException.BuildCharExceptionArgs(chars, i));
              goto Error;
              default:
              if (XmlCharType.IsHighSurrogate(chars[i]))
              {
                if (i + 1 < chars.Length)
                {
                  if (XmlCharType.IsLowSurrogate(chars[i + 1]))
                  {
                    i++;
                    continue;
                  }
                }
                error = Res.GetString(Res.Xml_InvalidSurrogateMissingLowChar);
                goto Error;
              }
              else if (XmlCharType.IsLowSurrogate(chars[i]))
              {
                error = Res.GetString(Res.Xml_InvalidSurrogateHighChar, ((uint)chars[i]).ToString("X", CultureInfo.InvariantCulture));
                goto Error;
              }
              continue;
            }
          }
        }
        return;

      Error:
        throw new ArgumentException(Res.GetString(Res.Xml_InvalidCharsInIndent, new string[] { propertyName, error }));
      }
    }

  }
}
