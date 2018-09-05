namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class TypeMapping : Mapping
  {
    TypeDesc typeDesc;
    string typeNs;
    string typeName;
    bool referencedByElement;
    bool referencedByTopLevelElement;
    bool includeInSchema = true;
    bool reference = false;

    internal bool ReferencedByTopLevelElement
    {
      get { return referencedByTopLevelElement; }
      set { referencedByTopLevelElement = value; }
    }

    internal bool ReferencedByElement
    {
      get { return referencedByElement || referencedByTopLevelElement; }
      set { referencedByElement = value; }
    }
    internal string Namespace
    {
      get { return typeNs; }
      set { typeNs = value; }
    }

    internal string TypeName
    {
      get { return typeName; }
      set { typeName = value; }
    }

    internal TypeDesc TypeDesc
    {
      get { return typeDesc; }
      set { typeDesc = value; }
    }

    internal bool IncludeInSchema
    {
      get { return includeInSchema; }
      set { includeInSchema = value; }
    }

    internal virtual bool IsList
    {
      get { return false; }
      set { }
    }

    internal bool IsReference
    {
      get { return reference; }
      set { reference = value; }
    }

    internal bool IsAnonymousType
    {
      get { return typeName == null || typeName.Length == 0; }
    }

    internal virtual string DefaultElementName
    {
      get { return IsAnonymousType ? XmlConvert.EncodeLocalName(typeDesc.Name) : typeName; }
    }
  }
}