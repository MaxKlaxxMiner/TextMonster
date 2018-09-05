using System;
using System.Globalization;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class XPathScanner
  {
    private string xpathExpr;
    private int xpathExprIndex;
    private LexKind kind;
    private char currentChar;
    private string name;
    private string prefix;
    private string stringValue;
    private double numberValue = double.NaN;
    private bool canBeFunction;
    private XmlCharType xmlCharType = XmlCharType.Instance;

    public XPathScanner(string xpathExpr)
    {
      if (xpathExpr == null)
      {
        throw XPathException.Create(Res.Xp_ExprExpected, string.Empty);
      }
      this.xpathExpr = xpathExpr;
      NextChar();
      NextLex();
    }

    public string SourceText { get { return this.xpathExpr; } }

    private char CurerntChar { get { return currentChar; } }

    private bool NextChar()
    {
      if (xpathExprIndex < xpathExpr.Length)
      {
        currentChar = xpathExpr[xpathExprIndex++];
        return true;
      }
      else
      {
        currentChar = '\0';
        return false;
      }
    }

    public LexKind Kind { get { return this.kind; } }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string Prefix
    {
      get
      {
        return this.prefix;
      }
    }

    public string StringValue
    {
      get
      {
        return this.stringValue;
      }
    }

    public double NumberValue
    {
      get
      {
        return this.numberValue;
      }
    }

    // To parse PathExpr we need a way to distinct name from function. 
    // THis distinction can't be done without context: "or (1 != 0)" this this a function or 'or' in OrExp 
    public bool CanBeFunction
    {
      get
      {
        return this.canBeFunction;
      }
    }

    void SkipSpace()
    {
      while (xmlCharType.IsWhiteSpace(this.CurerntChar) && NextChar()) ;
    }

    public bool NextLex()
    {
      SkipSpace();
      switch (this.CurerntChar)
      {
        case '\0':
        kind = LexKind.Eof;
        return false;
        case ',':
        case '@':
        case '(':
        case ')':
        case '|':
        case '*':
        case '[':
        case ']':
        case '+':
        case '-':
        case '=':
        case '#':
        case '$':
        kind = (LexKind)Convert.ToInt32(this.CurerntChar, CultureInfo.InvariantCulture);
        NextChar();
        break;
        case '<':
        kind = LexKind.Lt;
        NextChar();
        if (this.CurerntChar == '=')
        {
          kind = LexKind.Le;
          NextChar();
        }
        break;
        case '>':
        kind = LexKind.Gt;
        NextChar();
        if (this.CurerntChar == '=')
        {
          kind = LexKind.Ge;
          NextChar();
        }
        break;
        case '!':
        kind = LexKind.Bang;
        NextChar();
        if (this.CurerntChar == '=')
        {
          kind = LexKind.Ne;
          NextChar();
        }
        break;
        case '.':
        kind = LexKind.Dot;
        NextChar();
        if (this.CurerntChar == '.')
        {
          kind = LexKind.DotDot;
          NextChar();
        }
        else if (XmlCharType.IsDigit(this.CurerntChar))
        {
          kind = LexKind.Number;
          numberValue = ScanFraction();
        }
        break;
        case '/':
        kind = LexKind.Slash;
        NextChar();
        if (this.CurerntChar == '/')
        {
          kind = LexKind.SlashSlash;
          NextChar();
        }
        break;
        case '"':
        case '\'':
        this.kind = LexKind.String;
        this.stringValue = ScanString();
        break;
        default:
        if (XmlCharType.IsDigit(this.CurerntChar))
        {
          kind = LexKind.Number;
          numberValue = ScanNumber();
        }
        else if (xmlCharType.IsStartNCNameSingleChar(this.CurerntChar))
        {
          kind = LexKind.Name;
          this.name = ScanName();
          this.prefix = string.Empty;
          // "foo:bar" is one lexem not three because it doesn't allow spaces in between
          // We should distinct it from "foo::" and need process "foo ::" as well
          if (this.CurerntChar == ':')
          {
            NextChar();
            // can be "foo:bar" or "foo::"
            if (this.CurerntChar == ':')
            {   // "foo::"
              NextChar();
              kind = LexKind.Axe;
            }
            else
            {                          // "foo:*", "foo:bar" or "foo: "
              this.prefix = this.name;
              if (this.CurerntChar == '*')
              {
                NextChar();
                this.name = "*";
              }
              else if (xmlCharType.IsStartNCNameSingleChar(this.CurerntChar))
              {
                this.name = ScanName();
              }
              else
              {
                throw XPathException.Create(Res.Xp_InvalidName, SourceText);
              }
            }

          }
          else
          {
            SkipSpace();
            if (this.CurerntChar == ':')
            {
              NextChar();
              // it can be "foo ::" or just "foo :"
              if (this.CurerntChar == ':')
              {
                NextChar();
                kind = LexKind.Axe;
              }
              else
              {
                throw XPathException.Create(Res.Xp_InvalidName, SourceText);
              }
            }
          }
          SkipSpace();
          this.canBeFunction = (this.CurerntChar == '(');
        }
        else
        {
          throw XPathException.Create(Res.Xp_InvalidToken, SourceText);
        }
        break;
      }
      return true;
    }

    private double ScanNumber()
    {
      int start = xpathExprIndex - 1;
      int len = 0;
      while (XmlCharType.IsDigit(this.CurerntChar))
      {
        NextChar(); len++;
      }
      if (this.CurerntChar == '.')
      {
        NextChar(); len++;
        while (XmlCharType.IsDigit(this.CurerntChar))
        {
          NextChar(); len++;
        }
      }
      return XmlConvert.ToXPathDouble(this.xpathExpr.Substring(start, len));
    }

    private double ScanFraction()
    {
      int start = xpathExprIndex - 2;
      int len = 1; // '.'
      while (XmlCharType.IsDigit(this.CurerntChar))
      {
        NextChar(); len++;
      }
      return XmlConvert.ToXPathDouble(this.xpathExpr.Substring(start, len));
    }

    private string ScanString()
    {
      char endChar = this.CurerntChar;
      NextChar();
      int start = xpathExprIndex - 1;
      int len = 0;
      while (this.CurerntChar != endChar)
      {
        if (!NextChar())
        {
          throw XPathException.Create(Res.Xp_UnclosedString);
        }
        len++;
      }
      NextChar();
      return this.xpathExpr.Substring(start, len);
    }

    private string ScanName()
    {
      int start = xpathExprIndex - 1;
      int len = 0;

      for (; ; )
      {
        if (xmlCharType.IsNCNameSingleChar(this.CurerntChar))
        {
          NextChar();
          len++;
        }
        else
        {
          break;
        }
      }
      return this.xpathExpr.Substring(start, len);
    }

    public enum LexKind
    {
      Comma = ',',
      Slash = '/',
      At = '@',
      Dot = '.',
      LParens = '(',
      RParens = ')',
      LBracket = '[',
      RBracket = ']',
      Star = '*',
      Plus = '+',
      Minus = '-',
      Eq = '=',
      Lt = '<',
      Gt = '>',
      Bang = '!',
      Dollar = '$',
      Apos = '\'',
      Quote = '"',
      Union = '|',
      Ne = 'N',   // !=
      Le = 'L',   // <=
      Ge = 'G',   // >=
      And = 'A',   // &&
      Or = 'O',   // ||
      DotDot = 'D',   // ..
      SlashSlash = 'S',   // //
      Name = 'n',   // XML _Name
      String = 's',   // Quoted string constant
      Number = 'd',   // _Number constant
      Axe = 'a',   // Axe (like child::)
      Eof = 'E',
    };
  }
}
