namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Ref class is used to verify string atomization in debug mode.
  /// </summary>
  internal static class Ref
  {
    public static bool Equal(string strA, string strB)
    {
      return (object)strA == (object)strB;
    }
  }
}
