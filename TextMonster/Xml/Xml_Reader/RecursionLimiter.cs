namespace TextMonster.Xml.Xml_Reader
{
  internal class RecursionLimiter
  {
    int maxDepth;
    int depth;
    WorkItems deferredWorkItems;

    internal RecursionLimiter()
    {
      this.depth = 0;
      this.maxDepth = int.MaxValue;
    }

    internal bool IsExceededLimit { get { return this.depth > this.maxDepth; } }
    internal int Depth { get { return this.depth; } set { this.depth = value; } }

    internal WorkItems DeferredWorkItems
    {
      get
      {
        if (deferredWorkItems == null)
        {
          deferredWorkItems = new WorkItems();
        }
        return deferredWorkItems;
      }
    }

  }
}