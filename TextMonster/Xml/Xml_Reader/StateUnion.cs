using System.Runtime.InteropServices;

namespace TextMonster.Xml.Xml_Reader
{
  [StructLayout(LayoutKind.Explicit)]
  internal struct StateUnion
  {
    [FieldOffset(0)]
    public int State;  //DFA 
    [FieldOffset(0)]
    public int AllElementsRequired; //AllContentValidator
    [FieldOffset(0)]
    public int CurPosIndex; //NFAContentValidator
    [FieldOffset(0)]
    public int NumberOfRunningPos; //RangeContentValidator
  }
}
