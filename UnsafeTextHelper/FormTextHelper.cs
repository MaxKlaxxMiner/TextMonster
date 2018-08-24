using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TextMonster;

namespace UnsafeTextHelper
{
  public sealed partial class FormTextHelper : Form
  {
    public FormTextHelper()
    {
      InitializeComponent();
    }

    static string VisibleChar(byte c)
    {
      switch ((char)c)
      {
        case '\r': return "'\\r'";
        case '\n': return "'\\n'";
        case '\t': return "'\\t'";
        case '\'': return "'\\''";
        default: return c >= ' ' && c < 128 ? "'" + (char)c + "'" : null;
      }
    }

    static string BcharUnfixed(byte c, string extra = "")
    {
      switch (extra)
      {
        case "":
        {
          if (VisibleChar(c) != null) return VisibleChar(c);
          return "0x" + c.ToString("x").PadLeft(2, '0');
        }
        case "U":
        {
          if (VisibleChar(c) != null) return "(uint)" + VisibleChar(c);
          return "0x" + c.ToString("x").PadLeft(2, '0') + "U";
        }
        case "L":
        {
          if (VisibleChar(c) != null) return "(long)" + VisibleChar(c);
          return "0x" + c.ToString("x").PadLeft(2, '0') + "L";
        }
        case "UL":
        {
          if (VisibleChar(c) != null) return "(ulong)" + VisibleChar(c);
          return "0x" + c.ToString("x").PadLeft(2, '0') + "UL";
        }
        default: throw new Exception("au: " + extra);
      }
    }

    static string Bchar(byte c, string extra = "")
    {
      return BcharUnfixed(c, extra).Replace("'\\'", "'\\\\'").Replace("'''", "'\\''");
    }

    static string CompareMask(byte[] buf, int p, int len, char skipChar)
    {
      p -= len;
      ulong skipMask = 0;

      for (int i = len - 1; i >= 0; i--)
      {
        skipMask <<= 8;
        if ((char)buf[p + i] == skipChar)
        {
          skipMask |= 0xff;
        }
      }

      if (skipMask != 0)
      {
        skipMask = ~skipMask;
        if (len != 8) skipMask &= ((1UL << (len * 8)) - 1);
        switch (len)
        {
          case 1:
          case 2:
          case 3: return " & 0x" + skipMask.ToString("x") + ")";
          case 4: return " & 0x" + skipMask.ToString("x") + "U)";
          case 5:
          case 6:
          case 7: return " & 0x" + skipMask.ToString("x") + "L)";
          case 8: return " & 0x" + skipMask.ToString("x") + "UL)";
          default: return "";
        }
      }
      switch (len)
      {
        case 3: return " & (1 << 24) - 1)";
        case 5: return " & (1L << 40) - 1)";
        case 6: return " & (1L << 48) - 1)";
        case 7: return " & (1L << 56) - 1)";
        default: return "";
      }
    }

    static string BCombi(byte[] buf, int p, int len, char skipChar)
    {
      p -= len;
      var elements = new List<string>();
      for (int i = 0; i < len; i++)
      {
        if ((char)buf[p + i] == skipChar) continue;

        switch (i)
        {
          case 0: elements.Add(Bchar(buf[p + i])); break;
          case 1: elements.Add("(" + Bchar(buf[p + i]) + " << 8)"); break;
          case 2: elements.Add("(" + Bchar(buf[p + i]) + " << 16)"); break;
          case 3: elements.Add("(" + Bchar(buf[p + i], "U") + " << 24)"); break;
          case 4: elements.Add("(" + Bchar(buf[p + i], "L") + " << 32)"); break;
          case 5: elements.Add("(" + Bchar(buf[p + i], "L") + " << 40)"); break;
          case 6: elements.Add("(" + Bchar(buf[p + i], "L") + " << 48)"); break;
          case 7: elements.Add("(" + Bchar(buf[p + i], "UL") + " << 56)"); break;
        }
      }

      return string.Join(" + ", elements);
    }

