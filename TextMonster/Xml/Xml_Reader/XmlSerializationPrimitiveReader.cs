using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlSerializationPrimitiveReader : XmlSerializationReader
  {

    internal object Read_string()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id1_string && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          if (ReadNull())
          {
            o = null;
          }
          else
          {
            o = Reader.ReadElementString();
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_int()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id3_int && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToInt32(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_boolean()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id4_boolean && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToBoolean(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_short()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id5_short && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToInt16(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_long()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id6_long && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToInt64(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_float()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id7_float && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToSingle(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_double()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id8_double && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToDouble(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_decimal()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id9_decimal && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToDecimal(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_dateTime()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id10_dateTime && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = ToDateTime(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_unsignedByte()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id11_unsignedByte && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToByte(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_byte()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id12_byte && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToSByte(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_unsignedShort()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id13_unsignedShort && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToUInt16(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_unsignedInt()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id14_unsignedInt && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToUInt32(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_unsignedLong()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id15_unsignedLong && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToUInt64(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_base64Binary()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id16_base64Binary && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          if (ReadNull())
          {
            o = null;
          }
          else
          {
            o = ToByteArrayBase64(false);
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_guid()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id17_guid && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = XmlConvert.ToGuid(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_TimeSpan()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id19_TimeSpan && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          if (Reader.IsEmptyElement)
          {
            Reader.Skip();
            //For backward compatibiity 
            //When using old serializer, the serialized TimeSpan value is empty string
            o = default(TimeSpan);
          }
          else
          {
            o = XmlConvert.ToTimeSpan(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_char()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id18_char && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          {
            o = ToChar(Reader.ReadElementString());
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    internal object Read_QName()
    {
      object o = null;
      Reader.MoveToContent();
      if (Reader.NodeType == XmlNodeType.Element)
      {
        if (((object)Reader.LocalName == (object)id1_QName && (object)Reader.NamespaceURI == (object)id2_Item))
        {
          if (ReadNull())
          {
            o = null;
          }
          else
          {
            o = ReadElementQualifiedName();
          }
        }
        else
        {
          throw CreateUnknownNodeException();
        }
      }
      else
      {
        UnknownNode(null);
      }
      return (object)o;
    }

    protected override void InitCallbacks()
    {
    }

    System.String id4_boolean;
    System.String id14_unsignedInt;
    System.String id15_unsignedLong;
    System.String id7_float;
    System.String id10_dateTime;
    System.String id6_long;
    System.String id9_decimal;
    System.String id8_double;
    System.String id17_guid;
    System.String id19_TimeSpan;
    System.String id2_Item;
    System.String id13_unsignedShort;
    System.String id18_char;
    System.String id3_int;
    System.String id12_byte;
    System.String id16_base64Binary;
    System.String id11_unsignedByte;
    System.String id5_short;
    System.String id1_string;
    System.String id1_QName;

    protected override void InitIDs()
    {
      id4_boolean = Reader.NameTable.Add(@"boolean");
      id14_unsignedInt = Reader.NameTable.Add(@"unsignedInt");
      id15_unsignedLong = Reader.NameTable.Add(@"unsignedLong");
      id7_float = Reader.NameTable.Add(@"float");
      id10_dateTime = Reader.NameTable.Add(@"dateTime");
      id6_long = Reader.NameTable.Add(@"long");
      id9_decimal = Reader.NameTable.Add(@"decimal");
      id8_double = Reader.NameTable.Add(@"double");
      id17_guid = Reader.NameTable.Add(@"guid");
      id2_Item = Reader.NameTable.Add(@"");
      id13_unsignedShort = Reader.NameTable.Add(@"unsignedShort");
      id18_char = Reader.NameTable.Add(@"char");
      id3_int = Reader.NameTable.Add(@"int");
      id12_byte = Reader.NameTable.Add(@"byte");
      id16_base64Binary = Reader.NameTable.Add(@"base64Binary");
      id11_unsignedByte = Reader.NameTable.Add(@"unsignedByte");
      id5_short = Reader.NameTable.Add(@"short");
      id1_string = Reader.NameTable.Add(@"string");
      id1_QName = Reader.NameTable.Add(@"QName");
    }
  }
}