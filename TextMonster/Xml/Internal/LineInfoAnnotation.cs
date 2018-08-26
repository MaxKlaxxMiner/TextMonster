
namespace TextMonster.Xml
{
  internal class LineInfoAnnotation
  {
    internal readonly int lineNumber;
    internal readonly int linePosition;

    public LineInfoAnnotation(int lineNumber, int linePosition)
    {
      this.lineNumber = lineNumber;
      this.linePosition = linePosition;
    }
  }
}
