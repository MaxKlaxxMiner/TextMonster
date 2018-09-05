namespace TextMonster.Xml.Xml_Reader
{
  internal class EnumMapping : PrimitiveMapping
  {
    ConstantMapping[] constants;
    bool isFlags;

    internal bool IsFlags
    {
      get { return isFlags; }
      set { isFlags = value; }
    }

    internal ConstantMapping[] Constants
    {
      get { return constants; }
      set { constants = value; }
    }
  }
}