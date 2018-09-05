namespace TextMonster.Xml.Xml_Reader
{
  internal class ElementAccessor : Accessor
  {
    bool nullable;
    bool isSoap;
    bool unbounded = false;

    internal bool IsSoap
    {
      get { return isSoap; }
      set { isSoap = value; }
    }

    internal bool IsNullable
    {
      get { return nullable; }
      set { nullable = value; }
    }

    internal bool IsUnbounded
    {
      get { return unbounded; }
      set { unbounded = value; }
    }

    internal ElementAccessor Clone()
    {
      ElementAccessor newAccessor = new ElementAccessor();
      newAccessor.nullable = this.nullable;
      newAccessor.IsTopLevelInSchema = this.IsTopLevelInSchema;
      newAccessor.Form = this.Form;
      newAccessor.isSoap = this.isSoap;
      newAccessor.Name = this.Name;
      newAccessor.Default = this.Default;
      newAccessor.Namespace = this.Namespace;
      newAccessor.Mapping = this.Mapping;
      newAccessor.Any = this.Any;

      return newAccessor;
    }
  }
}