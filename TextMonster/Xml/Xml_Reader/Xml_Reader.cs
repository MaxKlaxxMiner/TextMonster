using System;
using System.Xml;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Stellt einen Reader dar, der einen schnellen Zugriff auf XML-Daten bietet, ohne Zwischenspeicher und welcher nur vorwärts möglich ist.Um den .NET Framework-Quellcode für diesen Typ zu durchsuchen, rufen Sie die Verweisquelle auf.
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public abstract class Xml_Reader : IDisposable
  {
    private static uint IsTextualNodeBitmap = 24600U;
    private static uint CanReadContentAsBitmap = 123324U;
    private static uint HasValueBitmap = 157084U;
    internal const int DefaultBufferSize = 4096;
    internal const int BiggerBufferSize = 8192;
    internal const int MaxStreamLengthForDefaultBufferSize = 65536;
    internal const int AsyncBufferSize = 65536;

    /// <summary>
    /// Ruft das zum Erstellen dieser <see cref="T:System.Xml.XmlReader"/>-Instanz verwendete <see cref="T:System.Xml.XmlReaderSettings"/>-Objekt ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das zum Erstellen dieser Reader-Instanz verwendete <see cref="T:System.Xml.XmlReaderSettings"/>-Objekt.Wenn dieser Reader nicht mit der <see cref="Overload:System.Xml.XmlReader.Create"/>-Methode erstellt wurde, gibt diese Eigenschaft null zurück.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    public virtual XmlReaderSettings Settings
    {
      get
      {
        return (XmlReaderSettings)null;
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Typ des aktuellen Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Einer der Enumerationswerte, die den Typ des aktuellen Knotens angeben.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract XmlNodeType NodeType { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den gekennzeichneten Namen des aktuellen Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der gekennzeichnete Name des aktuellen Knotens.Der Name für das &lt;bk:book&gt;-Element lautet z. B. bk:book.Der zurückgegebene Name hängt vom <see cref="P:System.Xml.XmlReader.NodeType"/> des Knotens ab.Die folgenden Knotentypen geben die jeweils aufgeführten Werte zurück.Alle anderen Knotentypen geben eine leere Zeichenfolge zurück.Knotentyp Name AttributeDer Name des Attributs. DocumentTypeDer Name des Dokumenttyps. ElementDer Tagname. EntityReferenceDer Name der Entität, auf die verwiesen wird. ProcessingInstructionDas Ziel der Verarbeitungsanweisung. XmlDeclarationDas xml-Zeichenfolgenliteral.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string Name
    {
      [__DynamicallyInvokable]
      get
      {
        if (this.Prefix.Length == 0)
          return this.LocalName;
        return this.NameTable.Add(this.Prefix + ":" + this.LocalName);
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den lokalen Namen des aktuellen Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Name des aktuellen Knotens ohne das Präfix.Der LocalName für das &lt;bk:book&gt;-Element lautet z. B. book.Bei unbenannten Knotentypen wie Text, Comment usw. gibt diese Eigenschaft String.Empty zurück.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string LocalName { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Namespace-URI (entsprechend der Definition in der Namespacespezifikation des W3C) des Knotens ab, auf dem der Reader positioniert ist.
    /// </summary>
    /// 
    /// <returns>
    /// Der Namespace-URI des aktuellen Knotens, andernfalls eine leere Zeichenfolge.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string NamespaceURI { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse das dem aktuellen Knoten zugeordnete Namespacepräfix ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das dem aktuellen Knoten zugeordnete Namespacepräfix.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string Prefix { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Knoten einen <see cref="P:System.Xml.XmlReader.Value"/> aufweisen kann.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der Knoten, auf dem der Reader derzeit positioniert ist, einen Value aufweisen darf, andernfalls false.Wenn false, weist der Knoten den Wert String.Empty auf.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool HasValue
    {
      [__DynamicallyInvokable]
      get
      {
        return Xml_Reader.HasValueInternal(this.NodeType);
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Textwert des aktuellen Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der zurückgegebene Wert hängt vom <see cref="P:System.Xml.XmlReader.NodeType"/> des Knotens ab.In der folgenden Tabelle sind Knotentypen aufgeführt, die einen zurückzugebenden Wert haben.Alle anderen Knotentypen geben String.Empty zurück.Knotentyp Wert AttributeDer Wert des Attributs. CDATADer Inhalt des CDATA-Abschnitts. CommentDer Inhalt des Kommentars. DocumentTypeDie interne Teilmenge. ProcessingInstructionDer gesamte Inhalt mit Ausnahme des Ziels. SignificantWhitespaceDer Leerraum zwischen Markups bei einem Modell für gemischten Inhalt. TextDer Inhalt des Textknotens. WhitespaceDer Leerraum zwischen Markups. XmlDeclarationDer Inhalt der Deklaration.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string Value { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse die Tiefe des aktuellen Knotens im XML-Dokument ab.
    /// </summary>
    /// 
    /// <returns>
    /// Die Tiefe des aktuellen Knotens im XML-Dokument.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract int Depth { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Basis-URI des aktuellen Knotens ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Basis-URI des aktuellen Knotens.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string BaseURI { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Knoten ein leeres Element ist (z. B. &lt;MyElement/&gt;).
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der aktuelle Knoten ein Element ist (<see cref="P:System.Xml.XmlReader.NodeType"/> ist gleich XmlNodeType.Element), das mit /&gt; endet, andernfalls false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract bool IsEmptyElement { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Knoten ein Attribut ist, das aus dem in der DTD oder dem Schema definierten Standardwert generiert wurde.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der aktuelle Knoten ein Attribut ist, dessen Wert aus dem in der DTD oder dem Schema definierten Standardwert generiert wurde. false, wenn der Attributwert explizit festgelegt wurde.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool IsDefault
    {
      [__DynamicallyInvokable]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse das Anführungszeichen ab, mit dem der Wert eines Attributknotens eingeschlossen wird.
    /// </summary>
    /// 
    /// <returns>
    /// Das Anführungszeichen (" oder '), mit dem der Wert eines Attributknotens eingeschlossen ist.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    public virtual char QuoteChar
    {
      get
      {
        return '"';
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den aktuellen xml:space-Bereich ab.
    /// </summary>
    /// 
    /// <returns>
    /// Einer der <see cref="T:System.Xml.XmlSpace"/>-Werte.Wenn kein xml:space-Bereich vorhanden ist, wird für diese Eigenschaft standardmäßig XmlSpace.None festgelegt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual XmlSpace XmlSpace
    {
      [__DynamicallyInvokable]
      get
      {
        return XmlSpace.None;
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den aktuellen xml:lang-Bereich ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der aktuelle xml:lang-Bereich.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string XmlLang
    {
      [__DynamicallyInvokable]
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Ruft die Schemainformationen ab, die dem aktuellen Knoten nach der Schemavalidierung zugewiesen wurden.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Xml.Schema.IXmlSchemaInfo"/>-Objekt, das die Schemainformationen für den aktuellen Knoten enthält.Schemainformationen können auf Elemente, Attribute oder Textknoten mit einem <see cref="P:System.Xml.XmlReader.ValueType"/> festgelegt werden, der nicht NULL (typisierte Werte) ist.Wenn der aktuelle Knoten keinem der oben angegebenen Knotentypen angehört oder wenn die XmlReader-Instanz keine Schemainformationen übermittelt, gibt diese Eigenschaft null zurück.Wenn diese Eigenschaft von einem <see cref="T:System.Xml.XmlTextReader"/>-Objekt oder einem <see cref="T:System.Xml.XmlValidatingReader"/>-Objekt aufgerufen wird, gibt diese Eigenschaft stets null zurück.Die XmlReader-Implementierungen machen über die SchemaInfo-Eigenschaft keine Schemainformationen verfügbar.HinweisWenn Sie den Informationensatz für die Post-Schema-Validierung (PSVI) für ein Element abrufen müssen, positionieren Sie den Reader im Endtag des Elements und nicht im Starttag.Der PSVI wird über die SchemaInfo-Eigenschaft eines Readers abgerufen.Der überprüfende Reader, der durch <see cref="Overload:System.Xml.XmlReader.Create"/> mit der <see cref="P:System.Xml.XmlReaderSettings.ValidationType"/>-Eigenschaft erstellt wurde, welche auf <see cref="F:System.Xml.ValidationType.Schema"/> festgelegt ist, verfügt nur über den vollständigen PSVI für ein Element, wenn der Reader im Endtag eines Elements positioniert ist.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    public virtual IXmlSchemaInfo SchemaInfo
    {
      get
      {
        return this as IXmlSchemaInfo;
      }
    }

    /// <summary>
    /// Ruft den CLR-Typ (Common Language Runtime) für den aktuellen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der CLR-Typ, der dem typisierten Wert des Knotens entspricht.Die Standardeinstellung ist System.String.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual Type ValueType
    {
      [__DynamicallyInvokable]
      get
      {
        return typeof(string);
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse die Anzahl der Attribute für den aktuellen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der Attribute im aktuellen Knoten.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract int AttributeCount { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Wert des Attributs mit dem angegebenen Index ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Wert des angegebenen Attributs.
    /// </returns>
    /// <param name="i">Der Index des Attributs.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string this[int i]
    {
      [__DynamicallyInvokable]
      get
      {
        return this.GetAttribute(i);
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Wert des Attributs mit dem angegebenen <see cref="P:System.Xml.XmlReader.Name"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Wert des angegebenen Attributs.Wenn das Attribut nicht gefunden wurde, wird null zurückgegeben.
    /// </returns>
    /// <param name="name">Der qualifizierte Name des Attributs.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string this[string name]
    {
      [__DynamicallyInvokable]
      get
      {
        return this.GetAttribute(name);
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Wert des Attributs mit dem angegebenen <see cref="P:System.Xml.XmlReader.LocalName"/> und <see cref="P:System.Xml.XmlReader.NamespaceURI"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Wert des angegebenen Attributs.Wenn das Attribut nicht gefunden wurde, wird null zurückgegeben.
    /// </returns>
    /// <param name="name">Der lokale Name des Attributs.</param><param name="namespaceURI">Der Namespace-URI dieses Attributs.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string this[string name, string namespaceURI]
    {
      [__DynamicallyInvokable]
      get
      {
        return this.GetAttribute(name, namespaceURI);
      }
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob sich der Reader am Ende des Streams befindet.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der Reader am Ende des Streams positioniert ist, andernfalls false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract bool EOF { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Zustand des Readers ab.
    /// </summary>
    /// 
    /// <returns>
    /// Einer der Enumerationswerte, der den Status des Readers angibt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract ReadState ReadState { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse die <see cref="T:System.Xml.XmlNameTable"/> ab, die dieser Implementierung zugeordnet ist.
    /// </summary>
    /// 
    /// <returns>
    /// Die XmlNameTable, die das Abrufen der atomisierten Version einer Zeichenfolge innerhalb des Knotens erlaubt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract XmlNameTable NameTable { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob dieser Reader Entitäten analysieren und auflösen kann.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der Reader Entitäten analysieren und auflösen kann, andernfalls false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool CanResolveEntity
    {
      [__DynamicallyInvokable]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob der <see cref="T:System.Xml.XmlReader"/> die Methoden für das Lesen von Inhalt im Binärformat implementiert.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn die Methoden für das Lesen von Inhalt im Binärformat implementiert werden, andernfalls false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool CanReadBinaryContent
    {
      [__DynamicallyInvokable]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob der <see cref="T:System.Xml.XmlReader"/> die angegebene <see cref="M:System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)"/>-Methode implementiert.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der <see cref="T:System.Xml.XmlReader"/> die <see cref="M:System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)"/>-Methode implementiert, andernfalls false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool CanReadValueChunk
    {
      [__DynamicallyInvokable]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Ruft einen Wert ab, der angibt, ob der aktuelle Knoten über Attribute verfügt.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der aktuelle Knoten über Attribute verfügt, andernfalls false.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool HasAttributes
    {
      [__DynamicallyInvokable]
      get
      {
        return this.AttributeCount > 0;
      }
    }

    internal virtual XmlNamespaceManager NamespaceManager
    {
      get
      {
        return (XmlNamespaceManager)null;
      }
    }

    internal bool IsDefaultInternal
    {
      get
      {
        if (this.IsDefault)
          return true;
        IXmlSchemaInfo schemaInfo = this.SchemaInfo;
        return schemaInfo != null && schemaInfo.IsDefault;
      }
    }

    internal virtual IDtdInfo DtdInfo
    {
      get
      {
        return (IDtdInfo)null;
      }
    }

    private object debuggerDisplayProxy
    {
      get
      {
        return (object)new Xml_Reader.XmlReaderDebuggerDisplayProxy(this);
      }
    }

    /// <summary>
    /// Initialisiert eine neue Instanz derXmlReader-Klasse.
    /// </summary>
    [__DynamicallyInvokable]
    protected Xml_Reader()
    {
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als <see cref="T:System.Object"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als geeignetstes CLR-Objekt (Common Language Runtime).
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual object ReadContentAsObject()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsObject");
      return (object)this.InternalReadContentAsString();
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als Boolean.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als <see cref="T:System.Boolean"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadContentAsBoolean()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsBoolean");
      try
      {
        return XmlConvert.ToBoolean(this.InternalReadContentAsString());
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "Boolean", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als <see cref="T:System.DateTime"/>-Objekt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als <see cref="T:System.DateTime"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    public virtual DateTime ReadContentAsDateTime()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsDateTime");
      try
      {
        return XmlConvert.ToDateTime(this.InternalReadContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "DateTime", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als <see cref="T:System.DateTimeOffset"/>-Objekt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als <see cref="T:System.DateTimeOffset"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual DateTimeOffset ReadContentAsDateTimeOffset()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsDateTimeOffset");
      try
      {
        return XmlConvert.ToDateTimeOffset(this.InternalReadContentAsString());
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "DateTimeOffset", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als Gleitkommazahl mit doppelter Genauigkeit.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als Gleitkommazahl mit doppelter Genauigkeit.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual double ReadContentAsDouble()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsDouble");
      try
      {
        return XmlConvert.ToDouble(this.InternalReadContentAsString());
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "Double", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als Gleitkommazahl mit einfacher Genauigkeit.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt an der aktuellen Position als Gleitkommazahl mit einfacher Genauigkeit.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual float ReadContentAsFloat()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsFloat");
      try
      {
        return XmlConvert.ToSingle(this.InternalReadContentAsString());
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "Float", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als <see cref="T:System.Decimal"/>-Objekt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt an der aktuellen Position als <see cref="T:System.Decimal"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual Decimal ReadContentAsDecimal()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsDecimal");
      try
      {
        return XmlConvert.ToDecimal(this.InternalReadContentAsString());
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "Decimal", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als 32-Bit-Ganzzahl mit Vorzeichen.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als 32-Bit-Ganzzahl mit Vorzeichen.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadContentAsInt()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsInt");
      try
      {
        return XmlConvert.ToInt32(this.InternalReadContentAsString());
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "Int", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als 64-Bit-Ganzzahl mit Vorzeichen.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als 64-Bit-Ganzzahl mit Vorzeichen.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual long ReadContentAsLong()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsLong");
      try
      {
        return XmlConvert.ToInt64(this.InternalReadContentAsString());
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", "Long", (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position als <see cref="T:System.String"/>-Objekt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als <see cref="T:System.String"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.FormatException">Das Zeichenfolgenformat ist nicht gültig.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string ReadContentAsString()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsString");
      return this.InternalReadContentAsString();
    }

    /// <summary>
    /// Liest den Inhalt als Objekt vom angegebenen Typ.
    /// </summary>
    /// 
    /// <returns>
    /// Der verkettete Textinhalt oder Attributwert, der in den angeforderten Typ konvertiert wurde.
    /// </returns>
    /// <param name="returnType">Der Typ des zurückzugebenden Werts.Hinweis   Seit der Veröffentlichung von .NET Framework 3.5 kann der Wert des <paramref name="returnType"/>-Parameters nun auch auf den <see cref="T:System.DateTimeOffset"/>-Typ festgelegt werden.</param><param name="namespaceResolver">Ein <see cref="T:System.Xml.IXmlNamespaceResolver"/>-Objekt, das für die Auflösung von Präfixen von Namespaces verwendet wird, die im Zusammenhang mit der Typkonvertierung stehen.Dieses kann zum Beispiel beim Konvertieren eines <see cref="T:System.Xml.XmlQualifiedName"/>-Objekts in eine xs:string verwendet werden.Dieser Wert kann null sein.</param><exception cref="T:System.FormatException">Der Inhalt weist nicht das richtige Format für den Zieltyp auf.</exception><exception cref="T:System.InvalidCastException">Die versuchte Typumwandlung ist ungültig.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="returnType"/>-Wert ist null.</exception><exception cref="T:System.InvalidOperationException">Der aktuelle Knoten ist kein unterstützter Knotentyp.Weitere Informationen finden Sie in der nachfolgenden Tabelle.</exception><exception cref="T:System.OverflowException">Lesen von Decimal.MaxValue.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAs");
      string str = this.InternalReadContentAsString();
      if (returnType == typeof(string))
        return (object)str;
      try
      {
        return XmlUntypedConverter.Untyped.ChangeType(str, returnType, namespaceResolver == null ? this as IXmlNamespaceResolver : namespaceResolver);
      }
      catch (FormatException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", returnType.ToString(), (Exception)ex, this as IXmlLineInfo);
      }
      catch (InvalidCastException ex)
      {
        throw new XmlException("Xml_ReadContentAsFormatException", returnType.ToString(), (Exception)ex, this as IXmlLineInfo);
      }
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als <see cref="T:System.Object"/> zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein geschachteltes CLR-Objekt (Common Language Runtime) des geeignetsten Typs.Die <see cref="P:System.Xml.XmlReader.ValueType"/>-Eigenschaft bestimmt den geeigneten CLR-Typ.Wenn der Inhalt als Listentyp typisiert ist, gibt diese Methode ein Array der geschachtelten Objekte des geeigneten Typs zurück.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual object ReadElementContentAsObject()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsObject"))
        return (object)string.Empty;
      object obj = this.ReadContentAsObject();
      this.FinishReadElementContentAsXxx();
      return obj;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als <see cref="T:System.Object"/> zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein geschachteltes CLR-Objekt (Common Language Runtime) des geeignetsten Typs.Die <see cref="P:System.Xml.XmlReader.ValueType"/>-Eigenschaft bestimmt den geeigneten CLR-Typ.Wenn der Inhalt als Listentyp typisiert ist, gibt diese Methode ein Array der geschachtelten Objekte des geeigneten Typs zurück.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual object ReadElementContentAsObject(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsObject();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als <see cref="T:System.Boolean"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.Boolean"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in ein <see cref="T:System.Boolean"/>-Objekt konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadElementContentAsBoolean()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsBoolean"))
        return XmlConvert.ToBoolean(string.Empty);
      int num = this.ReadContentAsBoolean() ? 1 : 0;
      this.FinishReadElementContentAsXxx();
      return num != 0;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als <see cref="T:System.Boolean"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.Boolean"/>-Objekt.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadElementContentAsBoolean(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsBoolean();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als <see cref="T:System.DateTime"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.DateTime"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in ein <see cref="T:System.DateTime"/>-Objekt konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    public virtual DateTime ReadElementContentAsDateTime()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsDateTime"))
        return XmlConvert.ToDateTime(string.Empty, XmlDateTimeSerializationMode.RoundtripKind);
      DateTime dateTime = this.ReadContentAsDateTime();
      this.FinishReadElementContentAsXxx();
      return dateTime;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als <see cref="T:System.DateTime"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.DateTime"/>-Objekt.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    public virtual DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsDateTime();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als Gleitkommazahl mit doppelter Genauigkeit zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als Gleitkommazahl mit doppelter Genauigkeit.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in eine Gleitkommazahl mit doppelter Genauigkeit konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual double ReadElementContentAsDouble()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsDouble"))
        return XmlConvert.ToDouble(string.Empty);
      double num = this.ReadContentAsDouble();
      this.FinishReadElementContentAsXxx();
      return num;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als Gleitkommazahl mit doppelter Genauigkeit zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als Gleitkommazahl mit doppelter Genauigkeit.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual double ReadElementContentAsDouble(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsDouble();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als Gleitkommazahl mit einfacher Genauigkeit zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als Gleitkommazahl mit einfacher Genauigkeit.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in eine Gleitkommazahl mit einfacher Genauigkeit konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual float ReadElementContentAsFloat()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsFloat"))
        return XmlConvert.ToSingle(string.Empty);
      double num = (double)this.ReadContentAsFloat();
      this.FinishReadElementContentAsXxx();
      return (float)num;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als Gleitkommazahl mit einfacher Genauigkeit zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als Gleitkommazahl mit einfacher Genauigkeit.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in eine Gleitkommazahl mit einfacher Genauigkeit konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual float ReadElementContentAsFloat(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsFloat();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als <see cref="T:System.Decimal"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.Decimal"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in <see cref="T:System.Decimal"/> konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual Decimal ReadElementContentAsDecimal()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsDecimal"))
        return XmlConvert.ToDecimal(string.Empty);
      Decimal num = this.ReadContentAsDecimal();
      this.FinishReadElementContentAsXxx();
      return num;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als <see cref="T:System.Decimal"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.Decimal"/>-Objekt.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in <see cref="T:System.Decimal"/> konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual Decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsDecimal();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als 32-Bit-Ganzzahl mit Vorzeichen zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als 32-Bit-Ganzzahl mit Vorzeichen.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in eine 32-Bit-Ganzzahl mit Vorzeichen konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadElementContentAsInt()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsInt"))
        return XmlConvert.ToInt32(string.Empty);
      int num = this.ReadContentAsInt();
      this.FinishReadElementContentAsXxx();
      return num;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als 32-Bit-Ganzzahl mit Vorzeichen zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als 32-Bit-Ganzzahl mit Vorzeichen.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in eine 32-Bit-Ganzzahl mit Vorzeichen konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadElementContentAsInt(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsInt();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als 64-Bit-Ganzzahl mit Vorzeichen zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als 64-Bit-Ganzzahl mit Vorzeichen.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in eine 64-Bit-Ganzzahl mit Vorzeichen konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual long ReadElementContentAsLong()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsLong"))
        return XmlConvert.ToInt64(string.Empty);
      long num = this.ReadContentAsLong();
      this.FinishReadElementContentAsXxx();
      return num;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als 64-Bit-Ganzzahl mit Vorzeichen zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als 64-Bit-Ganzzahl mit Vorzeichen.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in eine 64-Bit-Ganzzahl mit Vorzeichen konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual long ReadElementContentAsLong(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsLong();
    }

    /// <summary>
    /// Liest das aktuelle Element und gibt den Inhalt als <see cref="T:System.String"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.String"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in ein <see cref="T:System.String"/>-Objekt konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string ReadElementContentAsString()
    {
      if (!this.SetupReadElementContentAsXxx("ReadElementContentAsString"))
        return string.Empty;
      string str = this.ReadContentAsString();
      this.FinishReadElementContentAsXxx();
      return str;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, liest dann das aktuelle Element und gibt den Inhalt als <see cref="T:System.String"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.String"/>-Objekt.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in ein <see cref="T:System.String"/>-Objekt konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string ReadElementContentAsString(string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAsString();
    }

    /// <summary>
    /// Liest den Elementinhalt als angeforderten Typ.
    /// </summary>
    /// 
    /// <returns>
    /// Der in das angeforderte typisierte Objekt konvertierte Elementinhalt.
    /// </returns>
    /// <param name="returnType">Der Typ des zurückzugebenden Werts.Hinweis   Seit der Veröffentlichung von .NET Framework 3.5 kann der Wert des <paramref name="returnType"/>-Parameters nun auch auf den <see cref="T:System.DateTimeOffset"/>-Typ festgelegt werden.</param><param name="namespaceResolver">Ein <see cref="T:System.Xml.IXmlNamespaceResolver"/>-Objekt, das für die Auflösung von Präfixen von Namespaces verwendet wird, die im Zusammenhang mit der Typkonvertierung stehen.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.OverflowException">Lesen von Decimal.MaxValue.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
    {
      if (this.SetupReadElementContentAsXxx("ReadElementContentAs"))
      {
        object obj = this.ReadContentAs(returnType, namespaceResolver);
        this.FinishReadElementContentAsXxx();
        return obj;
      }
      if (!(returnType == typeof(string)))
        return XmlUntypedConverter.Untyped.ChangeType(string.Empty, returnType, namespaceResolver);
      return (object)string.Empty;
    }

    /// <summary>
    /// Überprüft, ob der angegebene lokale Name und der angegebene Namespace-URI mit denen des aktuellen Elements übereinstimmen, und liest dann den Elementinhalt als angeforderten Typ.
    /// </summary>
    /// 
    /// <returns>
    /// Der in das angeforderte typisierte Objekt konvertierte Elementinhalt.
    /// </returns>
    /// <param name="returnType">Der Typ des zurückzugebenden Werts.Hinweis   Seit der Veröffentlichung von .NET Framework 3.5 kann der Wert des <paramref name="returnType"/>-Parameters nun auch auf den <see cref="T:System.DateTimeOffset"/>-Typ festgelegt werden.</param><param name="namespaceResolver">Ein <see cref="T:System.Xml.IXmlNamespaceResolver"/>-Objekt, das für die Auflösung von Präfixen von Namespaces verwendet wird, die im Zusammenhang mit der Typkonvertierung stehen.</param><param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Der <see cref="T:System.Xml.XmlReader"/> wird nicht auf einem Element positioniert.</exception><exception cref="T:System.Xml.XmlException">Das aktuelle Element enthält untergeordnete Elemente.- oder - Der Elementinhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.ArgumentNullException">Die Methode wird mit null-Argumenten aufgerufen.</exception><exception cref="T:System.ArgumentException">Der angegebene lokale Name und der Namespace-URI stimmen nicht mit dem Element überein, das gerade gelesen wird.</exception><exception cref="T:System.OverflowException">Lesen von Decimal.MaxValue.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
    {
      this.CheckElement(localName, namespaceURI);
      return this.ReadElementContentAs(returnType, namespaceResolver);
    }

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Wert des Attributs mit dem angegebenen <see cref="P:System.Xml.XmlReader.Name"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Wert des angegebenen Attributs.Wenn das Attribut nicht gefunden wird oder Wert String.Empty ist, wird null zurückgegeben.
    /// </returns>
    /// <param name="name">Der qualifizierte Name des Attributs.</param><exception cref="T:System.ArgumentNullException"><paramref name="name"/> ist null.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string GetAttribute(string name);

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Wert des Attributs mit dem angegebenen <see cref="P:System.Xml.XmlReader.LocalName"/> und <see cref="P:System.Xml.XmlReader.NamespaceURI"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Wert des angegebenen Attributs.Wenn das Attribut nicht gefunden wird oder Wert String.Empty ist, wird null zurückgegeben.Diese Methode verschiebt den Reader nicht.
    /// </returns>
    /// <param name="name">Der lokale Name des Attributs.</param><param name="namespaceURI">Der Namespace-URI dieses Attributs.</param><exception cref="T:System.ArgumentNullException"><paramref name="name"/> ist null.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string GetAttribute(string name, string namespaceURI);

    /// <summary>
    /// Ruft beim Überschreiben in einer abgeleiteten Klasse den Wert des Attributs mit dem angegebenen Index ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Wert des angegebenen Attributs.Diese Methode verschiebt den Reader nicht.
    /// </returns>
    /// <param name="i">Der Index des Attributs.Der Index ist nullbasiert.(Das erste Attribut hat den Index 0.)</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="i"/> liegt außerhalb des Bereichs.Es darf nicht negativ sein und muss kleiner als die Größe der Attributauflistung sein.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string GetAttribute(int i);

    /// <summary>
    /// Wechselt beim Überschreiben in einer abgeleiteten Klasse zum Attribut mit dem angegebenen <see cref="P:System.Xml.XmlReader.Name"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn das Attribut gefunden wurde, andernfalls false.Bei einem Wert von false ändert sich die Position des Readers nicht.
    /// </returns>
    /// <param name="name">Der qualifizierte Name des Attributs.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentException">Der Parameter ist eine leere Zeichenfolge.</exception>
    [__DynamicallyInvokable]
    public abstract bool MoveToAttribute(string name);

    /// <summary>
    /// Wechselt beim Überschreiben in einer abgeleiteten Klasse zum Attribut mit dem angegebenen <see cref="P:System.Xml.XmlReader.LocalName"/> und dem angegebenen <see cref="P:System.Xml.XmlReader.NamespaceURI"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn das Attribut gefunden wurde, andernfalls false.Bei einem Wert von false ändert sich die Position des Readers nicht.
    /// </returns>
    /// <param name="name">Der lokale Name des Attributs.</param><param name="ns">Der Namespace-URI dieses Attributs.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentNullException">Beide Parameter-Werte sind null.</exception>
    [__DynamicallyInvokable]
    public abstract bool MoveToAttribute(string name, string ns);

    /// <summary>
    /// Wechselt beim Überschreiben in einer abgeleiteten Klasse zum Attribut mit dem angegebenen Index.
    /// </summary>
    /// <param name="i">Der Index des Attributs.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentOutOfRangeException">Der Parameter hat einen negativen Wert.</exception>
    [__DynamicallyInvokable]
    public virtual void MoveToAttribute(int i)
    {
      if (i < 0 || i >= this.AttributeCount)
        throw new ArgumentOutOfRangeException("i");
      this.MoveToElement();
      this.MoveToFirstAttribute();
      for (int index = 0; index < i; ++index)
        this.MoveToNextAttribute();
    }

    /// <summary>
    /// Wechselt beim Überschreiben in einer abgeleiteten Klasse zum ersten Attribut.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein Attribut vorhanden ist (der Reader wechselt zum ersten Attribut), andernfalls false (die Position des Readers bleibt unverändert).
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract bool MoveToFirstAttribute();

    /// <summary>
    /// Wechselt beim Überschreiben in einer abgeleiteten Klasse zum nächsten Attribut.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein nächstes Attribut vorhanden ist; false, wenn keine weiteren Attribute vorhanden sind.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract bool MoveToNextAttribute();

    /// <summary>
    /// Wechselt beim Überschreiben in einer abgeleiteten Klasse zu dem Element, das den aktuellen Attributknoten enthält.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der Reader auf einem Attribut positioniert ist (der Reader wechselt zu dem Element, das das Attribut besitzt); false, wenn der Reader nicht auf einem Attribut positioniert ist (die Position des Readers bleibt unverändert).
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract bool MoveToElement();

    /// <summary>
    /// Löst beim Überschreiben in einer abgeleiteten Klasse den Attributwert in einen oder mehrere Knoten vom Typ Text, EntityReference oder EndEntity auf.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn zurückzugebende Knoten vorhanden sind.false, wenn der Reader beim ersten Aufruf nicht auf einem Attributknoten positioniert ist oder alle Attributwerte gelesen wurden.Ein leeres Attribut, z. B. misc="", gibt true mit einem einzelnen Knoten mit dem Wert String.Empty zurück.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract bool ReadAttributeValue();

    /// <summary>
    /// Liest beim Überschreiben in einer abgeleiteten Klasse den nächsten Knoten aus dem Stream.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der nächste Knoten erfolgreich gelesen wurde, andernfalls, false.
    /// </returns>
    /// <exception cref="T:System.Xml.XmlException">Beim Analysieren der XML-Daten ist ein Fehler aufgetreten.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract bool Read();

    /// <summary>
    /// Ändert beim Überschreiben in einer abgeleiteten Klassen den <see cref="P:System.Xml.XmlReader.ReadState"/> in <see cref="F:System.Xml.ReadState.Closed"/>.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    public virtual void Close()
    {
    }

    /// <summary>
    /// Überspringt die untergeordneten Elemente des aktuellen Knotens.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual void Skip()
    {
      if (this.ReadState != ReadState.Interactive)
        return;
      this.SkipSubtree();
    }

    /// <summary>
    /// Löst beim Überschreiben in einer abgeleiteten Klasse ein Namespacepräfix im Gültigkeitsbereich des aktuellen Elements auf.
    /// </summary>
    /// 
    /// <returns>
    /// Der Namespace-URI, dem das Präfix zugeordnet ist, oder null, wenn kein entsprechendes Präfix gefunden wird.
    /// </returns>
    /// <param name="prefix">Das Präfix, dessen Namespace-URI aufgelöst werden soll.Um eine Übereinstimmung mit dem Standardnamespace zu erhalten, übergeben Sie eine leere Zeichenfolge.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract string LookupNamespace(string prefix);

    /// <summary>
    /// Löst beim Überschreiben in einer abgeleiteten Klasse den Entitätsverweis für EntityReference-Knoten auf.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Der Reader ist nicht auf einem EntityReference-Knoten positioniert. Diese Implementierung des Readers kann Entitäten nicht auflösen (<see cref="P:System.Xml.XmlReader.CanResolveEntity"/> gibt false zurück).</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public abstract void ResolveEntity();

    /// <summary>
    /// Liest den Inhalt und gibt die Base64-decodierten binären Bytes zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="buffer"/>-Wert ist null.</exception><exception cref="T:System.InvalidOperationException"><see cref="M:System.Xml.XmlReader.ReadContentAsBase64(System.Byte[],System.Int32,System.Int32)"/> wird auf dem aktuellen Knoten nicht unterstützt.</exception><exception cref="T:System.ArgumentOutOfRangeException">Der Index im Puffer oder Index + Anzahl übersteigen die Größe des zugeordneten Puffers.</exception><exception cref="T:System.NotSupportedException">Die <see cref="T:System.Xml.XmlReader"/>-Implementierung unterstützt diese Methode nicht.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadContentAsBase64(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadContentAsBase64"
      }));
    }

    /// <summary>
    /// Liest das Element und decodiert den Base64-Inhalt.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="buffer"/>-Wert ist null.</exception><exception cref="T:System.InvalidOperationException">Der aktuelle Knoten ist kein Elementknoten.</exception><exception cref="T:System.ArgumentOutOfRangeException">Der Index im Puffer oder Index + Anzahl übersteigen die Größe des zugeordneten Puffers.</exception><exception cref="T:System.NotSupportedException">Die <see cref="T:System.Xml.XmlReader"/>-Implementierung unterstützt diese Methode nicht.</exception><exception cref="T:System.Xml.XmlException">Das Element enthält gemischten Inhalt.</exception><exception cref="T:System.FormatException">Der Inhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadElementContentAsBase64(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadElementContentAsBase64"
      }));
    }

    /// <summary>
    /// Liest den Inhalt und gibt die BinHex-decodierten binären Bytes zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="buffer"/>-Wert ist null.</exception><exception cref="T:System.InvalidOperationException"><see cref="M:System.Xml.XmlReader.ReadContentAsBinHex(System.Byte[],System.Int32,System.Int32)"/> wird auf dem aktuellen Knoten nicht unterstützt.</exception><exception cref="T:System.ArgumentOutOfRangeException">Der Index im Puffer oder Index + Anzahl übersteigen die Größe des zugeordneten Puffers.</exception><exception cref="T:System.NotSupportedException">Die <see cref="T:System.Xml.XmlReader"/>-Implementierung unterstützt diese Methode nicht.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadContentAsBinHex(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadContentAsBinHex"
      }));
    }

    /// <summary>
    /// Liest das Element und decodiert den BinHex-Inhalt.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="buffer"/>-Wert ist null.</exception><exception cref="T:System.InvalidOperationException">Der aktuelle Knoten ist kein Elementknoten.</exception><exception cref="T:System.ArgumentOutOfRangeException">Der Index im Puffer oder Index + Anzahl übersteigen die Größe des zugeordneten Puffers.</exception><exception cref="T:System.NotSupportedException">Die <see cref="T:System.Xml.XmlReader"/>-Implementierung unterstützt diese Methode nicht.</exception><exception cref="T:System.Xml.XmlException">Das Element enthält gemischten Inhalt.</exception><exception cref="T:System.FormatException">Der Inhalt kann nicht in den angeforderten Typ konvertiert werden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadElementContentAsBinHex"
      }));
    }

    /// <summary>
    /// Liest umfangreiche Streams von Text, der in ein XML-Dokument eingebettet ist.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer gelesenen Zeichen.Der Wert 0 (null) wird zurückgegeben, wenn kein weiterer Textinhalt vorhanden ist.
    /// </returns>
    /// <param name="buffer">Das Array von Zeichen, das als Puffer dient, in den der Textinhalt geschrieben wird.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, ab dem der <see cref="T:System.Xml.XmlReader"/> die Ergebnisse kopieren kann.</param><param name="count">Die maximale Anzahl von Zeichen, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl der kopierten Zeichen zurück.</param><exception cref="T:System.InvalidOperationException">Der aktuelle Knoten verfügt über keinen Wert (<see cref="P:System.Xml.XmlReader.HasValue"/> ist false).</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="buffer"/>-Wert ist null.</exception><exception cref="T:System.ArgumentOutOfRangeException">Der Index im Puffer oder Index + Anzahl übersteigen die Größe des zugeordneten Puffers.</exception><exception cref="T:System.NotSupportedException">Die <see cref="T:System.Xml.XmlReader"/>-Implementierung unterstützt diese Methode nicht.</exception><exception cref="T:System.Xml.XmlException">Die XML-Daten sind nicht wohlgeformt.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual int ReadValueChunk(char[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadValueChunkNotSupported"));
    }

    /// <summary>
    /// Liest beim Überschreiben in einer abgeleiteten Klasse den Inhalt eines Element- oder Textknotens als Zeichenfolge.Sie sollten stattdessen allerdings die <see cref="Overload:System.Xml.XmlReader.ReadElementContentAsString"/>-Methode verwenden, da sie eine einfachere Möglichkeit zum Verarbeiten dieses Vorgangs bereitstellt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Inhalt des Elements oder eine leere Zeichenfolge.
    /// </returns>
    /// <exception cref="T:System.Xml.XmlException">Beim Analysieren der XML-Daten ist ein Fehler aufgetreten.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual string ReadString()
    {
      if (this.ReadState != ReadState.Interactive)
        return string.Empty;
      this.MoveToElement();
      if (this.NodeType == XmlNodeType.Element)
      {
        if (this.IsEmptyElement)
          return string.Empty;
        if (!this.Read())
          throw new InvalidOperationException(Res.GetString("Xml_InvalidOperation"));
        if (this.NodeType == XmlNodeType.EndElement)
          return string.Empty;
      }
      string str = string.Empty;
      while (Xml_Reader.IsTextualNode(this.NodeType))
      {
        str += this.Value;
        if (!this.Read())
          break;
      }
      return str;
    }

    /// <summary>
    /// Überprüft, ob der aktuelle Knoten ein Inhaltsknoten (Textknoten ohne Leerraum, CDATA-, Element-, EndElement-, EntityReference- oder EndEntity-Knoten) ist.Wenn der Knoten kein Inhaltsknoten ist, springt der Reader zum nächsten Inhaltsknoten oder an das Ende der Datei.Knoten folgender Typen werden übersprungen: ProcessingInstruction, DocumentType, Comment, Whitespace und SignificantWhitespace.
    /// </summary>
    /// 
    /// <returns>
    /// Der <see cref="P:System.Xml.XmlReader.NodeType"/> des von der Methode gefundenen aktuellen Knotens oder XmlNodeType.None, wenn der Reader das Ende des Eingabestreams erreicht hat.
    /// </returns>
    /// <exception cref="T:System.Xml.XmlException">Im Eingabestream wurde unzulässiger XML-Code gefunden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual XmlNodeType MoveToContent()
    {
      do
      {
        switch (this.NodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.EntityReference:
          case XmlNodeType.EndElement:
          case XmlNodeType.EndEntity:
          return this.NodeType;
          case XmlNodeType.Attribute:
          this.MoveToElement();
          goto case XmlNodeType.Element;
          default:
          continue;
        }
      }
      while (this.Read());
      return this.NodeType;
    }

    /// <summary>
    /// Überprüft, ob der aktuelle Knoten ein Element ist, und rückt den Reader zum nächsten Knoten vor.
    /// </summary>
    /// <exception cref="T:System.Xml.XmlException">Im Eingabestream wurde unzulässiger XML-Code gefunden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual void ReadStartElement()
    {
      if (this.MoveToContent() != XmlNodeType.Element)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      this.Read();
    }

    /// <summary>
    /// Überprüft, ob der aktuelle Inhaltsknoten ein Element mit dem angegebenen <see cref="P:System.Xml.XmlReader.Name"/> ist, und verschiebt den Reader auf den nächsten Knoten.
    /// </summary>
    /// <param name="name">Der qualifizierte Name des Elements.</param><exception cref="T:System.Xml.XmlException">Im Eingabestream wurde unzulässiger XML-Code gefunden. - oder -  Der <see cref="P:System.Xml.XmlReader.Name"/> des Elements entspricht nicht dem angegebenen <paramref name="name"/>.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual void ReadStartElement(string name)
    {
      if (this.MoveToContent() != XmlNodeType.Element)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      if (!(this.Name == name))
        throw new XmlException("Xml_ElementNotFound", name, this as IXmlLineInfo);
      this.Read();
    }

    /// <summary>
    /// Überprüft, ob der aktuelle Inhaltsknoten ein Element mit dem angegebenen <see cref="P:System.Xml.XmlReader.LocalName"/> und dem angegebenen <see cref="P:System.Xml.XmlReader.NamespaceURI"/> ist, und verschiebt den Reader auf den nächsten Knoten.
    /// </summary>
    /// <param name="localname">Der lokale Name des Elements.</param><param name="ns">Der Namespace-URI des Elements.</param><exception cref="T:System.Xml.XmlException">Im Eingabestream wurde unzulässiger XML-Code gefunden.- oder - Die Eigenschaften <see cref="P:System.Xml.XmlReader.LocalName"/> und <see cref="P:System.Xml.XmlReader.NamespaceURI"/> des gefundenen Elements stimmen nicht mit den angegebenen Argumenten überein.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual void ReadStartElement(string localname, string ns)
    {
      if (this.MoveToContent() != XmlNodeType.Element)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      if (this.LocalName == localname && this.NamespaceURI == ns)
        this.Read();
      else
        throw new XmlException("Xml_ElementNotFoundNs", new string[2]
        {
          localname,
          ns
        }, this as IXmlLineInfo);
    }

    /// <summary>
    /// Liest ein Nur-Text-Element.Sie sollten stattdessen allerdings die <see cref="M:System.Xml.XmlReader.ReadElementContentAsString"/>-Methode verwenden, da sie eine einfachere Möglichkeit zum Verarbeiten dieses Vorgangs bereitstellt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Text in dem gelesenen Element.Eine leere Zeichenfolge, wenn das Element leer ist.
    /// </returns>
    /// <exception cref="T:System.Xml.XmlException">Der nächste Inhaltsknoten ist kein Starttag, oder das gefundene Element enthält keinen Wert für einfachen Text.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual string ReadElementString()
    {
      string str = string.Empty;
      if (this.MoveToContent() != XmlNodeType.Element)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      if (!this.IsEmptyElement)
      {
        this.Read();
        str = this.ReadString();
        if (this.NodeType != XmlNodeType.EndElement)
          throw new XmlException("Xml_UnexpectedNodeInSimpleContent", new string[2]
          {
            this.NodeType.ToString(),
            "ReadElementString"
          }, this as IXmlLineInfo);
        this.Read();
      }
      else
        this.Read();
      return str;
    }

    /// <summary>
    /// Überprüft vor dem Lesen eines Nur-Text-Elements, ob die <see cref="P:System.Xml.XmlReader.Name"/>-Eigenschaft des gefundenen Elements mit der angegebenen Zeichenfolge übereinstimmt.Sie sollten stattdessen allerdings die <see cref="M:System.Xml.XmlReader.ReadElementContentAsString"/>-Methode verwenden, da sie eine einfachere Möglichkeit zum Verarbeiten dieses Vorgangs bereitstellt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Text in dem gelesenen Element.Eine leere Zeichenfolge, wenn das Element leer ist.
    /// </returns>
    /// <param name="name">Der zu überprüfende Name.</param><exception cref="T:System.Xml.XmlException">Wenn der nächste Inhaltsknoten kein Starttag ist, wenn der Element Name mit dem angegebenen Argument nicht übereinstimmt oder wenn das gefundene Element keinen Wert für einfachen Text enthält.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual string ReadElementString(string name)
    {
      string str = string.Empty;
      if (this.MoveToContent() != XmlNodeType.Element)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      if (this.Name != name)
        throw new XmlException("Xml_ElementNotFound", name, this as IXmlLineInfo);
      if (!this.IsEmptyElement)
      {
        str = this.ReadString();
        if (this.NodeType != XmlNodeType.EndElement)
          throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
        this.Read();
      }
      else
        this.Read();
      return str;
    }

    /// <summary>
    /// Überprüft vor dem Lesen eines Nur-Text-Elements, ob die <see cref="P:System.Xml.XmlReader.LocalName"/>- und die <see cref="P:System.Xml.XmlReader.NamespaceURI"/>-Eigenschaft des gefundenen Elements mit den angegebenen Zeichenfolgen übereinstimmen.Sie sollten stattdessen allerdings die <see cref="M:System.Xml.XmlReader.ReadElementContentAsString(System.String,System.String)"/>-Methode verwenden, da sie eine einfachere Möglichkeit zum Verarbeiten dieses Vorgangs bereitstellt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Text in dem gelesenen Element.Eine leere Zeichenfolge, wenn das Element leer ist.
    /// </returns>
    /// <param name="localname">Der zu überprüfende lokale Name.</param><param name="ns">Der zu überprüfende Namespace-URI.</param><exception cref="T:System.Xml.XmlException">Wenn der nächste Inhaltsknoten kein Starttag ist, wenn der LocalName oder NamespaceURI des Elements nicht mit den angegebenen Argumenten übereinstimmen oder wenn das gefundene Element keinen Wert für einfachen Text enthält.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual string ReadElementString(string localname, string ns)
    {
      string str = string.Empty;
      if (this.MoveToContent() != XmlNodeType.Element)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      if (this.LocalName != localname || this.NamespaceURI != ns)
        throw new XmlException("Xml_ElementNotFoundNs", new string[2]
        {
          localname,
          ns
        }, this as IXmlLineInfo);
      if (!this.IsEmptyElement)
      {
        str = this.ReadString();
        if (this.NodeType != XmlNodeType.EndElement)
          throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
        this.Read();
      }
      else
        this.Read();
      return str;
    }

    /// <summary>
    /// Überprüft, ob der aktuelle Inhaltsknoten ein Endtag ist, und verschiebt den Reader auf den nächsten Knoten.
    /// </summary>
    /// <exception cref="T:System.Xml.XmlException">Der aktuelle Knoten ist kein Endtag, oder im Eingabestream wurde unzulässiger XML-Code gefunden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual void ReadEndElement()
    {
      if (this.MoveToContent() != XmlNodeType.EndElement)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      this.Read();
    }

    /// <summary>
    /// Ruft <see cref="M:System.Xml.XmlReader.MoveToContent"/> auf und überprüft, ob der aktuelle Inhaltsknoten ein Starttag oder ein leeres Elementtag ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn <see cref="M:System.Xml.XmlReader.MoveToContent"/> ein Starttag oder ein leeres Elementtag findet. false, wenn ein anderer Knotentyp als XmlNodeType.Element gefunden wurde.
    /// </returns>
    /// <exception cref="T:System.Xml.XmlException">Im Eingabestream wurde unzulässiger XML-Code gefunden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool IsStartElement()
    {
      return this.MoveToContent() == XmlNodeType.Element;
    }

    /// <summary>
    /// Ruft <see cref="M:System.Xml.XmlReader.MoveToContent"/> auf und überprüft, ob der aktuelle Inhaltsknoten ein Starttag oder ein leeres Elementtag ist und die <see cref="P:System.Xml.XmlReader.Name"/>-Eigenschaft des gefundenen Elements mit dem angegebenen Argument übereinstimmt.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der resultierende Knoten ein Element ist und die Name-Eigenschaft mit der angegebenen Zeichenfolge übereinstimmt.false, wenn ein anderer Knotentyp als XmlNodeType.Element gefunden wurde oder die Name-Elementeigenschaft nicht mit der angegebenen Zeichenfolge übereinstimmt.
    /// </returns>
    /// <param name="name">Die mit der Name-Eigenschaft des gefundenen Elements verglichene Zeichenfolge.</param><exception cref="T:System.Xml.XmlException">Im Eingabestream wurde unzulässiger XML-Code gefunden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool IsStartElement(string name)
    {
      if (this.MoveToContent() == XmlNodeType.Element)
        return this.Name == name;
      return false;
    }

    /// <summary>
    /// Ruft <see cref="M:System.Xml.XmlReader.MoveToContent"/> auf und überprüft, ob der aktuelle Inhaltsknoten ein Starttag oder ein leeres Elementtag ist und ob die <see cref="P:System.Xml.XmlReader.LocalName"/>-Eigenschaft und die <see cref="P:System.Xml.XmlReader.NamespaceURI"/>-Eigenschaft des gefundenen Elements mit den angegebenen Zeichenfolgen übereinstimmen.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der resultlierende Knoten ein Element ist.false, wenn ein anderer Knotentyp als XmlNodeType.Element gefunden wurde oder die LocalName-Eigenschaft und die NamespaceURI-Eigenschaft des Elements nicht mit den angegebenen Zeichenfolgen übereinstimmen.
    /// </returns>
    /// <param name="localname">Die mit der LocalName-Eigenschaft des gefundenen Elements zu vergleichende Zeichenfolge.</param><param name="ns">Die mit der NamespaceURI-Eigenschaft des gefundenen Elements zu vergleichende Zeichenfolge.</param><exception cref="T:System.Xml.XmlException">Im Eingabestream wurde unzulässiger XML-Code gefunden.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual bool IsStartElement(string localname, string ns)
    {
      if (this.MoveToContent() == XmlNodeType.Element && this.LocalName == localname)
        return this.NamespaceURI == ns;
      return false;
    }

    /// <summary>
    /// Liest, bis ein Element mit dem angegebenen qualifizierten Namen gefunden wird.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein übereinstimmendes Element gefunden wird, andernfalls false, und der <see cref="T:System.Xml.XmlReader"/> in einem Dateiendezustand.
    /// </returns>
    /// <param name="name">Der qualifizierte Name des Elements.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentException">Der Parameter ist eine leere Zeichenfolge.</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadToFollowing(string name)
    {
      if (name == null || name.Length == 0)
        throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
      name = this.NameTable.Add(name);
      while (this.Read())
      {
        if (this.NodeType == XmlNodeType.Element && Ref.Equal(name, this.Name))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Liest, bis ein Element mit dem angegebenen lokalen Namen und dem angegebenen Namespace-URI gefunden wird.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein übereinstimmendes Element gefunden wird, andernfalls false, und der <see cref="T:System.Xml.XmlReader"/> in einem Dateiendezustand.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements.</param><param name="namespaceURI">Der Namespace-URI des Elements.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentNullException">Beide Parameter-Werte sind null.</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadToFollowing(string localName, string namespaceURI)
    {
      if (localName == null || localName.Length == 0)
        throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
      if (namespaceURI == null)
        throw new ArgumentNullException("namespaceURI");
      localName = this.NameTable.Add(localName);
      namespaceURI = this.NameTable.Add(namespaceURI);
      while (this.Read())
      {
        if (this.NodeType == XmlNodeType.Element && Ref.Equal(localName, this.LocalName) && Ref.Equal(namespaceURI, this.NamespaceURI))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Verschiebt den <see cref="T:System.Xml.XmlReader"/> auf das nächste Nachfolgerelement mit dem angegebenen qualifizierten Namen.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein übereinstimmendes Nachfolgerelement gefunden wurde, andernfalls false.Wenn kein übereinstimmendes untergeordnetes Element gefunden wurde, wird der <see cref="T:System.Xml.XmlReader"/> auf dem Endtag (<see cref="P:System.Xml.XmlReader.NodeType"/> ist XmlNodeType.EndElement) des Elements positioniert.Wenn der <see cref="T:System.Xml.XmlReader"/> beim Aufruf von <see cref="M:System.Xml.XmlReader.ReadToDescendant(System.String)"/> nicht in einem Element positioniert wird, gibt diese Methode false zurück, und die Position des <see cref="T:System.Xml.XmlReader"/> wird nicht geändert.
    /// </returns>
    /// <param name="name">Der qualifizierte Name des Elements, zu dem Sie wechseln möchten.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentException">Der Parameter ist eine leere Zeichenfolge.</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadToDescendant(string name)
    {
      if (name == null || name.Length == 0)
        throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
      int depth = this.Depth;
      if (this.NodeType != XmlNodeType.Element)
      {
        if (this.ReadState != ReadState.Initial)
          return false;
        --depth;
      }
      else if (this.IsEmptyElement)
        return false;
      name = this.NameTable.Add(name);
      while (this.Read() && this.Depth > depth)
      {
        if (this.NodeType == XmlNodeType.Element && Ref.Equal(name, this.Name))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Verschiebt den <see cref="T:System.Xml.XmlReader"/> auf das nächste Nachfolgerelement mit dem angegebenen lokalen Namen und dem angegebenen Namespace-URI.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein übereinstimmendes Nachfolgerelement gefunden wurde, andernfalls false.Wenn kein übereinstimmendes untergeordnetes Element gefunden wurde, wird der <see cref="T:System.Xml.XmlReader"/> auf dem Endtag (<see cref="P:System.Xml.XmlReader.NodeType"/> ist XmlNodeType.EndElement) des Elements positioniert.Wenn der <see cref="T:System.Xml.XmlReader"/> beim Aufruf von <see cref="M:System.Xml.XmlReader.ReadToDescendant(System.String,System.String)"/> nicht in einem Element positioniert wird, gibt diese Methode false zurück, und die Position des <see cref="T:System.Xml.XmlReader"/> wird nicht geändert.
    /// </returns>
    /// <param name="localName">Der lokale Name des Elements, zu dem Sie wechseln möchten.</param><param name="namespaceURI">Der Namespace-URI des Elements, zu dem Sie wechseln möchten.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentNullException">Beide Parameter-Werte sind null.</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadToDescendant(string localName, string namespaceURI)
    {
      if (localName == null || localName.Length == 0)
        throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
      if (namespaceURI == null)
        throw new ArgumentNullException("namespaceURI");
      int depth = this.Depth;
      if (this.NodeType != XmlNodeType.Element)
      {
        if (this.ReadState != ReadState.Initial)
          return false;
        --depth;
      }
      else if (this.IsEmptyElement)
        return false;
      localName = this.NameTable.Add(localName);
      namespaceURI = this.NameTable.Add(namespaceURI);
      while (this.Read() && this.Depth > depth)
      {
        if (this.NodeType == XmlNodeType.Element && Ref.Equal(localName, this.LocalName) && Ref.Equal(namespaceURI, this.NamespaceURI))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Verschiebt den XmlReader auf das nächste nebengeordnete Element mit dem angegebenen qualifizierten Namen.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein übereinstimmendes nebengeordnetes Element gefunden wurde, andernfalls false.Wenn kein übereinstimmendes nebengeordnetes Element gefunden wurde, wird der XmlReader auf dem Endtag (<see cref="P:System.Xml.XmlReader.NodeType"/> ist XmlNodeType.EndElement) des übergeordneten Elements positioniert.
    /// </returns>
    /// <param name="name">Der qualifizierte Name des nebengeordneten Elements, zu dem Sie wechseln möchten.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentException">Der Parameter ist eine leere Zeichenfolge.</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadToNextSibling(string name)
    {
      if (name == null || name.Length == 0)
        throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
      name = this.NameTable.Add(name);
      while (this.SkipSubtree())
      {
        XmlNodeType nodeType = this.NodeType;
        if (nodeType == XmlNodeType.Element && Ref.Equal(name, this.Name))
          return true;
        if (nodeType == XmlNodeType.EndElement || this.EOF)
          break;
      }
      return false;
    }

    /// <summary>
    /// Verschiebt den XmlReader auf das nächste nebengeordnete Element mit dem angegebenen lokalen Namen und dem angegebenen Namespace-URI.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn ein übereinstimmendes nebengeordnetes Element gefunden wurde, andernfalls false.Wenn kein übereinstimmendes nebengeordnetes Element gefunden wurde, wird der XmlReader auf dem Endtag (<see cref="P:System.Xml.XmlReader.NodeType"/> ist XmlNodeType.EndElement) des übergeordneten Elements positioniert.
    /// </returns>
    /// <param name="localName">Der lokale Name des nebengeordneten Elements, zu dem Sie wechseln möchten.</param><param name="namespaceURI">Der Namespace-URI des nebengeordneten Elements, zu dem Sie wechseln möchten.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.ArgumentNullException">Beide Parameter-Werte sind null.</exception>
    [__DynamicallyInvokable]
    public virtual bool ReadToNextSibling(string localName, string namespaceURI)
    {
      if (localName == null || localName.Length == 0)
        throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
      if (namespaceURI == null)
        throw new ArgumentNullException("namespaceURI");
      localName = this.NameTable.Add(localName);
      namespaceURI = this.NameTable.Add(namespaceURI);
      while (this.SkipSubtree())
      {
        XmlNodeType nodeType = this.NodeType;
        if (nodeType == XmlNodeType.Element && Ref.Equal(localName, this.LocalName) && Ref.Equal(namespaceURI, this.NamespaceURI))
          return true;
        if (nodeType == XmlNodeType.EndElement || this.EOF)
          break;
      }
      return false;
    }

    /// <summary>
    /// Gibt einen Wert zurück, der angibt, ob das Zeichenfolgenargument ein gültiger XML-Name ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der Name gültig ist, andernfalls false.
    /// </returns>
    /// <param name="str">Der Name, dessen Gültigkeit validiert werden soll.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="str"/>-Wert ist null.</exception>
    [__DynamicallyInvokable]
    public static bool IsName(string str)
    {
      if (str == null)
        throw new NullReferenceException();
      return ValidateNames.IsNameNoNamespaces(str);
    }

    /// <summary>
    /// Gibt einen Wert zurück, der angibt, ob das Zeichenfolgenargument ein gültiges XML-Namenstoken ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn es sich um ein gültiges Namenstoken handelt, andernfalls false.
    /// </returns>
    /// <param name="str">Das zu validierende Namenstoken.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="str"/>-Wert ist null.</exception>
    [__DynamicallyInvokable]
    public static bool IsNameToken(string str)
    {
      if (str == null)
        throw new NullReferenceException();
      return ValidateNames.IsNmtokenNoNamespaces(str);
    }

    /// <summary>
    /// Liest beim Überschreiben in einer abgeleiteten Klasse den gesamten Inhalt, einschließlich Markup, als Zeichenfolge.
    /// </summary>
    /// 
    /// <returns>
    /// Der gesamte XML-Inhalt (einschließlich Markup) im aktuellen Knoten.Wenn der aktuelle Knoten keine untergeordneten Elemente besitzt, wird eine leere Zeichenfolge zurückgegeben.Wenn der aktuelle Knoten weder ein Element noch ein Attribut ist, wird eine leere Zeichenfolge zurückgegeben.
    /// </returns>
    /// <exception cref="T:System.Xml.XmlException">Das XML war nicht wohlgeformt, oder bei der XML-Analyse ist ein Fehler aufgetreten.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string ReadInnerXml()
    {
      if (this.ReadState != ReadState.Interactive)
        return string.Empty;
      if (this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Element)
      {
        this.Read();
        return string.Empty;
      }
      StringWriter sw = new StringWriter((IFormatProvider)CultureInfo.InvariantCulture);
      XmlWriter forInnerOuterXml = this.CreateWriterForInnerOuterXml(sw);
      try
      {
        if (this.NodeType == XmlNodeType.Attribute)
        {
          ((XmlTextWriter)forInnerOuterXml).QuoteChar = this.QuoteChar;
          this.WriteAttributeValue(forInnerOuterXml);
        }
        if (this.NodeType == XmlNodeType.Element)
          this.WriteNode(forInnerOuterXml, false);
      }
      finally
      {
        forInnerOuterXml.Close();
      }
      return sw.ToString();
    }

    private void WriteNode(XmlWriter xtw, bool defattr)
    {
      int num = this.NodeType == XmlNodeType.None ? -1 : this.Depth;
      while (this.Read() && num < this.Depth)
      {
        switch (this.NodeType)
        {
          case XmlNodeType.Element:
          xtw.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
          ((XmlTextWriter)xtw).QuoteChar = this.QuoteChar;
          xtw.WriteAttributes(this, defattr);
          if (this.IsEmptyElement)
          {
            xtw.WriteEndElement();
            continue;
          }
          continue;
          case XmlNodeType.Text:
          xtw.WriteString(this.Value);
          continue;
          case XmlNodeType.CDATA:
          xtw.WriteCData(this.Value);
          continue;
          case XmlNodeType.EntityReference:
          xtw.WriteEntityRef(this.Name);
          continue;
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.XmlDeclaration:
          xtw.WriteProcessingInstruction(this.Name, this.Value);
          continue;
          case XmlNodeType.Comment:
          xtw.WriteComment(this.Value);
          continue;
          case XmlNodeType.DocumentType:
          xtw.WriteDocType(this.Name, this.GetAttribute("PUBLIC"), this.GetAttribute("SYSTEM"), this.Value);
          continue;
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          xtw.WriteWhitespace(this.Value);
          continue;
          case XmlNodeType.EndElement:
          xtw.WriteFullEndElement();
          continue;
          default:
          continue;
        }
      }
      if (num != this.Depth || this.NodeType != XmlNodeType.EndElement)
        return;
      this.Read();
    }

    private void WriteAttributeValue(XmlWriter xtw)
    {
      string name = this.Name;
      while (this.ReadAttributeValue())
      {
        if (this.NodeType == XmlNodeType.EntityReference)
          xtw.WriteEntityRef(this.Name);
        else
          xtw.WriteString(this.Value);
      }
      this.MoveToAttribute(name);
    }

    /// <summary>
    /// Liest beim Überschreiben in einer abgeleiteten Klasse den Inhalt (einschließlich Markup) ab, der diesen Knoten und alle untergeordneten Elemente darstellt.
    /// </summary>
    /// 
    /// <returns>
    /// Wenn der Reader auf einem Elementknoten oder einem Attributknoten positioniert ist, gibt diese Methode den gesamten XML-Inhalt (einschließlich Markup) des aktuellen Knotens sowie aller untergeordneten Elemente zurück. Andernfalls wird eine leere Zeichenfolge zurückgegeben.
    /// </returns>
    /// <exception cref="T:System.Xml.XmlException">Das XML war nicht wohlgeformt, oder bei der XML-Analyse ist ein Fehler aufgetreten.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual string ReadOuterXml()
    {
      if (this.ReadState != ReadState.Interactive)
        return string.Empty;
      if (this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Element)
      {
        this.Read();
        return string.Empty;
      }
      StringWriter sw = new StringWriter((IFormatProvider)CultureInfo.InvariantCulture);
      XmlWriter forInnerOuterXml = this.CreateWriterForInnerOuterXml(sw);
      try
      {
        if (this.NodeType == XmlNodeType.Attribute)
        {
          forInnerOuterXml.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
          this.WriteAttributeValue(forInnerOuterXml);
          forInnerOuterXml.WriteEndAttribute();
        }
        else
          forInnerOuterXml.WriteNode(this, false);
      }
      finally
      {
        forInnerOuterXml.Close();
      }
      return sw.ToString();
    }

    private XmlWriter CreateWriterForInnerOuterXml(StringWriter sw)
    {
      XmlTextWriter xtw = new XmlTextWriter((TextWriter)sw);
      this.SetNamespacesFlag(xtw);
      return (XmlWriter)xtw;
    }

    private void SetNamespacesFlag(XmlTextWriter xtw)
    {
      XmlTextReader xmlTextReader = this as XmlTextReader;
      if (xmlTextReader != null)
      {
        xtw.Namespaces = xmlTextReader.Namespaces;
      }
      else
      {
        XmlValidatingReader validatingReader = this as XmlValidatingReader;
        if (validatingReader == null)
          return;
        xtw.Namespaces = validatingReader.Namespaces;
      }
    }

    /// <summary>
    /// Gibt eine neue XmlReader-Instanz zurück, die zum Lesen des aktuellen Knotens und aller Nachfolgerknoten verwendet werden kann.
    /// </summary>
    /// 
    /// <returns>
    /// Eine neue auf <see cref="F:System.Xml.ReadState.Initial"/> festgelegte XML-Reader-Instanz.Durch den Aufruf der <see cref="M:System.Xml.XmlReader.Read"/>-Methode wird der neue Reader auf dem Knoten positioniert, der vor dem Aufruf der <see cref="M:System.Xml.XmlReader.ReadSubtree"/>-Methode aktuell war.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Der XML-Reader ist nicht auf einem Element positioniert, wenn diese Methode aufgerufen wird.</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public virtual Xml_Reader ReadSubtree()
    {
      if (this.NodeType != XmlNodeType.Element)
        throw new InvalidOperationException(Res.GetString("Xml_ReadSubtreeNotOnElement"));
      return (Xml_Reader)new XmlSubtreeReader(this);
    }

    /// <summary>
    /// Gibt alle von der aktuellen Instanz der <see cref="T:System.Xml.XmlReader"/>-Klasse verwendeten Ressourcen frei.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    public void Dispose()
    {
      this.Dispose(true);
    }

    /// <summary>
    /// Gibt die von <see cref="T:System.Xml.XmlReader"/> verwendeten nicht verwalteten Ressourcen und optional die verwalteten Ressourcen frei.
    /// </summary>
    /// <param name="disposing">true, um sowohl verwaltete als auch nicht verwaltete Ressourcen freizugeben, false, um ausschließlich nicht verwaltete Ressourcen freizugeben.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception>
    [__DynamicallyInvokable]
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.ReadState == ReadState.Closed)
        return;
      this.Close();
    }

    internal static bool IsTextualNode(XmlNodeType nodeType)
    {
      return ((ulong)Xml_Reader.IsTextualNodeBitmap & (ulong)(1 << (int)(nodeType & (XmlNodeType)31))) > 0UL;
    }

    internal static bool CanReadContentAs(XmlNodeType nodeType)
    {
      return ((ulong)Xml_Reader.CanReadContentAsBitmap & (ulong)(1 << (int)(nodeType & (XmlNodeType)31))) > 0UL;
    }

    internal static bool HasValueInternal(XmlNodeType nodeType)
    {
      return ((ulong)Xml_Reader.HasValueBitmap & (ulong)(1 << (int)(nodeType & (XmlNodeType)31))) > 0UL;
    }

    private bool SkipSubtree()
    {
      this.MoveToElement();
      if (this.NodeType != XmlNodeType.Element || this.IsEmptyElement)
        return this.Read();
      int depth = this.Depth;
      do
        ;
      while (this.Read() && depth < this.Depth);
      if (this.NodeType == XmlNodeType.EndElement)
        return this.Read();
      return false;
    }

    internal void CheckElement(string localName, string namespaceURI)
    {
      if (localName == null || localName.Length == 0)
        throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
      if (namespaceURI == null)
        throw new ArgumentNullException("namespaceURI");
      if (this.NodeType != XmlNodeType.Element)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
      if (this.LocalName != localName || this.NamespaceURI != namespaceURI)
        throw new XmlException("Xml_ElementNotFoundNs", new string[2]
        {
          localName,
          namespaceURI
        }, this as IXmlLineInfo);
    }

    internal Exception CreateReadContentAsException(string methodName)
    {
      return Xml_Reader.CreateReadContentAsException(methodName, this.NodeType, this as IXmlLineInfo);
    }

    internal Exception CreateReadElementContentAsException(string methodName)
    {
      return Xml_Reader.CreateReadElementContentAsException(methodName, this.NodeType, this as IXmlLineInfo);
    }

    internal bool CanReadContentAs()
    {
      return Xml_Reader.CanReadContentAs(this.NodeType);
    }

    internal static Exception CreateReadContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
    {
      return (Exception)new InvalidOperationException(Xml_Reader.AddLineInfo(Res.GetString("Xml_InvalidReadContentAs", (object[])new string[2]
      {
        methodName,
        nodeType.ToString()
      }), lineInfo));
    }

    internal static Exception CreateReadElementContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
    {
      return (Exception)new InvalidOperationException(Xml_Reader.AddLineInfo(Res.GetString("Xml_InvalidReadElementContentAs", (object[])new string[2]
      {
        methodName,
        nodeType.ToString()
      }), lineInfo));
    }

    private static string AddLineInfo(string message, IXmlLineInfo lineInfo)
    {
      if (lineInfo != null)
      {
        string[] strArray = new string[2]
        {
          lineInfo.LineNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          lineInfo.LinePosition.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        };
        message = message + " " + Res.GetString("Xml_ErrorPosition", (object[])strArray);
      }
      return message;
    }

    internal string InternalReadContentAsString()
    {
      string str = string.Empty;
      StringBuilder stringBuilder = (StringBuilder)null;
      do
      {
        switch (this.NodeType)
        {
          case XmlNodeType.Attribute:
          return this.Value;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          if (str.Length == 0)
          {
            str = this.Value;
            goto case XmlNodeType.ProcessingInstruction;
          }
          else
          {
            if (stringBuilder == null)
            {
              stringBuilder = new StringBuilder();
              stringBuilder.Append(str);
            }
            stringBuilder.Append(this.Value);
            goto case XmlNodeType.ProcessingInstruction;
          }
          case XmlNodeType.EntityReference:
          if (this.CanResolveEntity)
          {
            this.ResolveEntity();
            goto case XmlNodeType.ProcessingInstruction;
          }
          else
            goto label_11;
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.Comment:
          case XmlNodeType.EndEntity:
          continue;
          default:
          goto label_11;
        }
      }
      while ((this.AttributeCount != 0 ? (this.ReadAttributeValue() ? 1 : 0) : (this.Read() ? 1 : 0)) != 0);
    label_11:
      if (stringBuilder != null)
        return stringBuilder.ToString();
      return str;
    }

    private bool SetupReadElementContentAsXxx(string methodName)
    {
      if (this.NodeType != XmlNodeType.Element)
        throw this.CreateReadElementContentAsException(methodName);
      int num = this.IsEmptyElement ? 1 : 0;
      this.Read();
      if (num != 0)
        return false;
      switch (this.NodeType)
      {
        case XmlNodeType.EndElement:
        this.Read();
        return false;
        case XmlNodeType.Element:
        throw new XmlException("Xml_MixedReadElementContentAs", string.Empty, this as IXmlLineInfo);
        default:
        return true;
      }
    }

    private void FinishReadElementContentAsXxx()
    {
      if (this.NodeType != XmlNodeType.EndElement)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString());
      this.Read();
    }

    internal static Encoding GetEncoding(Xml_Reader reader)
    {
      XmlTextReaderImpl xmlTextReaderImpl = Xml_Reader.GetXmlTextReaderImpl(reader);
      if (xmlTextReaderImpl == null)
        return (Encoding)null;
      return xmlTextReaderImpl.Encoding;
    }

    internal static ConformanceLevel GetV1ConformanceLevel(Xml_Reader reader)
    {
      XmlTextReaderImpl xmlTextReaderImpl = Xml_Reader.GetXmlTextReaderImpl(reader);
      if (xmlTextReaderImpl == null)
        return ConformanceLevel.Document;
      return xmlTextReaderImpl.V1ComformanceLevel;
    }

    private static XmlTextReaderImpl GetXmlTextReaderImpl(Xml_Reader reader)
    {
      XmlTextReaderImpl xmlTextReaderImpl = reader as XmlTextReaderImpl;
      if (xmlTextReaderImpl != null)
        return xmlTextReaderImpl;
      XmlTextReader xmlTextReader = reader as XmlTextReader;
      if (xmlTextReader != null)
        return xmlTextReader.Impl;
      XmlValidatingReaderImpl validatingReaderImpl = reader as XmlValidatingReaderImpl;
      if (validatingReaderImpl != null)
        return validatingReaderImpl.ReaderImpl;
      XmlValidatingReader validatingReader = reader as XmlValidatingReader;
      if (validatingReader != null)
        return validatingReader.Impl.ReaderImpl;
      return (XmlTextReaderImpl)null;
    }

    /// <summary>
    /// Erstellt eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz mit angegebenem URI.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="inputUri">Der URI der Datei, die die XML-Daten enthält.Mit der <see cref="T:System.Xml.XmlUrlResolver"/>-Klasse wird der Pfad in eine kanonische Datendarstellung konvertiert.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="inputUri"/>-Wert ist null.</exception><exception cref="T:System.Security.SecurityException">Der <see cref="T:System.Xml.XmlReader"/> verfügt nicht über ausreichende Berechtigungen für den Zugriff auf den Speicherort der XML-Daten.</exception><exception cref="T:System.IO.FileNotFoundException">Die durch den URI angegebene Datei ist nicht vorhanden.</exception><exception cref="T:System.UriFormatException">Unter .NET for Windows Store apps oder in der Portable Klassenbibliothek verwenden Sie stattdessen die Basisklassenausnahme <see cref="T:System.FormatException"/>.Das URI-Format ist nicht korrekt.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(string inputUri)
    {
      return Xml_Reader.Create(inputUri, (XmlReaderSettings)null, (XmlParserContext)null);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen URI und den angegebenen Einstellungen eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="inputUri">Der URI der Datei, die die XML-Daten enthält.Das <see cref="T:System.Xml.XmlResolver"/>-Objekt für das <see cref="T:System.Xml.XmlReaderSettings"/>-Objekt wird zum Konvertieren des Pfads in eine kanonische Datendarstellung verwendet.Wenn <see cref="P:System.Xml.XmlReaderSettings.XmlResolver"/>null ist, wird ein neues <see cref="T:System.Xml.XmlUrlResolver"/>-Objekt verwendet.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Dieser Wert kann null sein.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="inputUri"/>-Wert ist null.</exception><exception cref="T:System.IO.FileNotFoundException">Die durch den URI angegebene Datei kann nicht gefunden werden.</exception><exception cref="T:System.UriFormatException">Unter .NET for Windows Store apps oder in der Portable Klassenbibliothek verwenden Sie stattdessen die Basisklassenausnahme <see cref="T:System.FormatException"/>.Das URI-Format ist nicht korrekt.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(string inputUri, XmlReaderSettings settings)
    {
      return Xml_Reader.Create(inputUri, settings, (XmlParserContext)null);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen URI, den Einstellungen und den Kontextinformationen für Analysezwecke eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="inputUri">Der URI der Datei, die die XML-Daten enthält.Das <see cref="T:System.Xml.XmlResolver"/>-Objekt für das <see cref="T:System.Xml.XmlReaderSettings"/>-Objekt wird zum Konvertieren des Pfads in eine kanonische Datendarstellung verwendet.Wenn <see cref="P:System.Xml.XmlReaderSettings.XmlResolver"/>null ist, wird ein neues <see cref="T:System.Xml.XmlUrlResolver"/>-Objekt verwendet.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Dieser Wert kann null sein.</param><param name="inputContext">Die Kontextinformationen, die zum Analysieren des XML-Fragments erforderlich sind.Die Kontextinformationen können die zu verwendende <see cref="T:System.Xml.XmlNameTable"/>, die Codierung, den Namespacebereich, den aktuellen xml:lang-Bereich, den aktuellen xml:space-Bereich, den Basis-URI und die Dokumenttypdefinition enthalten.Dieser Wert kann null sein.</param><exception cref="T:System.ArgumentNullException">Der inputUri-Wert ist null.</exception><exception cref="T:System.Security.SecurityException">Der <see cref="T:System.Xml.XmlReader"/> verfügt nicht über ausreichende Berechtigungen für den Zugriff auf den Speicherort der XML-Daten.</exception><exception cref="T:System.ArgumentException">Die <see cref="P:System.Xml.XmlReaderSettings.NameTable"/> und die <see cref="P:System.Xml.XmlParserContext.NameTable"/>-Eigenschaften enthalten Werte.(Nur eine dieser NameTable-Eigenschaften kann festgelegt und verwendet werden).</exception><exception cref="T:System.IO.FileNotFoundException">Die durch den URI angegebene Datei kann nicht gefunden werden.</exception><exception cref="T:System.UriFormatException">Das URI-Format ist nicht korrekt.</exception>
    public static Xml_Reader Create(string inputUri, XmlReaderSettings settings, XmlParserContext inputContext)
    {
      if (settings == null)
        settings = new XmlReaderSettings();
      return settings.CreateReader(inputUri, inputContext);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Stream mit den Standardeinstellungen eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Stream, der die XML-Daten enthält.Der <see cref="T:System.Xml.XmlReader"/> überprüft die ersten Bytes des Streams und durchsucht sie nach einer Bytereihenfolgemarkierung oder einem anderen Codierungszeichen.Nachdem die Codierung bestimmt wurde, wird sie zum weiteren Lesen des Streams verwendet, und die Eingabe wird weiterhin als Stream von (Unicode-)Zeichen analysiert.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception><exception cref="T:System.Security.SecurityException">Der <see cref="T:System.Xml.XmlReader"/> verfügt nicht über ausreichende Berechtigungen für den Zugriff auf den Speicherort der XML-Daten.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(Stream input)
    {
      return Xml_Reader.Create(input, (XmlReaderSettings)null, string.Empty);
    }

    /// <summary>
    /// Erstellt eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz mit dem angegebenen Stream und den angegebenen Einstellungen.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Stream, der die XML-Daten enthält.Der <see cref="T:System.Xml.XmlReader"/> überprüft die ersten Bytes des Streams und durchsucht sie nach einer Bytereihenfolgemarkierung oder einem anderen Codierungszeichen.Nachdem die Codierung bestimmt wurde, wird sie zum weiteren Lesen des Streams verwendet, und die Eingabe wird weiterhin als Stream von (Unicode-)Zeichen analysiert.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Dieser Wert kann null sein.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(Stream input, XmlReaderSettings settings)
    {
      return Xml_Reader.Create(input, settings, string.Empty);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Stream, dem Basis-URI und den Einstellungen eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Stream, der die XML-Daten enthält. Der <see cref="T:System.Xml.XmlReader"/> überprüft die ersten Bytes des Streams und durchsucht sie nach einer Bytereihenfolgemarkierung oder einem anderen Codierungszeichen.Nachdem die Codierung bestimmt wurde, wird sie zum weiteren Lesen des Streams verwendet, und die Eingabe wird weiterhin als Stream von (Unicode-)Zeichen analysiert.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Dieser Wert kann null sein.</param><param name="baseUri">Der Basis-URI der gelesenen Entität oder des gelesenen Dokuments.Dieser Wert kann null sein.Sicherheitshinweis   Der Basis-URI wird verwendet, um den relativen URI des XML-Dokuments aufzulösen.Verwenden Sie keinen Basis-URI von einer nicht vertrauenswürdigen Quelle.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception>
    public static Xml_Reader Create(Stream input, XmlReaderSettings settings, string baseUri)
    {
      if (settings == null)
        settings = new XmlReaderSettings();
      return settings.CreateReader(input, (Uri)null, baseUri, (XmlParserContext)null);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Stream, den Einstellungen und den Kontextinformationen für Analysezwecke eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Stream, der die XML-Daten enthält. Der <see cref="T:System.Xml.XmlReader"/> überprüft die ersten Bytes des Streams und durchsucht sie nach einer Bytereihenfolgemarkierung oder einem anderen Codierungszeichen.Nachdem die Codierung bestimmt wurde, wird sie zum weiteren Lesen des Streams verwendet, und die Eingabe wird weiterhin als Stream von (Unicode-)Zeichen analysiert.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Dieser Wert kann null sein.</param><param name="inputContext">Die Kontextinformationen, die zum Analysieren des XML-Fragments erforderlich sind.Die Kontextinformationen können die zu verwendende <see cref="T:System.Xml.XmlNameTable"/>, die Codierung, den Namespacebereich, den aktuellen xml:lang-Bereich, den aktuellen xml:space-Bereich, den Basis-URI und die Dokumenttypdefinition enthalten.Dieser Wert kann null sein.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
    {
      if (settings == null)
        settings = new XmlReaderSettings();
      return settings.CreateReader(input, (Uri)null, string.Empty, inputContext);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Text-Reader eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Text-Reader, aus dem die XML-Daten gelesen werden sollen.Ein Text-Reader gibt einen Stream von Unicode-Zeichen zurück, sodass die in der XML-Deklaration angegebene Codierung nicht vom XML-Reader zum Decodieren des Datenstreams verwendet wird.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(TextReader input)
    {
      return Xml_Reader.Create(input, (XmlReaderSettings)null, string.Empty);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Text-Reader und den angegebenen Einstellungen eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Text-Reader, aus dem die XML-Daten gelesen werden sollen.Ein Text-Reader gibt einen Stream von Unicode-Zeichen zurück, sodass die in der XML-Deklaration angegebene Codierung nicht vom XML-Reader zum Decodieren des Datenstreams verwendet wird.</param><param name="settings">Die Einstellungen für den neuen <see cref="T:System.Xml.XmlReader"/>.Dieser Wert kann null sein.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(TextReader input, XmlReaderSettings settings)
    {
      return Xml_Reader.Create(input, settings, string.Empty);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Text-Reader, den angegebenen Einstellungen und dem angegebenen Basis-URI eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Text-Reader, aus dem die XML-Daten gelesen werden sollen.Ein Text-Reader gibt einen Stream von Unicode-Zeichen zurück, sodass die in der XML-Deklaration angegebene Codierung nicht vom <see cref="T:System.Xml.XmlReader"/> zum Decodieren des Datenstreams verwendet wird.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Dieser Wert kann null sein.</param><param name="baseUri">Der Basis-URI der gelesenen Entität oder des gelesenen Dokuments.Dieser Wert kann null sein.Sicherheitshinweis   Der Basis-URI wird verwendet, um den relativen URI des XML-Dokuments aufzulösen.Verwenden Sie keinen Basis-URI von einer nicht vertrauenswürdigen Quelle.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception>
    public static Xml_Reader Create(TextReader input, XmlReaderSettings settings, string baseUri)
    {
      if (settings == null)
        settings = new XmlReaderSettings();
      return settings.CreateReader(input, baseUri, (XmlParserContext)null);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen Text-Reader, den Einstellungen und den Kontextinformationen für Analysezwecke eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, mit dem die im Stream enthaltenen XML-Daten gelesen werden.
    /// </returns>
    /// <param name="input">Der Text-Reader, aus dem die XML-Daten gelesen werden sollen.Ein Text-Reader gibt einen Stream von Unicode-Zeichen zurück, sodass die in der XML-Deklaration angegebene Codierung nicht vom XML-Reader zum Decodieren des Datenstreams verwendet wird.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Dieser Wert kann null sein.</param><param name="inputContext">Die Kontextinformationen, die zum Analysieren des XML-Fragments erforderlich sind.Die Kontextinformationen können die zu verwendende <see cref="T:System.Xml.XmlNameTable"/>, die Codierung, den Namespacebereich, den aktuellen xml:lang-Bereich, den aktuellen xml:space-Bereich, den Basis-URI und die Dokumenttypdefinition enthalten.Dieser Wert kann null sein.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="input"/>-Wert ist null.</exception><exception cref="T:System.ArgumentException">Die <see cref="P:System.Xml.XmlReaderSettings.NameTable"/> und die <see cref="P:System.Xml.XmlParserContext.NameTable"/>-Eigenschaften enthalten Werte.(Nur eine dieser NameTable-Eigenschaften kann festgelegt und verwendet werden).</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(TextReader input, XmlReaderSettings settings, XmlParserContext inputContext)
    {
      if (settings == null)
        settings = new XmlReaderSettings();
      return settings.CreateReader(input, string.Empty, inputContext);
    }

    /// <summary>
    /// Erstellt mit dem angegebenen XML-Reader und den angegebenen Einstellungen eine neue <see cref="T:System.Xml.XmlReader"/>-Instanz.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Objekt, das das angegebene <see cref="T:System.Xml.XmlReader"/>-Objekt umschließt.
    /// </returns>
    /// <param name="reader">Das Objekt, dass Sie als zugrunde liegenden XML-Reader verwenden möchten.</param><param name="settings">Die Einstellungen für die neue <see cref="T:System.Xml.XmlReader"/>-Instanz.Der Konformitätsgrad des <see cref="T:System.Xml.XmlReaderSettings"/>-Objekts muss mit dem Konformitätsgrad des zugrunde liegenden Readers übereinstimmen oder auf <see cref="F:System.Xml.ConformanceLevel.Auto"/> festgelegt werden.</param><exception cref="T:System.ArgumentNullException">Der <paramref name="reader"/>-Wert ist null.</exception><exception cref="T:System.InvalidOperationException">Wenn das <see cref="T:System.Xml.XmlReaderSettings"/>-Objekt einen Konformitätsgrad angibt, der nicht mit dem Konformitätsgrad des zugrunde liegenden Readers übereinstimmt.- oder - Der zugrunde liegende <see cref="T:System.Xml.XmlReader"/> befindet sich in einem <see cref="F:System.Xml.ReadState.Error"/>-Zustand oder einem <see cref="F:System.Xml.ReadState.Closed"/>-Zustand.</exception>
    [__DynamicallyInvokable]
    public static Xml_Reader Create(Xml_Reader reader, XmlReaderSettings settings)
    {
      if (settings == null)
        settings = new XmlReaderSettings();
      return settings.CreateReader(reader);
    }

    internal static Xml_Reader CreateSqlReader(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
    {
      if (input == null)
        throw new ArgumentNullException("input");
      if (settings == null)
        settings = new XmlReaderSettings();
      byte[] numArray = new byte[Xml_Reader.CalcBufferSize(input)];
      int num1 = 0;
      int num2;
      do
      {
        num2 = input.Read(numArray, num1, numArray.Length - num1);
        num1 += num2;
      }
      while (num2 > 0 && num1 < 2);
      Xml_Reader reader;
      if (num1 >= 2 && (int)numArray[0] == 223 && (int)numArray[1] == (int)byte.MaxValue)
      {
        if (inputContext != null)
          throw new ArgumentException(Res.GetString("XmlBinary_NoParserContext"), "inputContext");
        reader = (Xml_Reader)new XmlSqlBinaryReader(input, numArray, num1, string.Empty, settings.CloseInput, settings);
      }
      else
        reader = (Xml_Reader)new XmlTextReaderImpl(input, numArray, num1, settings, (Uri)null, string.Empty, inputContext, settings.CloseInput);
      if (settings.ValidationType != ValidationType.None)
        reader = settings.AddValidation(reader);
      if (settings.Async)
        reader = (Xml_Reader)XmlAsyncCheckReader.CreateAsyncCheckWrapper(reader);
      return reader;
    }

    internal static int CalcBufferSize(Stream input)
    {
      int num = 4096;
      if (input.CanSeek)
      {
        long length = input.Length;
        if (length < (long)num)
          num = checked((int)length);
        else if (length > 65536L)
          num = 8192;
      }
      return num;
    }

    /// <summary>
    /// Ruft den Wert des aktuellen Knotens asynchron ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Wert des aktuellen Knotens.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<string> GetValueAsync()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position asynchron als <see cref="T:System.Object"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als geeignetstes CLR-Objekt (Common Language Runtime).
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<object> ReadContentAsObjectAsync()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsObject");
      return (object)await this.InternalReadContentAsStringAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Liest den Textinhalt an der aktuellen Position asynchron als <see cref="T:System.String"/>-Objekt.
    /// </summary>
    /// 
    /// <returns>
    /// Der Textinhalt als <see cref="T:System.String"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<string> ReadContentAsStringAsync()
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAsString");
      return this.InternalReadContentAsStringAsync();
    }

    /// <summary>
    /// Liest den Inhalt asynchron als Objekt vom angegebenen Typ.
    /// </summary>
    /// 
    /// <returns>
    /// Der verkettete Textinhalt oder Attributwert, der in den angeforderten Typ konvertiert wurde.
    /// </returns>
    /// <param name="returnType">Der Typ des zurückzugebenden Werts.</param><param name="namespaceResolver">Ein <see cref="T:System.Xml.IXmlNamespaceResolver"/>-Objekt, das für die Auflösung von Präfixen von Namespaces verwendet wird, die im Zusammenhang mit der Typkonvertierung stehen.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<object> ReadContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
    {
      if (!this.CanReadContentAs())
        throw this.CreateReadContentAsException("ReadContentAs");
      string str = await this.InternalReadContentAsStringAsync().ConfigureAwait(false);
      object obj;
      if (returnType == typeof(string))
      {
        obj = (object)str;
      }
      else
      {
        try
        {
          obj = XmlUntypedConverter.Untyped.ChangeType(str, returnType, namespaceResolver == null ? this as IXmlNamespaceResolver : namespaceResolver);
        }
        catch (FormatException ex)
        {
          throw new XmlException("Xml_ReadContentAsFormatException", returnType.ToString(), (Exception)ex, this as IXmlLineInfo);
        }
        catch (InvalidCastException ex)
        {
          throw new XmlException("Xml_ReadContentAsFormatException", returnType.ToString(), (Exception)ex, this as IXmlLineInfo);
        }
      }
      return obj;
    }

    /// <summary>
    /// Liest das aktuelle Element asynchron und gibt den Inhalt als <see cref="T:System.Object"/> zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Ein geschachteltes CLR-Objekt (Common Language Runtime) des geeignetsten Typs.Die <see cref="P:System.Xml.XmlReader.ValueType"/>-Eigenschaft bestimmt den geeigneten CLR-Typ.Wenn der Inhalt als Listentyp typisiert ist, gibt diese Methode ein Array der geschachtelten Objekte des geeigneten Typs zurück.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<object> ReadElementContentAsObjectAsync()
    {
      object obj;
      if (await this.SetupReadElementContentAsXxxAsync("ReadElementContentAsObject").ConfigureAwait(false))
      {
        object value = await this.ReadContentAsObjectAsync().ConfigureAwait(false);
        await this.FinishReadElementContentAsXxxAsync().ConfigureAwait(false);
        obj = value;
      }
      else
        obj = (object)string.Empty;
      return obj;
    }

    /// <summary>
    /// Liest das aktuelle Element asynchron und gibt den Inhalt als <see cref="T:System.String"/>-Objekt zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Der Elementinhalt als <see cref="T:System.String"/>-Objekt.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<string> ReadElementContentAsStringAsync()
    {
      string str;
      if (await this.SetupReadElementContentAsXxxAsync("ReadElementContentAsString").ConfigureAwait(false))
      {
        string value = await this.ReadContentAsStringAsync().ConfigureAwait(false);
        await this.FinishReadElementContentAsXxxAsync().ConfigureAwait(false);
        str = value;
      }
      else
        str = string.Empty;
      return str;
    }

    /// <summary>
    /// Liest den Elementinhalt asynchron als angeforderten Typ.
    /// </summary>
    /// 
    /// <returns>
    /// Der in das angeforderte typisierte Objekt konvertierte Elementinhalt.
    /// </returns>
    /// <param name="returnType">Der Typ des zurückzugebenden Werts.</param><param name="namespaceResolver">Ein <see cref="T:System.Xml.IXmlNamespaceResolver"/>-Objekt, das für die Auflösung von Präfixen von Namespaces verwendet wird, die im Zusammenhang mit der Typkonvertierung stehen.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<object> ReadElementContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
    {
      object obj;
      if (await this.SetupReadElementContentAsXxxAsync("ReadElementContentAs").ConfigureAwait(false))
      {
        object value = await this.ReadContentAsAsync(returnType, namespaceResolver).ConfigureAwait(false);
        await this.FinishReadElementContentAsXxxAsync().ConfigureAwait(false);
        obj = value;
      }
      else
        obj = returnType == typeof(string) ? (object)string.Empty : XmlUntypedConverter.Untyped.ChangeType(string.Empty, returnType, namespaceResolver);
      return obj;
    }

    /// <summary>
    /// Liest den nächsten Knoten aus dem Stream asynchron.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn der nächste Knoten erfolgreich gelesen wurde, false, wenn keine weiteren zu lesenden Knoten vorhanden sind.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<bool> ReadAsync()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Überspringt die untergeordneten Elemente des aktuellen Knotens asynchron.
    /// </summary>
    /// 
    /// <returns>
    /// Der aktuelle Knoten.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task SkipAsync()
    {
      if (this.ReadState != ReadState.Interactive)
        return AsyncHelper.DoneTask;
      return (Task)this.SkipSubtreeAsync();
    }

    /// <summary>
    /// Liest den Inhalt asynchron und gibt die Base64-decodierten binären Bytes zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadContentAsBase64"
      }));
    }

    /// <summary>
    /// Liest das Element asynchron und decodiert den Base64-Inhalt.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadElementContentAsBase64"
      }));
    }

    /// <summary>
    /// Liest den Inhalt asynchron und gibt die BinHex-decodierten binären Bytes zurück.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadContentAsBinHex"
      }));
    }

    /// <summary>
    /// Liest das Element asynchron und decodiert den BinHex-Inhalt.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer geschriebenen Bytes.
    /// </returns>
    /// <param name="buffer">Der Puffer, in den der resultierende Text kopiert werden soll.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, an dem mit dem Kopieren des Ergebnisses begonnen werden soll.</param><param name="count">Die maximale Anzahl von Bytes, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl von kopierten Bytes zurück.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[1]
      {
        (object) "ReadElementContentAsBinHex"
      }));
    }

    /// <summary>
    /// Liest asynchron umfangreiche Streams von Text, der in ein XML-Dokument eingebettet ist.
    /// </summary>
    /// 
    /// <returns>
    /// Die Anzahl der in den Puffer gelesenen Zeichen.Der Wert 0 (null) wird zurückgegeben, wenn kein weiterer Textinhalt vorhanden ist.
    /// </returns>
    /// <param name="buffer">Das Array von Zeichen, das als Puffer dient, in den der Textinhalt geschrieben wird.Dieser Wert darf nicht null sein.</param><param name="index">Der Offset im Puffer, ab dem der <see cref="T:System.Xml.XmlReader"/> die Ergebnisse kopieren kann.</param><param name="count">Die maximale Anzahl von Zeichen, die in den Puffer kopiert werden sollen.Diese Methode gibt die tatsächliche Anzahl der kopierten Zeichen zurück.</param><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual Task<int> ReadValueChunkAsync(char[] buffer, int index, int count)
    {
      throw new NotSupportedException(Res.GetString("Xml_ReadValueChunkNotSupported"));
    }

    /// <summary>
    /// Asynchrone Überprüfungen, ob der aktuelle Knoten ein Inhaltsknoten ist.Wenn der Knoten kein Inhaltsknoten ist, springt der Reader zum nächsten Inhaltsknoten oder an das Ende der Datei.
    /// </summary>
    /// 
    /// <returns>
    /// Der <see cref="P:System.Xml.XmlReader.NodeType"/> des von der Methode gefundenen aktuellen Knotens oder XmlNodeType.None, wenn der Reader das Ende des Eingabestreams erreicht hat.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<XmlNodeType> MoveToContentAsync()
    {
      XmlNodeType nodeType;
      do
      {
        switch (this.NodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.EntityReference:
          case XmlNodeType.EndElement:
          case XmlNodeType.EndEntity:
          nodeType = this.NodeType;
          goto label_6;
          case XmlNodeType.Attribute:
          this.MoveToElement();
          goto case XmlNodeType.Element;
          default:
          continue;
        }
      }
      while (await this.ReadAsync().ConfigureAwait(false));
      nodeType = this.NodeType;
    label_6:
      return nodeType;
    }

    /// <summary>
    /// Liest asynchron den gesamten Inhalt, einschließlich Markup als Zeichenfolge.
    /// </summary>
    /// 
    /// <returns>
    /// Der gesamte XML-Inhalt (einschließlich Markup) im aktuellen Knoten.Wenn der aktuelle Knoten keine untergeordneten Elemente besitzt, wird eine leere Zeichenfolge zurückgegeben.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<string> ReadInnerXmlAsync()
    {
      string str;
      if (this.ReadState != ReadState.Interactive)
        str = string.Empty;
      else if (this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Element)
      {
        int num = await this.ReadAsync().ConfigureAwait(false) ? 1 : 0;
        str = string.Empty;
      }
      else
      {
        StringWriter sw = new StringWriter((IFormatProvider)CultureInfo.InvariantCulture);
        XmlWriter xtw = this.CreateWriterForInnerOuterXml(sw);
        try
        {
          if (this.NodeType == XmlNodeType.Attribute)
          {
            ((XmlTextWriter)xtw).QuoteChar = this.QuoteChar;
            this.WriteAttributeValue(xtw);
          }
          if (this.NodeType == XmlNodeType.Element)
            await this.WriteNodeAsync(xtw, false).ConfigureAwait(false);
        }
        finally
        {
          xtw.Close();
        }
        str = sw.ToString();
      }
      return str;
    }

    private async Task WriteNodeAsync(XmlWriter xtw, bool defattr)
    {
      int d = this.NodeType == XmlNodeType.None ? -1 : this.Depth;
      while (true)
      {
        do
        {
          if (await this.ReadAsync().ConfigureAwait(false) && d < this.Depth)
          {
            switch (this.NodeType)
            {
              case XmlNodeType.Element:
              xtw.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
              ((XmlTextWriter)xtw).QuoteChar = this.QuoteChar;
              xtw.WriteAttributes(this, defattr);
              continue;
              case XmlNodeType.Text:
              goto label_4;
              case XmlNodeType.CDATA:
              goto label_8;
              case XmlNodeType.EntityReference:
              goto label_9;
              case XmlNodeType.ProcessingInstruction:
              case XmlNodeType.XmlDeclaration:
              goto label_10;
              case XmlNodeType.Comment:
              goto label_12;
              case XmlNodeType.DocumentType:
              goto label_11;
              case XmlNodeType.Whitespace:
              case XmlNodeType.SignificantWhitespace:
              goto label_6;
              case XmlNodeType.EndElement:
              goto label_13;
              default:
              continue;
            }
          }
          else
            goto label_16;
        }
        while (!this.IsEmptyElement);
        xtw.WriteEndElement();
        continue;
      label_4:
        XmlWriter xmlWriter = xtw;
        string text = await this.GetValueAsync().ConfigureAwait(false);
        xmlWriter.WriteString(text);
        xmlWriter = (XmlWriter)null;
        continue;
      label_6:
        xmlWriter = xtw;
        string ws = await this.GetValueAsync().ConfigureAwait(false);
        xmlWriter.WriteWhitespace(ws);
        xmlWriter = (XmlWriter)null;
        continue;
      label_8:
        xtw.WriteCData(this.Value);
        continue;
      label_9:
        xtw.WriteEntityRef(this.Name);
        continue;
      label_10:
        xtw.WriteProcessingInstruction(this.Name, this.Value);
        continue;
      label_11:
        xtw.WriteDocType(this.Name, this.GetAttribute("PUBLIC"), this.GetAttribute("SYSTEM"), this.Value);
        continue;
      label_12:
        xtw.WriteComment(this.Value);
        continue;
      label_13:
        xtw.WriteFullEndElement();
      }
    label_16:
      if (d == this.Depth && this.NodeType == XmlNodeType.EndElement)
      {
        int num = await this.ReadAsync().ConfigureAwait(false) ? 1 : 0;
      }
    }

    /// <summary>
    /// Liest den Inhalt, einschließlich Markup, das diesen Knoten und alle untergeordneten Elemente darstellt, asynchron.
    /// </summary>
    /// 
    /// <returns>
    /// Wenn der Reader auf einem Elementknoten oder einem Attributknoten positioniert ist, gibt diese Methode den gesamten XML-Inhalt (einschließlich Markup) des aktuellen Knotens sowie aller untergeordneten Elemente zurück. Andernfalls wird eine leere Zeichenfolge zurückgegeben.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/>-Methode wurde aufgerufen, bevor ein vorheriger asynchroner Vorgang abgeschlossen wurde.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Meldung ausgelöst "ein asynchroner Vorgang wird bereits ausgeführt."</exception><exception cref="T:System.InvalidOperationException">Eine <see cref="T:System.Xml.XmlReader"/> asynchrone Methode wurde aufgerufen, ohne das <see cref="P:System.Xml.XmlReaderSettings.Async"/>-Flag auf true festzulegen.In diesem Fall wird <see cref="T:System.InvalidOperationException"/> mit der Nachricht ausgelöst "Legen Sie XmlReaderSettings.Async auf True fest, wenn Sie die Async-Methoden verwenden möchten."</exception>
    [__DynamicallyInvokable]
    public virtual async Task<string> ReadOuterXmlAsync()
    {
      string str;
      if (this.ReadState != ReadState.Interactive)
        str = string.Empty;
      else if (this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Element)
      {
        int num = await this.ReadAsync().ConfigureAwait(false) ? 1 : 0;
        str = string.Empty;
      }
      else
      {
        StringWriter sw = new StringWriter((IFormatProvider)CultureInfo.InvariantCulture);
        XmlWriter forInnerOuterXml = this.CreateWriterForInnerOuterXml(sw);
        try
        {
          if (this.NodeType == XmlNodeType.Attribute)
          {
            forInnerOuterXml.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
            this.WriteAttributeValue(forInnerOuterXml);
            forInnerOuterXml.WriteEndAttribute();
          }
          else
            forInnerOuterXml.WriteNode(this, false);
        }
        finally
        {
          forInnerOuterXml.Close();
        }
        str = sw.ToString();
      }
      return str;
    }

    private async Task<bool> SkipSubtreeAsync()
    {
      this.MoveToElement();
      bool flag;
      if (this.NodeType == XmlNodeType.Element && !this.IsEmptyElement)
      {
        int depth = this.Depth;
        do
          ;
        while (await this.ReadAsync().ConfigureAwait(false) && depth < this.Depth);
        if (this.NodeType == XmlNodeType.EndElement)
          flag = await this.ReadAsync().ConfigureAwait(false);
        else
          flag = false;
      }
      else
        flag = await this.ReadAsync().ConfigureAwait(false);
      return flag;
    }

    internal async Task<string> InternalReadContentAsStringAsync()
    {
      string value = string.Empty;
      StringBuilder sb = (StringBuilder)null;
      string str1;
      bool flag;
      do
      {
        switch (this.NodeType)
        {
          case XmlNodeType.Attribute:
          str1 = this.Value;
          goto label_17;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          ConfiguredTaskAwaitable<string> configuredTaskAwaitable;
          if (value.Length == 0)
          {
            configuredTaskAwaitable = this.GetValueAsync().ConfigureAwait(false);
            value = await configuredTaskAwaitable;
            goto case XmlNodeType.ProcessingInstruction;
          }
          else
          {
            if (sb == null)
            {
              sb = new StringBuilder();
              sb.Append(value);
            }
            StringBuilder stringBuilder = sb;
            configuredTaskAwaitable = this.GetValueAsync().ConfigureAwait(false);
            string str2 = await configuredTaskAwaitable;
            stringBuilder.Append(str2);
            stringBuilder = (StringBuilder)null;
            goto case XmlNodeType.ProcessingInstruction;
          }
          case XmlNodeType.EntityReference:
          if (this.CanResolveEntity)
          {
            this.ResolveEntity();
            goto case XmlNodeType.ProcessingInstruction;
          }
          else
            goto label_16;
          case XmlNodeType.ProcessingInstruction:
          case XmlNodeType.Comment:
          case XmlNodeType.EndEntity:
          if (this.AttributeCount != 0)
            flag = this.ReadAttributeValue();
          else
            flag = await this.ReadAsync().ConfigureAwait(false);
          continue;
          default:
          goto label_16;
        }
      }
      while (flag);
    label_16:
      str1 = sb == null ? value : sb.ToString();
    label_17:
      return str1;
    }

    private async Task<bool> SetupReadElementContentAsXxxAsync(string methodName)
    {
      if (this.NodeType != XmlNodeType.Element)
        throw this.CreateReadElementContentAsException(methodName);
      bool isEmptyElement = this.IsEmptyElement;
      int num1 = await this.ReadAsync().ConfigureAwait(false) ? 1 : 0;
      bool flag;
      if (isEmptyElement)
      {
        flag = false;
      }
      else
      {
        XmlNodeType nodeType = this.NodeType;
        switch (nodeType)
        {
          case XmlNodeType.EndElement:
          int num2 = await this.ReadAsync().ConfigureAwait(false) ? 1 : 0;
          flag = false;
          break;
          case XmlNodeType.Element:
          throw new XmlException("Xml_MixedReadElementContentAs", string.Empty, this as IXmlLineInfo);
          default:
          flag = true;
          break;
        }
      }
      return flag;
    }

    private Task FinishReadElementContentAsXxxAsync()
    {
      if (this.NodeType != XmlNodeType.EndElement)
        throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString());
      return (Task)this.ReadAsync();
    }

    [DebuggerDisplay("{ToString()}")]
    private struct XmlReaderDebuggerDisplayProxy
    {
      private Xml_Reader reader;

      internal XmlReaderDebuggerDisplayProxy(Xml_Reader reader)
      {
        this.reader = reader;
      }

      public override string ToString()
      {
        XmlNodeType nodeType = this.reader.NodeType;
        string str = nodeType.ToString();
        switch (nodeType)
        {
          case XmlNodeType.Element:
          case XmlNodeType.EntityReference:
          case XmlNodeType.EndElement:
          case XmlNodeType.EndEntity:
          str = str + ", Name=\"" + this.reader.Name + "\"";
          break;
          case XmlNodeType.Attribute:
          case XmlNodeType.ProcessingInstruction:
          str = str + ", Name=\"" + this.reader.Name + "\", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value) + "\"";
          break;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.Comment:
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
          case XmlNodeType.XmlDeclaration:
          str = str + ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value) + "\"";
          break;
          case XmlNodeType.DocumentType:
          str = str + ", Name=\"" + this.reader.Name + "'" + ", SYSTEM=\"" + this.reader.GetAttribute("SYSTEM") + "\"" + ", PUBLIC=\"" + this.reader.GetAttribute("PUBLIC") + "\"" + ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value) + "\"";
          break;
        }
        return str;
      }
    }
  }
}
