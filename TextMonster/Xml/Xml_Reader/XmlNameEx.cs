namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class XmlNameEx : XmlName
  {
    byte flags;
    XmlSchemaSimpleType memberType;
    XmlSchemaType schemaType;
    object decl;

    // flags
    // 0,1  : Validity
    // 2    : IsDefault
    // 3    : IsNil
    const byte ValidityMask = 0x03;
    const byte IsDefaultBit = 0x04;
    const byte IsNilBit = 0x08;

    internal XmlNameEx(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next, IXmlSchemaInfo schemaInfo)
      : base(prefix, localName, ns, hashCode, ownerDoc, next)
    {
      SetValidity(schemaInfo.Validity);
      SetIsDefault(schemaInfo.IsDefault);
      SetIsNil(schemaInfo.IsNil);
      memberType = schemaInfo.MemberType;
      schemaType = schemaInfo.SchemaType;
      decl = schemaInfo.SchemaElement != null
             ? (object)schemaInfo.SchemaElement
             : (object)schemaInfo.SchemaAttribute;
    }

    public override XmlSchemaValidity Validity
    {
      get
      {
        return ownerDoc.CanReportValidity ? (XmlSchemaValidity)(flags & ValidityMask) : XmlSchemaValidity.NotKnown;
      }
    }

    public override bool IsDefault
    {
      get
      {
        return (flags & IsDefaultBit) != 0;
      }
    }

    public override bool IsNil
    {
      get
      {
        return (flags & IsNilBit) != 0;
      }
    }

    public override XmlSchemaSimpleType MemberType
    {
      get
      {
        return memberType;
      }
    }

    public override XmlSchemaType SchemaType
    {
      get
      {
        return schemaType;
      }
    }

    public override XmlSchemaElement SchemaElement
    {
      get
      {
        return decl as XmlSchemaElement;
      }
    }

    public override XmlSchemaAttribute SchemaAttribute
    {
      get
      {
        return decl as XmlSchemaAttribute;
      }
    }

    public void SetValidity(XmlSchemaValidity value)
    {
      flags = (byte)((flags & ~ValidityMask) | (byte)(value));
    }

    public void SetIsDefault(bool value)
    {
      if (value) flags = (byte)(flags | IsDefaultBit);
      else flags = (byte)(flags & ~IsDefaultBit);
    }

    public void SetIsNil(bool value)
    {
      if (value) flags = (byte)(flags | IsNilBit);
      else flags = (byte)(flags & ~IsNilBit);
    }

    public override bool Equals(IXmlSchemaInfo schemaInfo)
    {
      if (schemaInfo != null
          && schemaInfo.Validity == (XmlSchemaValidity)(flags & ValidityMask)
          && schemaInfo.IsDefault == ((flags & IsDefaultBit) != 0)
          && schemaInfo.IsNil == ((flags & IsNilBit) != 0)
          && (object)schemaInfo.MemberType == (object)memberType
          && (object)schemaInfo.SchemaType == (object)schemaType
          && (object)schemaInfo.SchemaElement == (object)(decl as XmlSchemaElement)
          && (object)schemaInfo.SchemaAttribute == (object)(decl as XmlSchemaAttribute))
      {
        return true;
      }
      return false;
    }
  }
}
