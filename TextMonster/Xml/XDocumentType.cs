using System;
using System.Xml;

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt eine XML-Dokumenttypdefinition (DTD) dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public class XDocumentType : XNode
  {
    private string name;
    private string publicId;
    private string systemId;
    private string internalSubset;
    private IDtdInfo dtdInfo;

    /// <summary>
    /// Ruft die interne Teilmenge für die Dokumenttypdefinition (DTD) ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die interne Teilmenge für diese Dokumenttypdefinition (DTD) enthält.
    /// </returns>
    public string InternalSubset
    {
      get
      {
        return this.internalSubset;
      }
      set
      {
        bool flag = this.NotifyChanging((object)this, XObjectChangeEventArgs.Value);
        this.internalSubset = value;
        if (!flag)
          return;
        this.NotifyChanged((object)this, XObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Ruft den Namen für die Dokumenttypdefinition (DTD) ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Namen für diese Dokumenttypdefinition (DTD) enthält.
    /// </returns>
    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        value = XmlConvert.VerifyName(value);
        bool flag = this.NotifyChanging((object)this, XObjectChangeEventArgs.Name);
        this.name = value;
        if (!flag)
          return;
        this.NotifyChanged((object)this, XObjectChangeEventArgs.Name);
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für diesen Knoten ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp. Für <see cref="T:System.Xml.Linq.XDocumentType"/>-Objekte ist dieser Wert <see cref="F:System.Xml.XmlNodeType.DocumentType"/>.
    /// </returns>
    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.DocumentType;
      }
    }

    /// <summary>
    /// Ruft den öffentlichen Bezeichner für die Dokumenttypdefinition (DTD) ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den öffentlichen Bezeichner für diese Dokumenttypdefinition (DTD) enthält.
    /// </returns>
    public string PublicId
    {
      get
      {
        return this.publicId;
      }
      set
      {
        bool flag = this.NotifyChanging((object)this, XObjectChangeEventArgs.Value);
        this.publicId = value;
        if (!flag)
          return;
        this.NotifyChanged((object)this, XObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Ruft den Systembezeichner für die Dokumenttypdefinition (DTD) ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Systembezeichner für diese Dokumenttypdefinition (DTD) enthält.
    /// </returns>
    public string SystemId
    {
      get
      {
        return this.systemId;
      }
      set
      {
        bool flag = this.NotifyChanging((object)this, XObjectChangeEventArgs.Value);
        this.systemId = value;
        if (!flag)
          return;
        this.NotifyChanged((object)this, XObjectChangeEventArgs.Value);
      }
    }

    internal IDtdInfo DtdInfo
    {
      get
      {
        return this.dtdInfo;
      }
    }

    /// <summary>
    /// Initialisiert eine Instanz der <see cref="T:System.Xml.Linq.XDocumentType"/>-Klasse.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.String"/>, der den qualifizierten Namen der DTD enthält. Dieser stimmt mit dem qualifizierten Namen des Stammelements des XML-Dokuments überein.</param><param name="publicId">Ein <see cref="T:System.String"/>, der den öffentlichen Bezeichner einer externen öffentlichen DTD enthält.</param><param name="systemId">Ein <see cref="T:System.String"/>, der den Systembezeichner einer externen privaten DTD enthält.</param><param name="internalSubset">Ein <see cref="T:System.String"/>, der die interne Teilmenge für eine interne DTD enthält.</param>
    public XDocumentType(string name, string publicId, string systemId, string internalSubset)
    {
      this.name = XmlConvert.VerifyName(name);
      this.publicId = publicId;
      this.systemId = systemId;
      this.internalSubset = internalSubset;
    }

    /// <summary>
    /// Initialisiert eine Instanz der <see cref="T:System.Xml.Linq.XDocumentType"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XDocumentType"/>-Objekt
    /// </summary>
    /// <param name="other">Ein <see cref="T:System.Xml.Linq.XDocumentType"/>-Objekt, aus dem kopiert werden soll.</param>
    public XDocumentType(XDocumentType other)
    {
      if (other == null) throw new ArgumentNullException("other");
      this.name = other.name;
      this.publicId = other.publicId;
      this.systemId = other.systemId;
      this.internalSubset = other.internalSubset;
      this.dtdInfo = other.dtdInfo;
    }

    internal XDocumentType(XmlReader r)
    {
      this.name = r.Name;
      this.publicId = r.GetAttribute("PUBLIC");
      this.systemId = r.GetAttribute("SYSTEM");
      this.internalSubset = r.Value;
      this.dtdInfo = r.DtdInfo;
      r.Read();
    }

    internal XDocumentType(string name, string publicId, string systemId, string internalSubset, IDtdInfo dtdInfo)
      : this(name, publicId, systemId, internalSubset)
    {
      this.dtdInfo = dtdInfo;
    }

    /// <summary>
    /// Schreibt diesen <see cref="T:System.Xml.Linq.XDocumentType"/> in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      writer.WriteDocType(this.name, this.publicId, this.systemId, this.internalSubset);
    }

    internal override XNode CloneNode()
    {
      return (XNode)new XDocumentType(this);
    }

    internal override bool DeepEquals(XNode node)
    {
      XDocumentType xdocumentType = node as XDocumentType;
      if (xdocumentType != null && this.name == xdocumentType.name && (this.publicId == xdocumentType.publicId && this.systemId == xdocumentType.SystemId))
        return this.internalSubset == xdocumentType.internalSubset;
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return this.name.GetHashCode() ^ (this.publicId != null ? this.publicId.GetHashCode() : 0) ^ (this.systemId != null ? this.systemId.GetHashCode() : 0) ^ (this.internalSubset != null ? this.internalSubset.GetHashCode() : 0);
    }
  }
}
