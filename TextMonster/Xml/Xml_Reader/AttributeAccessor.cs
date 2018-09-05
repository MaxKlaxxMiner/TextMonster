using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class AttributeAccessor : Accessor
  {
    bool isSpecial;
    bool isList;

    internal bool IsSpecialXmlNamespace
    {
      get { return isSpecial; }
    }

    internal bool IsList
    {
      get { return isList; }
      set { isList = value; }
    }

    internal void CheckSpecial()
    {
      int colon = Name.LastIndexOf(':');

      if (colon >= 0)
      {
        if (!Name.StartsWith("xml:", StringComparison.Ordinal))
        {
          throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidNameChars, Name));
        }
        Name = Name.Substring("xml:".Length);
        Namespace = XmlReservedNs.NsXml;
        isSpecial = true;
      }
      else
      {
        if (Namespace == XmlReservedNs.NsXml)
        {
          isSpecial = true;
        }
        else
        {
          isSpecial = false;
        }
      }
      if (isSpecial)
      {
        Form = XmlSchemaForm.Qualified;
      }
    }
  }
}