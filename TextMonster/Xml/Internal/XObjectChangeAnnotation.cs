using System;

namespace TextMonster.Xml
{
  internal class XObjectChangeAnnotation
  {
    internal EventHandler<XObjectChangeEventArgs> changing;
    internal EventHandler<XObjectChangeEventArgs> changed;
  }
}
