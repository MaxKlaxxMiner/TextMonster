using System;

namespace TextMonster.Xml.Xml_Reader
{
  [Flags]
  public enum NamespaceHandling
  {

    //
    // Default behavior
    //
    Default = 0x0,

    //
    // Duplicate namespace declarations will be removed
    //
    OmitDuplicates = 0x1,
  }
}
