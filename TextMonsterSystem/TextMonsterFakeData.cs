#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMonsterSystem.Core;

#endregion

namespace TextMonsterSystem
{
  public sealed class TextMonsterFakeData : ITextMonster
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich die Anzahl der Zeilen
    /// </summary>
    long lineCount;

    /// <summary>
    /// merkt sich, ob am Anfang jeder Zeile die Zeilennummer stehen soll
    /// </summary>
    bool showLineNumbers;

    /// <summary>
    /// merkt sich die gewünschte Breite einer Zeile
    /// </summary>
    long lineWidth;

    #region # struct GenLine // Struktur einer Generierungs-Zeile
    /// <summary>
    /// Struktur einer Generierungs-Zeile
    /// </summary>
    struct GenLine
    {
      /// <summary>
      /// merkt sich den Inhalt der Zeile
      /// </summary>
      public string data;

      /// <summary>
      /// gibt an, wie oft der Inhalt wiederholt hinzugefügt werden soll (Standard: 0 = keine zusätzlichen Wiederholungen)
      /// </summary>
      public long addRepeat;

      /// <summary>
      /// gibt die genaue Länge der berechneten Zeile zurück
      /// </summary>
      public long Length
      {
        get
        {
          return (data.Length + 1) * (addRepeat + 1);
        }
      }

      /// <summary>
      /// gibt die entsprechenden Zeichen der gesamten Zeile zurück
      /// </summary>
      /// <param name="start">Startposition, ab welcher die Zeichen gelesen werden sollen</param>
      /// <returns>Enumerable der entsprechenden Zeichen</returns>
      public IEnumerable<char> GetChars(long start)
      {
        long max = Length - 1;
        long pos = start;

        while (pos < data.Length)
        {
          yield return data[(int)pos++];
        }

        if (addRepeat > 0)
        {
          char[] fillData = (" " + data).ToCharArray();
          int fillPos = (int)((pos - data.Length) % fillData.Length);

          while (pos < max)
          {
            yield return fillData[fillPos++];

            if (fillPos == fillData.Length) fillPos = 0;
            pos++;
          }
        }

        yield return '\n';
      }

      /// <summary>
      /// erstellt die Zeilen anhand eines Textes mit maximaler Zeilen-Breite
      /// </summary>
      /// <param name="text">Text, welcher verarbeitet werden soll</param>
      /// <param name="maxLength">maximale Länge einer Zeile</param>
      /// <returns>Enumerable aller genertierten Zeilen</returns>
      public static IEnumerable<GenLine> CreateLines(string text, long maxLength)
      {
        foreach (var line in text.Split('\n'))
        {

          if (line.Length <= maxLength)
          {
            yield return new GenLine { data = line, addRepeat = maxLength / line.Length - 1 };
            continue;
          }

          var wordsSum = new StringBuilder();

          foreach (var word in line.Split(' '))
          {
            if (wordsSum.Length + word.Length > maxLength && wordsSum.Length > 0)
            {
              yield return new GenLine { data = wordsSum.ToString(0, wordsSum.Length - 1), addRepeat = 0 };
              wordsSum.Clear();
            }

            wordsSum.Append(word).Append(' ');
          }

          if (wordsSum.Length > 1)
          {
            yield return new GenLine { data = wordsSum.ToString(0, wordsSum.Length - 1), addRepeat = 0 };
          }
        }
      }
    }
    #endregion

    /// <summary>
    /// merkt sich die Daten aller Generierungszeilen
    /// </summary>
    GenLine[] genLines;

    /// <summary>
    /// merkt sich die Größe eines Zeilen-Blockes
    /// </summary>
    long genBlockSize;
    #endregion

