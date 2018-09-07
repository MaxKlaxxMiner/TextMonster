using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class TypeModel
  {
    TypeDesc typeDesc;
    Type type;
    ModelScope scope;

    protected TypeModel(Type type, TypeDesc typeDesc, ModelScope scope)
    {
      this.scope = scope;
      this.type = type;
      this.typeDesc = typeDesc;
    }
  }
}