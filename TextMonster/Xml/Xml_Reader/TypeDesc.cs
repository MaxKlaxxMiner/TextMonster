using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class TypeDesc
  {
    string name;
    string fullName;
    string cSharpName;
    TypeDesc arrayElementTypeDesc;
    TypeDesc arrayTypeDesc;
    TypeDesc nullableTypeDesc;
    TypeKind kind;
    XmlSchemaType dataType;
    Type type;
    TypeDesc baseTypeDesc;
    TypeFlags flags;
    string formatterName;
    bool isXsdType;
    bool isMixed;
    MappedTypeDesc extendedType;
    int weight;
    Exception exception;

    internal TypeDesc(string name, string fullName, XmlSchemaType dataType, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags, string formatterName)
    {
      this.name = name.Replace('+', '.');
      this.fullName = fullName.Replace('+', '.');
      this.kind = kind;
      this.baseTypeDesc = baseTypeDesc;
      this.flags = flags;
      this.isXsdType = kind == TypeKind.Primitive;
      if (this.isXsdType)
        this.weight = 1;
      else if (kind == TypeKind.Enum)
        this.weight = 2;
      else if (this.kind == TypeKind.Root)
        this.weight = -1;
      else
        this.weight = baseTypeDesc == null ? 0 : baseTypeDesc.Weight + 1;
      this.dataType = dataType;
      this.formatterName = formatterName;
    }

    internal TypeDesc(string name, string fullName, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags)
      : this(name, fullName, (XmlSchemaType)null, kind, baseTypeDesc, flags, null) { }

    internal TypeDesc(Type type, bool isXsdType, XmlSchemaType dataType, string formatterName, TypeFlags flags)
      : this(type.Name, type.FullName, dataType, TypeKind.Primitive, (TypeDesc)null, flags, formatterName)
    {
      this.isXsdType = isXsdType;
      this.type = type;
    }
    internal TypeDesc(Type type, string name, string fullName, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags, TypeDesc arrayElementTypeDesc)
      : this(name, fullName, null, kind, baseTypeDesc, flags, null)
    {

      this.arrayElementTypeDesc = arrayElementTypeDesc;
      this.type = type;
    }

    public override string ToString()
    {
      return fullName;
    }

    internal TypeFlags Flags
    {
      get { return flags; }
    }

    internal bool IsXsdType
    {
      get { return isXsdType; }
    }

    internal bool IsMappedType
    {
      get { return extendedType != null; }
    }

    internal string Name
    {
      get { return name; }
    }

    internal string FullName
    {
      get { return fullName; }
    }

    internal XmlSchemaType DataType
    {
      get { return dataType; }
    }

    internal Type Type
    {
      get { return type; }
    }

    internal string FormatterName
    {
      get { return formatterName; }
    }

    internal TypeKind Kind
    {
      get { return kind; }
    }

    internal bool IsValueType
    {
      get { return (flags & TypeFlags.Reference) == 0; }
    }

    internal bool CanBeAttributeValue
    {
      get { return (flags & TypeFlags.CanBeAttributeValue) != 0; }
    }

    internal bool CanBeElementValue
    {
      get { return (flags & TypeFlags.CanBeElementValue) != 0; }
    }

    internal bool CanBeTextValue
    {
      get { return (flags & TypeFlags.CanBeTextValue) != 0; }
    }

    internal bool IsMixed
    {
      get { return isMixed || CanBeTextValue; }
      set { isMixed = value; }
    }

    internal bool IsSpecial
    {
      get { return (flags & TypeFlags.Special) != 0; }
    }

    internal bool IsAmbiguousDataType
    {
      get { return (flags & TypeFlags.AmbiguousDataType) != 0; }
    }

    internal bool HasCustomFormatter
    {
      get { return (flags & TypeFlags.HasCustomFormatter) != 0; }
    }

    internal bool HasIsEmpty
    {
      get { return (flags & TypeFlags.HasIsEmpty) != 0; }
    }

    internal bool HasDefaultConstructor
    {
      get { return (flags & TypeFlags.HasDefaultConstructor) != 0; }
    }

    internal bool IsUnsupported
    {
      get { return (flags & TypeFlags.Unsupported) != 0; }
    }

    internal bool IsAbstract
    {
      get { return (flags & TypeFlags.Abstract) != 0; }
    }

    internal bool IsOptionalValue
    {
      get { return (flags & TypeFlags.OptionalValue) != 0; }
    }

    internal bool IsVoid
    {
      get { return kind == TypeKind.Void; }
    }

    internal bool IsClass
    {
      get { return kind == TypeKind.Class; }
    }

    internal bool IsStructLike
    {
      get { return kind == TypeKind.Struct || kind == TypeKind.Class; }
    }

    internal bool IsArrayLike
    {
      get { return kind == TypeKind.Array || kind == TypeKind.Collection || kind == TypeKind.Enumerable; }
    }

    internal bool IsCollection
    {
      get { return kind == TypeKind.Collection; }
    }

    internal bool IsEnumerable
    {
      get { return kind == TypeKind.Enumerable; }
    }

    internal bool IsPrimitive
    {
      get { return kind == TypeKind.Primitive; }
    }

    internal bool IsEnum
    {
      get { return kind == TypeKind.Enum; }
    }

    internal bool IsNullable
    {
      get { return !IsValueType; }
    }

    internal bool IsRoot
    {
      get { return kind == TypeKind.Root; }
    }

    internal Exception Exception
    {
      get { return exception; }
      set { exception = value; }
    }

    internal TypeDesc GetNullableTypeDesc(Type type)
    {
      if (IsOptionalValue)
        return this;

      if (nullableTypeDesc == null)
      {
        nullableTypeDesc = new TypeDesc("NullableOf" + this.name, "System.Nullable`1[" + this.fullName + "]", null, TypeKind.Struct, this, this.flags | TypeFlags.OptionalValue, this.formatterName);
        nullableTypeDesc.type = type;
      }

      return nullableTypeDesc;
    }
    internal void CheckSupported()
    {
      if (IsUnsupported)
      {
        if (Exception != null)
        {
          throw Exception;
        }
        else
        {
          throw new NotSupportedException(Res.GetString(Res.XmlSerializerUnsupportedType, FullName));
        }
      }
      if (baseTypeDesc != null)
        baseTypeDesc.CheckSupported();
      if (arrayElementTypeDesc != null)
        arrayElementTypeDesc.CheckSupported();
    }

    internal void CheckNeedConstructor()
    {
      if (!IsValueType && !IsAbstract && !HasDefaultConstructor)
      {
        flags |= TypeFlags.Unsupported;
        this.exception = new InvalidOperationException(Res.GetString(Res.XmlConstructorInaccessible, FullName));
      }
    }

    internal TypeDesc ArrayElementTypeDesc
    {
      get { return arrayElementTypeDesc; }
      set { arrayElementTypeDesc = value; }
    }

    internal int Weight
    {
      get { return weight; }
    }

    internal TypeDesc CreateArrayTypeDesc()
    {
      if (arrayTypeDesc == null)
        arrayTypeDesc = new TypeDesc(null, name + "[]", fullName + "[]", TypeKind.Array, null, TypeFlags.Reference | (flags & TypeFlags.UseReflection), this);
      return arrayTypeDesc;
    }

    internal TypeDesc CreateMappedTypeDesc(MappedTypeDesc extension)
    {
      TypeDesc newTypeDesc = new TypeDesc(extension.Name, extension.Name, null, this.kind, this.baseTypeDesc, this.flags, null);
      newTypeDesc.isXsdType = this.isXsdType;
      newTypeDesc.isMixed = this.isMixed;
      newTypeDesc.extendedType = extension;
      newTypeDesc.dataType = this.dataType;
      return newTypeDesc;
    }

    internal TypeDesc BaseTypeDesc
    {
      get { return baseTypeDesc; }
      set
      {
        baseTypeDesc = value;
        weight = baseTypeDesc == null ? 0 : baseTypeDesc.Weight + 1;
      }
    }

    internal bool IsDerivedFrom(TypeDesc baseTypeDesc)
    {
      TypeDesc typeDesc = this;
      while (typeDesc != null)
      {
        if (typeDesc == baseTypeDesc) return true;
        typeDesc = typeDesc.BaseTypeDesc;
      }
      return baseTypeDesc.IsRoot;
    }

    internal static TypeDesc FindCommonBaseTypeDesc(TypeDesc[] typeDescs)
    {
      if (typeDescs.Length == 0) return null;
      TypeDesc leastDerivedTypeDesc = null;
      int leastDerivedLevel = int.MaxValue;

      for (int i = 0; i < typeDescs.Length; i++)
      {
        int derivationLevel = typeDescs[i].Weight;
        if (derivationLevel < leastDerivedLevel)
        {
          leastDerivedLevel = derivationLevel;
          leastDerivedTypeDesc = typeDescs[i];
        }
      }
      while (leastDerivedTypeDesc != null)
      {
        int i;
        for (i = 0; i < typeDescs.Length; i++)
        {
          if (!typeDescs[i].IsDerivedFrom(leastDerivedTypeDesc)) break;
        }
        if (i == typeDescs.Length) break;
        leastDerivedTypeDesc = leastDerivedTypeDesc.BaseTypeDesc;
      }
      return leastDerivedTypeDesc;
    }
  }
}