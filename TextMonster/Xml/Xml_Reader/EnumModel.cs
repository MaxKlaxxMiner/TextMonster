using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class EnumModel : TypeModel
  {
    ConstantModel[] constants;

    internal EnumModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }
  }
}