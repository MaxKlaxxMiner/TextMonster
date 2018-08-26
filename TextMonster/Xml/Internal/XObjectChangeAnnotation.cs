using System;

namespace TextMonster.Xml
{
  internal class XObjectChangeAnnotation
  {
    internal EventHandler<X_ObjectChangeEventArgs> changing;
    internal EventHandler<X_ObjectChangeEventArgs> changed;
  }
}
