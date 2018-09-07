using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using Microsoft.CSharp;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\CodeIdentifier.uex' path='docs/doc[@for="CodeIdentifier"]/*' />
  ///<internalonly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class CodeIdentifier
  {
    internal static CodeDomProvider csharp = new CSharpCodeProvider();
    internal const int MaxIdentifierLength = 511;

    [Obsolete("This class should never get constructed as it contains only static methods.")]
    public CodeIdentifier()
    {
    }

    /// <include file='doc\CodeIdentifier.uex' path='docs/doc[@for="CodeIdentifier.MakePascal"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string MakePascal(string identifier)
    {
      identifier = MakeValid(identifier);
      if (identifier.Length <= 2)
        return identifier.ToUpper(CultureInfo.InvariantCulture);
      if (char.IsLower(identifier[0]))
        return char.ToUpper(identifier[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + identifier.Substring(1);
      return identifier;
    }

    /// <include file='doc\CodeIdentifier.uex' path='docs/doc[@for="CodeIdentifier.MakeCamel"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string MakeCamel(string identifier)
    {
      identifier = MakeValid(identifier);
      if (identifier.Length <= 2)
        return identifier.ToLower(CultureInfo.InvariantCulture);
      if (char.IsUpper(identifier[0]))
        return char.ToLower(identifier[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + identifier.Substring(1);
      return identifier;
    }

    /// <include file='doc\CodeIdentifier.uex' path='docs/doc[@for="CodeIdentifier.MakeValid"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string MakeValid(string identifier)
    {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < identifier.Length && builder.Length < MaxIdentifierLength; i++)
      {
        char c = identifier[i];
        if (IsValid(c))
        {
          if (builder.Length == 0 && !IsValidStart(c))
          {
            builder.Append("Item");
          }
          builder.Append(c);
        }
      }
      if (builder.Length == 0) return "Item";
      return builder.ToString();
    }

    static bool IsValidStart(char c)
    {
      // First char cannot be a number
      if (Char.GetUnicodeCategory(c) == UnicodeCategory.DecimalDigitNumber)
        return false;
      return true;
    }

    static bool IsValid(char c)
    {
      UnicodeCategory uc = Char.GetUnicodeCategory(c);
      // each char must be Lu, Ll, Lt, Lm, Lo, Nd, Mn, Mc, Pc
      // 
      switch (uc)
      {
        case UnicodeCategory.UppercaseLetter:        // Lu
        case UnicodeCategory.LowercaseLetter:        // Ll
        case UnicodeCategory.TitlecaseLetter:        // Lt
        case UnicodeCategory.ModifierLetter:         // Lm
        case UnicodeCategory.OtherLetter:            // Lo
        case UnicodeCategory.DecimalDigitNumber:     // Nd
        case UnicodeCategory.NonSpacingMark:         // Mn
        case UnicodeCategory.SpacingCombiningMark:   // Mc
        case UnicodeCategory.ConnectorPunctuation:   // Pc
          break;
        case UnicodeCategory.LetterNumber:
        case UnicodeCategory.OtherNumber:
        case UnicodeCategory.EnclosingMark:
        case UnicodeCategory.SpaceSeparator:
        case UnicodeCategory.LineSeparator:
        case UnicodeCategory.ParagraphSeparator:
        case UnicodeCategory.Control:
        case UnicodeCategory.Format:
        case UnicodeCategory.Surrogate:
        case UnicodeCategory.PrivateUse:
        case UnicodeCategory.DashPunctuation:
        case UnicodeCategory.OpenPunctuation:
        case UnicodeCategory.ClosePunctuation:
        case UnicodeCategory.InitialQuotePunctuation:
        case UnicodeCategory.FinalQuotePunctuation:
        case UnicodeCategory.OtherPunctuation:
        case UnicodeCategory.MathSymbol:
        case UnicodeCategory.CurrencySymbol:
        case UnicodeCategory.ModifierSymbol:
        case UnicodeCategory.OtherSymbol:
        case UnicodeCategory.OtherNotAssigned:
          return false;
        default:
          return false;
      }
      return true;
    }

    internal static void CheckValidIdentifier(string ident)
    {
      if (!CodeGenerator.IsValidLanguageIndependentIdentifier(ident))
        throw new ArgumentException(Res.GetString(Res.XmlInvalidIdentifier, ident), "ident");
    }

    static int GetCSharpName(Type t, Type[] parameters, int index, StringBuilder sb)
    {
      if (t.DeclaringType != null && t.DeclaringType != t)
      {
        index = GetCSharpName(t.DeclaringType, parameters, index, sb);
        sb.Append(".");
      }
      string name = t.Name;
      int nameEnd = name.IndexOf('`');
      if (nameEnd < 0)
      {
        nameEnd = name.IndexOf('!');
      }
      if (nameEnd > 0)
      {
        EscapeKeywords(name.Substring(0, nameEnd), csharp, sb);
        sb.Append("<");
        int arguments = Int32.Parse(name.Substring(nameEnd + 1), CultureInfo.InvariantCulture) + index;
        for (; index < arguments; index++)
        {
          sb.Append(GetCSharpName(parameters[index]));
          if (index < arguments - 1)
          {
            sb.Append(",");
          }
        }
        sb.Append(">");
      }
      else
      {
        EscapeKeywords(name, csharp, sb);
      }
      return index;
    }

    internal static string GetCSharpName(Type t)
    {
      int rank = 0;
      while (t.IsArray)
      {
        t = t.GetElementType();
        rank++;
      }
      StringBuilder sb = new StringBuilder();
      sb.Append("global::");
      string ns = t.Namespace;
      if (ns != null && ns.Length > 0)
      {
        string[] parts = ns.Split('.');
        for (int i = 0; i < parts.Length; i++)
        {
          EscapeKeywords(parts[i], csharp, sb);
          sb.Append(".");
        }
      }

      Type[] arguments = t.IsGenericType || t.ContainsGenericParameters ? t.GetGenericArguments() : new Type[0];
      GetCSharpName(t, arguments, 0, sb);
      for (int i = 0; i < rank; i++)
      {
        sb.Append("[]");
      }
      return sb.ToString();
    }

    //
    /*
    internal static string GetTypeName(string name, CodeDomProvider codeProvider) {
        return codeProvider.GetTypeOutput(new CodeTypeReference(name));
    }
    */

    static void EscapeKeywords(string identifier, CodeDomProvider codeProvider, StringBuilder sb)
    {
      if (identifier == null || identifier.Length == 0)
        return;
      string originalIdentifier = identifier;
      int arrayCount = 0;
      while (identifier.EndsWith("[]", StringComparison.Ordinal))
      {
        arrayCount++;
        identifier = identifier.Substring(0, identifier.Length - 2);
      }
      if (identifier.Length > 0)
      {
        CheckValidIdentifier(identifier);
        identifier = codeProvider.CreateEscapedIdentifier(identifier);
        sb.Append(identifier);
      }
      for (int i = 0; i < arrayCount; i++)
      {
        sb.Append("[]");
      }
    }
  }
}