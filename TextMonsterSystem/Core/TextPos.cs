#region # using *.*

using TextMonsterSystem.Memory;

#endregion

namespace TextMonsterSystem.Core
{
  /// <summary>
  /// merkt sich die Speicherposition in einer TextMemory-Klasse
  /// </summary>
  public struct TextPos
  {
    /// <summary>
    /// merkt sich die ZeilenNummer (beginnend bei 1)
    /// </summary>
    internal long line;
    /// <summary>
    /// merkt sich die Spaltennummer (beginnend bei 1)
    /// </summary>
    internal long column;
  }
}
