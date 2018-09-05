using System;

namespace TextMonster.Xml.Xml_Reader
{
  public class XmlReflectionMember
  {
    string memberName;
    Type type;
    XmlAttributes xmlAttributes = new XmlAttributes();
    bool isReturnValue;
    bool overrideIsNullable;

    public Type MemberType
    {
      get { return type; }
    }

    public XmlAttributes XmlAttributes
    {
      get { return xmlAttributes; }
    }

    public string MemberName
    {
      get { return memberName == null ? string.Empty : memberName; }
    }

    public bool IsReturnValue
    {
      get { return isReturnValue; }
    }

    public bool OverrideIsNullable
    {
      get { return overrideIsNullable; }
    }
  }
}