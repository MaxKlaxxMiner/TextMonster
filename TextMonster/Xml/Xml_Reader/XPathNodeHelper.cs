namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Library of XPathNode helper routines.
  /// </summary>
  internal abstract class XPathNodeHelper
  {
    /// <summary>
    /// Return a location integer that can be easily compared with other locations from the same document
    /// in order to determine the relative document order of two nodes.
    /// </summary>
    public static int GetLocation(XPathNode[] pageNode, int idxNode)
    {
      return (pageNode[0].PageInfo.PageNumber << 16) | idxNode;
    }
  }
}
