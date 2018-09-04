namespace TextMonster.Xml.Xml_Reader
{
  // each node in the xpath tree
  internal class DoubleLinkAxis : Axis
  {
    internal Axis next;

    internal Axis Next
    {
      get { return this.next; }
      set { this.next = value; }
    }

    //constructor
    internal DoubleLinkAxis(Axis axis, DoubleLinkAxis inputaxis)
      : base(axis.TypeOfAxis, inputaxis, axis.Prefix, axis.Name, axis.NodeType)
    {
      this.next = null;
      this.Urn = axis.Urn;
      this.abbrAxis = axis.AbbrAxis;
      if (inputaxis != null)
      {
        inputaxis.Next = this;
      }
    }

    // recursive here
    internal static DoubleLinkAxis ConvertTree(Axis axis)
    {
      if (axis == null)
      {
        return null;
      }
      return (new DoubleLinkAxis(axis, ConvertTree((Axis)(axis.Input))));
    }
  }
}
