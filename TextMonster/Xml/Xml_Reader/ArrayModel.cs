using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class ArrayModel : TypeModel
  {
    internal ArrayModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }

    internal TypeModel Element
    {
      get { return ModelScope.GetTypeModel(TypeScope.GetArrayElementType(Type, null)); }
    }
  }
}