    #region # // --- Konstruktor / Dispose ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="lineCount">Anzahl der Zeilen, welche genertiert werden sollen (Standard: 1.000.000)</param>
    /// <param name="showLineNumbers">gibt an, ob die Zeilen-Nummern im Text angezeigt werden sollen (Standard: false)</param>
    /// <param name="lineWidth">gewünschte Breite der Zeilen (Standard: 200)</param>
    public TextMonsterFakeData(long lineCount = 1000000L, bool showLineNumbers = false, long lineWidth = 200)
    {
      if (lineCount < 0 || lineWidth < 0) throw new ArgumentOutOfRangeException();

      if (showLineNumbers)
      {
        throw new NotImplementedException();
      }

      this.lineCount = lineCount;
      this.showLineNumbers = showLineNumbers;
      this.lineWidth = lineWidth;

      #region # string loremIpsum = "Lorem ipsum dolor...
      string loremIpsum = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et " +
                          "dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet " +
                          "clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, con" +
                          "setetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed" +
                          " diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea ta" +
                          "kimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed d" +
                          "iam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et " +
                          "accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum " +
                          "dolor sit amet.\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel" +
                          " illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesen" +
                          "t luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetue" +
                          "r adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.\nUt" +
                          " wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea co" +
                          "mmodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel" +
                          " illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesen" +
                          "t luptatum zzril delenit augue duis dolore te feugait nulla facilisi.\nNam liber tempor cum soluta nobis elei" +
                          "fend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit am" +
                          "et, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam era" +
                          "t volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut al" +
                          "iquip ex ea commodo consequat.\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie" +
                          " consequat, vel illum dolore eu feugiat nulla facilisis.\nAt vero eos et accusam et justo duo dolores et ea r" +
                          "ebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor si" +
                          "t amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquya" +
                          "m erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren" +
                          ", no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing e" +
                          "litr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt j" +
                          "usto labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua. est L" +
                          "orem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod te" +
                          "mpor invidunt ut labore et dolore magna aliquyam erat.\nConsetetur sadipscing elitr, sed diam nonumy eirmod t" +
                          "empor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo du" +
                          "o dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lor" +
                          "em ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dol" +
                          "ore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet cli" +
                          "ta kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, conset" +
                          "etur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed di" +
                          "am voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takim" +
                          "ata sanctus.\n";
      #endregion

      loremIpsum = loremIpsum.Replace("\n", "\n\n");

      long minWidth = loremIpsum.Split(' ', '\n').Max(x => x.Length);
      if (this.lineWidth < minWidth) this.lineWidth = minWidth;

      genLines = GenLine.CreateLines(loremIpsum, this.lineWidth).ToArray();
      genBlockSize = genLines.Sum(x => x.Length);
    }

    /// <summary>
    /// alle Ressourcen wieder frei geben
    /// </summary>
    public override void Dispose()
    {
    }
    #endregion

    #region # // --- Properties ---
    /// <summary>
    /// gibt die aktuelle Anzahl der gespeicherten Zeichen zurück
    /// </summary>
    public override long Length
    {
      get
      {
        long blockCount = lineCount / genLines.Length;

        long result = genBlockSize * blockCount +
                      genLines.Take((int)(lineCount - (blockCount * genLines.Length))).Sum(x => x.Length);

        if (showLineNumbers)
        {
          throw new NotImplementedException();
        }

        return result;
      }
    }

    /// <summary>
    /// gibt die theoretisch maximale Anzahl der verarbeitbaren Zeichen zurück (absolutes Limit)
    /// </summary>
    public override long LengthLimit
    {
      get
      {
        return long.MaxValue;
      }
    }

    /// <summary>
    /// gibt die Anzahl der Zeilen zurück
    /// </summary>
    public override long Lines
    {
      get
      {
        return lineCount;
      }
    }

    /// <summary>
    /// gibt den aktuell genutzen Arbeitsspeicher zurück
    /// </summary>
    public override long ByteUsedRam
    {
      get
      {
        return genLines.Sum(x => x.data.Length * 2 + 16);
      }
    }

    /// <summary>
    /// gibt den aktuell genutzen Speicher auf der Festplatte zurück
    /// </summary>
    public override long ByteUsedDrive { get { return 0; } }
    #endregion

    #region # // --- Methoden ---

    #region # // --- GetPos() ---
    /// <summary>
    /// gibt die Speicherposition anhand einer absoluten Zeichenposition zurück
    /// </summary>
    /// <param name="charPos">Zeichenposition, welche abgefragt werden soll</param>
    /// <returns>passende Speicherposition</returns>
    public override MemoryPos GetMemoryPos(long charPos)
    {
      return new MemoryPos { pos = charPos };
    }

    /// <summary>
    /// gibt die absolute Zeichenposition anhand einer Speicherposition zurück
    /// </summary>
    /// <param name="memPos">Speicherposition, welche abgefragt werden soll</param>
    /// <returns>passende absolute Zeichenposition</returns>
    public override long GetCharPos(MemoryPos memPos)
    {
      return memPos.pos;
    }
    #endregion

    #region # // --- Insert() ---
    /// <summary>
    /// fügt ein einzelnes Zeichen in den Speicher ein
    /// </summary>
    /// <param name="memPos">Startposition, wo das Zeichen eingefügt werden soll</param>
    /// <param name="value">das Zeichen, welches eingefügt werden soll</param>
    /// <returns>neue Speicherposition am Ende des eingefügten Zeichens</returns>
    public override MemoryPos Insert(MemoryPos memPos, char value)
    {
      throw new NotSupportedException();
    }
    #endregion

    #region # // --- Remove() ---
    /// <summary>
    /// löscht betimmte Zeichen aus dem Speicher
    /// </summary>
    /// <param name="memPosStart">Startposition, wo Daten im Speicher gelöscht werden sollen</param>
    /// <param name="memPosEnd">Endposition, bis zu den Daten, welche Daten gelöscht werden sollen</param>
    /// <returns>Länge der Daten, welche gelöscht wurden</returns>
    public override void Remove(MemoryPos memPosStart, MemoryPos memPosEnd)
    {
      throw new NotSupportedException();
    }
    #endregion

