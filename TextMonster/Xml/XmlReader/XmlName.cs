namespace TextMonster.Xml.XmlReader
{
  internal class XmlName : IXmlSchemaInfo
  {
    string prefix;
    string localName;
    string ns;
    string name;
    int hashCode;
    internal XmlDocument ownerDoc;
    internal XmlName next;

    public static XmlName Create(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next, IXmlSchemaInfo schemaInfo)
    {
      if (schemaInfo == null)
      {
        return new XmlName(prefix, localName, ns, hashCode, ownerDoc, next);
      }
      else
      {
        return new XmlNameEx(prefix, localName, ns, hashCode, ownerDoc, next, schemaInfo);
      }
    }

    internal XmlName(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next)
    {
      this.prefix = prefix;
      this.localName = localName;
      this.ns = ns;
      this.name = null;
      this.hashCode = hashCode;
      this.ownerDoc = ownerDoc;
      this.next = next;
    }

    public string LocalName
    {
      get
      {
        return localName;
      }
    }

    public string NamespaceURI
    {
      get
      {
        return ns;
      }
    }

    public string Prefix
    {
      get
      {
        return prefix;
      }
    }

    public int HashCode
    {
      get
      {
        return hashCode;
      }
    }

    public XmlDocument OwnerDocument
    {
      get
      {
        return ownerDoc;
      }
    }

    public string Name
    {
      get
      {
        if (name == null)
        {
          Debug.Assert(prefix != null);
          if (prefix.Length > 0)
          {
            if (localName.Length > 0)
            {
              string n = string.Concat(prefix, ":", localName);
              lock (ownerDoc.NameTable)
              {
                if (name == null)
                {
                  name = ownerDoc.NameTable.Add(n);
                }
              }
            }
            else
            {
              name = prefix;
            }
          }
          else
          {
            name = localName;
          }
          Debug.Assert(Ref.Equal(name, ownerDoc.NameTable.Get(name)));
        }
        return name;
      }
    }

    public virtual XmlSchemaValidity Validity
    {
      get
      {
        return XmlSchemaValidity.NotKnown;
      }
    }

    public virtual bool IsDefault
    {
      get
      {
        return false;
      }
    }

    public virtual bool IsNil
    {
      get
      {
        return false;
      }
    }

    public virtual XmlSchemaSimpleType MemberType
    {
      get
      {
        return null;
      }
    }

    public virtual XmlSchemaType SchemaType
    {
      get
      {
        return null;
      }
    }

    public virtual XmlSchemaElement SchemaElement
    {
      get
      {
        return null;
      }
    }

    public virtual XmlSchemaAttribute SchemaAttribute
    {
      get
      {
        return null;
      }
    }

    public virtual bool Equals(IXmlSchemaInfo schemaInfo)
    {
      return schemaInfo == null;
    }

    public static int GetHashCode(string name)
    {
      int hashCode = 0;
      if (name != null)
      {
        for (int i = name.Length - 1; i >= 0; i--)
        {
          char ch = name[i];
          if (ch == ':') break;
          hashCode += (hashCode << 7) ^ ch;
        }
        hashCode -= hashCode >> 17;
        hashCode -= hashCode >> 11;
        hashCode -= hashCode >> 5;
      }
      return hashCode;
    }
  }
}
