namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// The 0th node in each page contains a non-null reference to an XPathNodePageInfo internal class that provides
  /// information about that node's page.  The other fields in the 0th node are undefined and should never
  /// be used.
  /// </summary>
  sealed internal class XPathNodePageInfo
  {
    private int pageNum;
    private int nodeCount;
    private XPathNode[] pagePrev;
    private XPathNode[] pageNext;

    /// <summary>
    /// Constructor.
    /// </summary>
    public XPathNodePageInfo(XPathNode[] pagePrev, int pageNum)
    {
      this.pagePrev = pagePrev;
      this.pageNum = pageNum;
      this.nodeCount = 1;         // Every node page contains PageInfo at 0th position
    }

    /// <summary>
    /// Return the sequential page number of the page containing nodes that share this information atom.
    /// </summary>
    public int PageNumber
    {
      get { return this.pageNum; }
    }

    /// <summary>
    /// Return the number of nodes allocated in this page.
    /// </summary>
    public int NodeCount
    {
      get { return this.nodeCount; }
      set { this.nodeCount = value; }
    }

    /// <summary>
    /// Return the next node page in the document.
    /// </summary>
    public XPathNode[] NextPage
    {
      get { return this.pageNext; }
      set { this.pageNext = value; }
    }
  }
}
