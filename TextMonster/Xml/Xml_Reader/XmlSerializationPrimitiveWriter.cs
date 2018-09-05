using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlSerializationPrimitiveWriter : XmlSerializationWriter
  {

    internal void Write_string(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteNullTagLiteral(@"string", @"");
        return;
      }
      TopLevelElement();
      WriteNullableStringLiteral(@"string", @"", ((System.String)o));
    }

    internal void Write_int(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"int", @"");
        return;
      }
      WriteElementStringRaw(@"int", @"", XmlConvert.ToString((System.Int32)((System.Int32)o)));
    }

    internal void Write_boolean(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"boolean", @"");
        return;
      }
      WriteElementStringRaw(@"boolean", @"", XmlConvert.ToString((System.Boolean)((System.Boolean)o)));
    }

    internal void Write_short(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"short", @"");
        return;
      }
      WriteElementStringRaw(@"short", @"", XmlConvert.ToString((System.Int16)((System.Int16)o)));
    }

    internal void Write_long(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"long", @"");
        return;
      }
      WriteElementStringRaw(@"long", @"", XmlConvert.ToString((System.Int64)((System.Int64)o)));
    }

    internal void Write_float(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"float", @"");
        return;
      }
      WriteElementStringRaw(@"float", @"", XmlConvert.ToString((System.Single)((System.Single)o)));
    }

    internal void Write_double(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"double", @"");
        return;
      }
      WriteElementStringRaw(@"double", @"", XmlConvert.ToString((System.Double)((System.Double)o)));
    }

    internal void Write_decimal(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"decimal", @"");
        return;
      }
      WriteElementStringRaw(@"decimal", @"", XmlConvert.ToString((System.Decimal)((System.Decimal)o)));
    }

    internal void Write_dateTime(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"dateTime", @"");
        return;
      }
      WriteElementStringRaw(@"dateTime", @"", FromDateTime(((System.DateTime)o)));
    }

    internal void Write_unsignedByte(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"unsignedByte", @"");
        return;
      }
      WriteElementStringRaw(@"unsignedByte", @"", XmlConvert.ToString((System.Byte)((System.Byte)o)));
    }

    internal void Write_byte(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"byte", @"");
        return;
      }
      WriteElementStringRaw(@"byte", @"", XmlConvert.ToString((System.SByte)((System.SByte)o)));
    }

    internal void Write_unsignedShort(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"unsignedShort", @"");
        return;
      }
      WriteElementStringRaw(@"unsignedShort", @"", XmlConvert.ToString((System.UInt16)((System.UInt16)o)));
    }

    internal void Write_unsignedInt(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"unsignedInt", @"");
        return;
      }
      WriteElementStringRaw(@"unsignedInt", @"", XmlConvert.ToString((System.UInt32)((System.UInt32)o)));
    }

    internal void Write_unsignedLong(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"unsignedLong", @"");
        return;
      }
      WriteElementStringRaw(@"unsignedLong", @"", XmlConvert.ToString((System.UInt64)((System.UInt64)o)));
    }

    internal void Write_base64Binary(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteNullTagLiteral(@"base64Binary", @"");
        return;
      }
      TopLevelElement();
      WriteNullableStringLiteralRaw(@"base64Binary", @"", FromByteArrayBase64(((System.Byte[])o)));
    }

    internal void Write_guid(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"guid", @"");
        return;
      }
      WriteElementStringRaw(@"guid", @"", XmlConvert.ToString((System.Guid)((System.Guid)o)));
    }

    internal void Write_TimeSpan(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"TimeSpan", @"");
        return;
      }
      TimeSpan timeSpan = (TimeSpan)o;
      WriteElementStringRaw(@"TimeSpan", @"", XmlConvert.ToString(timeSpan));
    }

    internal void Write_char(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteEmptyTag(@"char", @"");
        return;
      }
      WriteElementString(@"char", @"", FromChar(((System.Char)o)));
    }

    internal void Write_QName(object o)
    {
      WriteStartDocument();
      if (o == null)
      {
        WriteNullTagLiteral(@"QName", @"");
        return;
      }
      TopLevelElement();
      WriteNullableQualifiedNameLiteral(@"QName", @"", ((XmlQualifiedName)o));
    }

    protected override void InitCallbacks()
    {
    }
  }
}