    static string CompareZeile(byte[] buf, ref int p, int len, char skipChar)
    {
      while (len > 0 && (char)buf[p] == skipChar)
      {
        p++;
        len--;
      }

      switch (len)
      {
        case 0: return "";
        case 1: p++; return "b[p" + (p != 1 ? " + " + (p - 1) : "") + "] != " + BCombi(buf, p, 1, skipChar);
        case 2: p += 2; return (CompareMask(buf, p, 2, skipChar) != "" ? "(" : "") + "*(ushort*)(b + p" + (p != 2 ? " + " + (p - 2) : "") + ")" + CompareMask(buf, p, 2, skipChar) + " != " + BCombi(buf, p, 2, skipChar);
        case 3: p += 3; return (CompareMask(buf, p, 3, skipChar) != "" ? "(" : "") + "*(int*)(b + p" + (p != 3 ? " + " + (p - 3) : "") + ")" + CompareMask(buf, p, 3, skipChar) + " != " + BCombi(buf, p, 3, skipChar);
        case 4: p += 4; return (CompareMask(buf, p, 4, skipChar) != "" ? "(" : "") + "*(uint*)(b + p" + (p != 4 ? " + " + (p - 4) : "") + ")" + CompareMask(buf, p, 4, skipChar) + " != " + BCombi(buf, p, 4, skipChar);
        case 5: p += 5; return (CompareMask(buf, p, 5, skipChar) != "" ? "(" : "") + "*(long*)(b + p" + (p != 5 ? " + " + (p - 5) : "") + ")" + CompareMask(buf, p, 5, skipChar) + " != " + BCombi(buf, p, 5, skipChar);
        case 6: p += 6; return (CompareMask(buf, p, 6, skipChar) != "" ? "(" : "") + "*(long*)(b + p" + (p != 6 ? " + " + (p - 6) : "") + ")" + CompareMask(buf, p, 6, skipChar) + " != " + BCombi(buf, p, 6, skipChar);
        case 7: p += 7; return (CompareMask(buf, p, 7, skipChar) != "" ? "(" : "") + "*(long*)(b + p" + (p != 7 ? " + " + (p - 7) : "") + ")" + CompareMask(buf, p, 7, skipChar) + " != " + BCombi(buf, p, 7, skipChar);
        default: p += 8; return (CompareMask(buf, p, 8, skipChar) != "" ? "(" : "") + "*(ulong*)(b + p" + (p != 8 ? " + " + (p - 8) : "") + ")" + CompareMask(buf, p, 8, skipChar) + " != " + BCombi(buf, p, 8, skipChar);
      }
    }

    static string EncodeUnsafe(string txt)
    {
      var a = new StringBuilder();

      var data = Encoding.UTF8.GetBytes(txt);
      int p = 0;

      while (p < data.Length)
      {
        string zeile = CompareZeile(data, ref p, data.Length - p, '#');
        if (a.Length == 0)
        {
          a.Append("      if (" + zeile);
        }
        else
        {
          a.AppendLine(" ||");
          a.Append("         " + (zeile.Length > 0 && zeile[0] == '(' ? "" : " ") + zeile);
        }
      }
      a.AppendLine(") break;");
      a.AppendLine("      p += " + data.Length + "; // Skip: " + txt + "");

      int firstSkip = UnsafeHelper.Latin1.GetString(data).IndexOf('#');
      if (firstSkip >= 0)
      {
        a.AppendLine("      // Jump: b[p - " + (data.Length - firstSkip) + "] // Next: " + Encoding.UTF8.GetString(data.Skip(firstSkip).ToArray()));
      }

      return a.ToString();
    }

    private void inputTextBox_TextChanged(object sender, EventArgs e)
    {
      outputTextBox.Text = EncodeUnsafe(inputTextBox.Text.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t"));
    }

    private void textBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Control && e.KeyCode == Keys.A) ((TextBox)sender).SelectAll();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      inputTextBox_TextChanged(null, null);
    }
  }
}