    #region # // --- GetChars() ---
    /// <summary>
    /// gibt die Zeichen aus dem Speicher zurück
    /// </summary>
    /// <param name="memPos">Startposition, wo die Zeichen im Speicher gelesen werden sollen</param>
    /// <returns>Enumerable mit den entsprechenden Zeichen</returns>
    public override IEnumerable<char> GetChars(MemoryPos memPos)
    {
      long pos = memPos.pos;
      if (pos >= Length) yield break;

      long blockNum = pos / genBlockSize;
      long line = blockNum * genLines.Length;
      pos -= genBlockSize * blockNum;

      int genPos = 0;
      foreach (var genLine in genLines)
      {
        long len = genLine.Length;
        if (len <= pos)
        {
          pos -= len;
          genPos++;
          line++;
          continue;
        }
        foreach (var c in genLine.GetChars(pos)) yield return c;
        genPos++;
        line++;
        break;
      }

      while (line < lineCount)
      {
        if (genPos == genLines.Length) genPos = 0;
        foreach (var c in genLines[genPos++].GetChars(pos)) yield return c;
        line++;
      }
    }
    #endregion

    #region # // --- GetLine() ---
    /// <summary>
    /// gibt den Anfang einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf den Anfang der Zeile</returns>
    public override MemoryPos GetLineStart(MemoryPos memPos)
    {
      throw new NotImplementedException("todo");
    }

    /// <summary>
    /// gibt das Ende einer Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Speicherposition auf das Ende der Zeile (hinter dem letzten Zeichen)</returns>
    public override MemoryPos GetLineEnd(MemoryPos memPos)
    {
      throw new NotImplementedException("todo");
    }

    /// <summary>
    /// gibt Position einer gesamten Zeile zurück
    /// </summary>
    /// <param name="memPos">Speicherposition ab welcher gesucht werden soll</param>
    /// <returns>Position der gesamten Zeile (Anfang und Ende der Zeile)</returns>
    public override LinePos GetLine(MemoryPos memPos)
    {
      throw new NotImplementedException("todo");
      //      return new LinePos { lineStart = mem.GetLineStart(memPos), lineEnd = mem.GetLineEnd(memPos) };
    }

    /// <summary>
    /// gibt die Position der nachfolgenden Zeile zurück
    /// </summary>
    /// <param name="linePos">vorherige Zeilenposition</param>
    /// <returns>nachfolgende Zeilenposition</returns>
    public override LinePos GetNextLine(LinePos linePos)
    {
      throw new NotImplementedException("todo");
      //if (!linePos.Valid) return LinePos.InvalidPos;

      //long p = mem.GetCharPos(linePos.lineEnd) + 1;
      //if (p > mem.Length) return LinePos.InvalidPos;
      //if (p < mem.Length && mem.GetChars(p, 1).First() == '\r') p++;

      //MemoryPos nextLineStart = mem.GetMemoryPos(p);
      //return new LinePos { lineStart = nextLineStart, lineEnd = mem.GetLineEnd(nextLineStart) };
    }

    /// <summary>
    /// gibt die Position der vorhergehenden Zeile zurück
    /// </summary>
    /// <param name="linePos">bisherige Zeilenposition</param>
    /// <returns>vorhergehende Zeilenposition</returns>
    public override LinePos GetPrevLine(LinePos linePos)
    {
      throw new NotImplementedException("todo");
      //if (!linePos.Valid) return LinePos.InvalidPos;

      //long p = mem.GetCharPos(linePos.lineStart) - 1;
      //if (p < 0) return LinePos.InvalidPos;
      //if (p > 0 && mem.GetChars(p - 1, 1).First() == '\r') p--;

      //MemoryPos prevLineEnd = mem.GetMemoryPos(p);
      //return new LinePos { lineStart = mem.GetLineStart(prevLineEnd), lineEnd = prevLineEnd };
    }

    /// <summary>
    /// gibt die Position einer bestimmten Zeile zurück
    /// </summary>
    /// <param name="lineNumber">Zeilennummer, welche zurück gegeben werden soll (beginnend bei 0)</param>
    /// <returns>entsprechende Zeilenposition</returns>
    public override LinePos GetLine(long lineNumber)
    {
      throw new NotImplementedException("todo");
      //if (lineNumber < 0) return LinePos.InvalidPos;

      //long p = 0;
      //long memLength = mem.Length;
      //while (lineNumber > 0)
      //{
      //  p = mem.GetCharPos(mem.GetLineEnd(mem.GetMemoryPos(p))) + 1;
      //  if (p > memLength) return LinePos.InvalidPos;
      //  if (p < memLength && mem.GetChars(p, 1).First() == '\r') p++;
      //  lineNumber--;
      //}

      //MemoryPos lineStart = mem.GetMemoryPos(p);
      //return new LinePos { lineStart = lineStart, lineEnd = mem.GetLineEnd(lineStart) };
    }
    #endregion

    #endregion
  }
}
