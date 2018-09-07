using System;
using System.IO;

namespace TextMonster.Xml.Xml_Reader
{
  // Represents a writer that provides fast non-cached forward-only way of generating XML streams containing XML documents 
  // that conform to the W3C Extensible Markup Language (XML) 1.0 specification and the Namespaces in XML specification.
  public abstract partial class XmlWriter : IDisposable
  {

    // Helper buffer for WriteNode(XmlReader, bool)
    char[] writeNodeBuffer;

    // Returns the settings describing the features of the the writer. Returns null for V1 XmlWriters (XmlTextWriter).
    public virtual XmlWriterSettings Settings
    {
      get
      {
        return null;
      }
    }

    // Write methods
    // Writes out the XML declaration with the version "1.0".

    public abstract void WriteStartDocument();

    public abstract void WriteEndDocument();

    // Writes out the DOCTYPE declaration with the specified name and optional attributes.

    public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

    public abstract void WriteStartElement(string prefix, string localName, string ns);

    public abstract void WriteEndElement();

    public abstract void WriteFullEndElement();

    public abstract void WriteStartAttribute(string prefix, string localName, string ns);

    public abstract void WriteEndAttribute();

    // Writes out a <![CDATA[...]]>; block containing the specified text.

    public abstract void WriteCData(string text);

    // Writes out a comment <!--...-->; containing the specified text.

    public abstract void WriteComment(string text);

    // Writes out a processing instruction with a space between the name and text as follows: <?name text?>

    public abstract void WriteProcessingInstruction(string name, string text);

    // Writes out an entity reference as follows: "&"+name+";".

    public abstract void WriteEntityRef(string name);

    // Forces the generation of a character entity for the specified Unicode character value.

    public abstract void WriteCharEntity(char ch);

    // Writes out the given whitespace.

    public abstract void WriteWhitespace(string ws);

    // Writes out the specified text content.

    public abstract void WriteString(string text);

    // Write out the given surrogate pair as an entity reference.

    public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

    // Writes out the specified text content.

    public abstract void WriteChars(char[] buffer, int index, int count);

    // Writes raw markup from the given character buffer.

    public abstract void WriteRaw(char[] buffer, int index, int count);

    // Writes raw markup from the given string.

    public abstract void WriteRaw(string data);

    // Encodes the specified binary bytes as base64 and writes out the resulting text.

    public abstract void WriteBase64(byte[] buffer, int index, int count);

    // Encodes the specified binary bytes as binhex and writes out the resulting text.
    public virtual void WriteBinHex(byte[] buffer, int index, int count)
    {
      BinHexEncoder.Encode(buffer, index, count, this);
    }

    // Returns the state of the XmlWriter.
    public abstract WriteState WriteState { get; }

    // Closes the XmlWriter and the underlying stream/TextReader (if Settings.CloseOutput is true).
    public virtual void Close() { }

    // Flushes data that is in the internal buffers into the underlying streams/TextReader and flushes the stream/TextReader.

    public abstract void Flush();

    public virtual void WriteValue(object value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      WriteString(XmlUntypedConverter.Untyped.ToString(value, null));
    }

    // Writes out the specified value.
    public virtual void WriteValue(string value)
    {
      if (value == null)
      {

        return;

      }
      WriteString(value);
    }

    // Writes out the specified value.
    public virtual void WriteValue(bool value)
    {
      WriteString(XmlConvert.ToString(value));
    }

    // Writes out the specified value.
    public virtual void WriteValue(DateTime value)
    {
      WriteString(XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind));
    }

    // Writes out the specified value.
    public virtual void WriteValue(DateTimeOffset value)
    {
      // Under Win8P, WriteValue(DateTime) will invoke this overload, but custom writers
      // might not have implemented it. This base implementation should call WriteValue(DateTime).
      // The following conversion results in the same string as calling ToString with DateTimeOffset.
      if (value.Offset != TimeSpan.Zero)
      {
        WriteValue(value.LocalDateTime);
      }
      else
      {
        WriteValue(value.UtcDateTime);
      }
    }

    // Writes out the specified value.
    public virtual void WriteValue(double value)
    {
      WriteString(XmlConvert.ToString(value));
    }

    // Writes out the specified value.
    public virtual void WriteValue(float value)
    {
      WriteString(XmlConvert.ToString(value));
    }

    // Writes out the specified value.
    public virtual void WriteValue(decimal value)
    {
      WriteString(XmlConvert.ToString(value));
    }

    // Writes out the specified value.
    public virtual void WriteValue(int value)
    {
      WriteString(XmlConvert.ToString(value));
    }

    // Writes out the specified value.
    public virtual void WriteValue(long value)
    {
      WriteString(XmlConvert.ToString(value));
    }

    public void Dispose()
    {
      Dispose(true);
    }

    // Dispose the underline stream objects (calls Close on the XmlWriter)
    protected void Dispose(bool disposing)
    {
      if (disposing && WriteState != WriteState.Closed)
      {
        Close();
      }
    }

    public static XmlWriter Create(Stream output, XmlWriterSettings settings)
    {
      if (settings == null)
      {
        settings = new XmlWriterSettings();
      }
      return settings.CreateWriter(output);
    }

    public static XmlWriter Create(TextWriter output, XmlWriterSettings settings)
    {
      if (settings == null)
      {
        settings = new XmlWriterSettings();
      }
      return settings.CreateWriter(output);
    }
  }
}
