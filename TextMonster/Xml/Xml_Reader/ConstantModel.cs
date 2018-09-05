using System.Reflection;

namespace TextMonster.Xml.Xml_Reader
{
  internal class ConstantModel
  {
    FieldInfo fieldInfo;
    long value;

    internal ConstantModel(FieldInfo fieldInfo, long value)
    {
      this.fieldInfo = fieldInfo;
      this.value = value;
    }

    internal string Name
    {
      get { return fieldInfo.Name; }
    }

    internal long Value
    {
      get { return value; }
    }

    internal FieldInfo FieldInfo
    {
      get { return fieldInfo; }
    }
  }
}