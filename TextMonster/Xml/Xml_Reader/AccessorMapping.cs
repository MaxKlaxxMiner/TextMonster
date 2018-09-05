namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class AccessorMapping : Mapping
  {
    TypeDesc typeDesc;
    AttributeAccessor attribute;
    ElementAccessor[] elements;
    ElementAccessor[] sortedElements;
    TextAccessor text;
    ChoiceIdentifierAccessor choiceIdentifier;
    XmlnsAccessor xmlns;
    bool ignore;

    internal AccessorMapping()
    { }

    protected AccessorMapping(AccessorMapping mapping)
      : base(mapping)
    {
      this.typeDesc = mapping.typeDesc;
      this.attribute = mapping.attribute;
      this.elements = mapping.elements;
      this.sortedElements = mapping.sortedElements;
      this.text = mapping.text;
      this.choiceIdentifier = mapping.choiceIdentifier;
      this.xmlns = mapping.xmlns;
      this.ignore = mapping.ignore;
    }

    internal bool IsText
    {
      get { return text != null && (elements == null || elements.Length == 0); }
    }

    internal bool IsParticle
    {
      get { return (elements != null && elements.Length > 0); }
    }

    internal TypeDesc TypeDesc
    {
      get { return typeDesc; }
      set { typeDesc = value; }
    }

    internal AttributeAccessor Attribute
    {
      get { return attribute; }
      set { attribute = value; }
    }

    internal ElementAccessor[] Elements
    {
      get { return elements; }
      set { elements = value; sortedElements = null; }
    }

    internal TextAccessor Text
    {
      get { return text; }
      set { text = value; }
    }

    internal ChoiceIdentifierAccessor ChoiceIdentifier
    {
      get { return choiceIdentifier; }
      set { choiceIdentifier = value; }
    }

    internal XmlnsAccessor Xmlns
    {
      get { return xmlns; }
      set { xmlns = value; }
    }

    internal bool Ignore
    {
      get { return ignore; }
      set { ignore = value; }
    }

    internal Accessor Accessor
    {
      get
      {
        if (xmlns != null) return xmlns;
        if (attribute != null) return attribute;
        if (elements != null && elements.Length > 0) return elements[0];
        return text;
      }
    }

    static bool IsNeedNullableMember(ElementAccessor element)
    {
      if (element.Mapping is ArrayMapping)
      {
        ArrayMapping arrayMapping = (ArrayMapping)element.Mapping;
        if (arrayMapping.Elements != null && arrayMapping.Elements.Length == 1)
        {
          return IsNeedNullableMember(arrayMapping.Elements[0]);
        }
        return false;
      }
      else
      {
        return element.IsNullable && element.Mapping.TypeDesc.IsValueType;
      }
    }

    internal bool IsNeedNullable
    {
      get
      {
        if (xmlns != null) return false;
        if (attribute != null) return false;
        if (elements != null && elements.Length == 1)
        {
          return IsNeedNullableMember(elements[0]);
        }
        return false;
      }
    }

    internal static bool ElementsMatch(ElementAccessor[] a, ElementAccessor[] b)
    {
      if (a == null)
      {
        if (b == null)
          return true;
        return false;
      }
      if (b == null)
        return false;
      if (a.Length != b.Length)
        return false;
      for (int i = 0; i < a.Length; i++)
      {
        if (a[i].Name != b[i].Name || a[i].Namespace != b[i].Namespace || a[i].Form != b[i].Form || a[i].IsNullable != b[i].IsNullable)
          return false;
      }
      return true;
    }

    internal bool Match(AccessorMapping mapping)
    {
      if (Elements != null && Elements.Length > 0)
      {
        if (!ElementsMatch(Elements, mapping.Elements))
        {
          return false;
        }
        if (Text == null)
        {
          return (mapping.Text == null);
        }
      }
      if (Attribute != null)
      {
        if (mapping.Attribute == null)
          return false;
        return (Attribute.Name == mapping.Attribute.Name && Attribute.Namespace == mapping.Attribute.Namespace && Attribute.Form == mapping.Attribute.Form);
      }
      if (Text != null)
      {
        return (mapping.Text != null);
      }
      return (mapping.Accessor == null);
    }
  }
}