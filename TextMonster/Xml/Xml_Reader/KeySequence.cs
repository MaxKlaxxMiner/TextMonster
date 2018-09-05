using System;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal class KeySequence
  {
    TypedObject[] ks;
    int dim;
    int hashcode = -1;
    int posline, poscol;            // for error reporting

    internal KeySequence(int dim, int line, int col)
    {
      this.dim = dim;
      this.ks = new TypedObject[dim];
      this.posline = line;
      this.poscol = col;
    }

    public int PosLine
    {
      get { return this.posline; }
    }

    public int PosCol
    {
      get { return this.poscol; }
    }

    public object this[int index]
    {
      get
      {
        object result = ks[index];
        return result;
      }
      set
      {
        ks[index] = (TypedObject)value;
      }
    }

    // return true if no null field
    internal bool IsQualified()
    {
      for (int i = 0; i < this.ks.Length; ++i)
      {
        if ((this.ks[i] == null) || (this.ks[i].Value == null)) return false;
      }
      return true;
    }

    // it's not directly suit for hashtable, because it's always calculating address
    public override int GetHashCode()
    {
      if (hashcode != -1)
      {
        return hashcode;
      }
      hashcode = 0;  // indicate it's changed. even the calculated hashcode below is 0
      for (int i = 0; i < this.ks.Length; i++)
      {
        // extract its primitive value to calculate hashcode
        // decimal is handled differently to enable among different CLR types
        this.ks[i].SetDecimal();
        if (this.ks[i].IsDecimal)
        {
          for (int j = 0; j < this.ks[i].Dim; j++)
          {
            hashcode += this.ks[i].Dvalue[j].GetHashCode();
          }
        }
        // 
        else
        {
          Array arr = this.ks[i].Value as System.Array;
          if (arr != null)
          {
            XmlAtomicValue[] atomicValues = arr as XmlAtomicValue[];
            if (atomicValues != null)
            {
              for (int j = 0; j < atomicValues.Length; j++)
              {
                hashcode += ((XmlAtomicValue)atomicValues.GetValue(j)).TypedValue.GetHashCode();
              }
            }
            else
            {
              for (int j = 0; j < ((Array)this.ks[i].Value).Length; j++)
              {
                hashcode += ((Array)this.ks[i].Value).GetValue(j).GetHashCode();
              }
            }
          }
          else
          { //not a list
            hashcode += this.ks[i].Value.GetHashCode();
          }
        }
      }
      return hashcode;
    }

    // considering about derived type
    public override bool Equals(object other)
    {
      // each key sequence member can have different type
      KeySequence keySequence = (KeySequence)other;
      for (int i = 0; i < this.ks.Length; i++)
      {
        if (!this.ks[i].Equals(keySequence.ks[i]))
        {
          return false;
        }
      }
      return true;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.ks[0].ToString());
      for (int i = 1; i < this.ks.Length; i++)
      {
        sb.Append(" ");
        sb.Append(this.ks[i].ToString());
      }
      return sb.ToString();
    }
  }
}
