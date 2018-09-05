using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// UPA violations will throw this exception
  /// </summary>
  class UpaException : Exception
  {
    object particle1;
    object particle2;
    public UpaException(object particle1, object particle2)
    {
      this.particle1 = particle1;
      this.particle2 = particle2;
    }
    public object Particle1 { get { return particle1; } }
    public object Particle2 { get { return particle2; } }
  }
}
