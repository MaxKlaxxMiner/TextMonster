namespace TextMonster.Xml.Xml_Reader
{
  internal class ArrayMapping : TypeMapping
  {
    ElementAccessor[] elements;
    ElementAccessor[] sortedElements;
    ArrayMapping next;
    StructMapping topLevelMapping;

    internal ElementAccessor[] Elements
    {
      get { return elements; }
      set { elements = value; sortedElements = null; }
    }


    internal ArrayMapping Next
    {
      get { return next; }
      set { next = value; }
    }

    internal StructMapping TopLevelMapping
    {
      get { return topLevelMapping; }
      set { topLevelMapping = value; }
    }
  }
}