namespace TextMonster.Xml.XmlReader
{
  //
  // XSL HTML output method properties
  //
  // Keep the first four bits in sync, so that the element and attribute mask operation can be combined.
  internal enum ElementProperties : uint { DEFAULT = 0, URI_PARENT = 1, BOOL_PARENT = 2, NAME_PARENT = 4, EMPTY = 8, NO_ENTITIES = 16, HEAD = 32, BLOCK_WS = 64, HAS_NS = 128 }
}
