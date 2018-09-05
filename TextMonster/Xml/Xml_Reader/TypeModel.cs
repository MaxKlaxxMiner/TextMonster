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

    internal Type Type
    {
      get { return type; }
    }

    internal ModelScope ModelScope
    {
      get { return scope; }
    }

    internal TypeDesc TypeDesc
    {
      get { return typeDesc; }
    }
  }
}