using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class MemberMappingComparer : IComparer
  {
    public int Compare(object o1, object o2)
    {
      MemberMapping m1 = (MemberMapping)o1;
      MemberMapping m2 = (MemberMapping)o2;

      bool m1Text = m1.IsText;
      if (m1Text)
      {
        if (m2.IsText)
          return 0;
        return 1;
      }
      else if (m2.IsText)
        return -1;

      if (m1.SequenceId < 0 && m2.SequenceId < 0)
        return 0;
      if (m1.SequenceId < 0)
        return 1;
      if (m2.SequenceId < 0)
        return -1;
      if (m1.SequenceId < m2.SequenceId)
        return -1;
      if (m1.SequenceId > m2.SequenceId)
        return 1;
      return 0;
    }
  }
}