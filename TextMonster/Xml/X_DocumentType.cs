using System;
using System.Xml;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt eine XML-Dokumenttypdefinition (DTD) dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  // ReSharper disable once InconsistentNaming
  public class X_DocumentType : X_Node
  {
    string name;
    string publicId;
    string systemId;
    string internalSubset;

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
        return internalSubset;
      }
      set
      {
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Value);
        internalSubset = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Value);
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
        return name;
      }
      set
      {
        value = XmlConvert.VerifyName(value);
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Name);
        name = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Name);
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
        return publicId;
      }
      set
      {
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Value);
        publicId = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Value);
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
        return systemId;
      }
      set
      {
        bool flag = NotifyChanging(this, X_ObjectChangeEventArgs.Value);
        systemId = value;
        if (!flag)
          return;
        NotifyChanged(this, X_ObjectChangeEventArgs.Value);
      }
    }

    /// <summary>
    /// Initialisiert eine Instanz der <see cref="T:System.Xml.Linq.XDocumentType"/>-Klasse.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.String"/>, der den qualifizierten Namen der DTD enthält. Dieser stimmt mit dem qualifizierten Namen des Stammelements des XML-Dokuments überein.</param><param name="publicId">Ein <see cref="T:System.String"/>, der den öffentlichen Bezeichner einer externen öffentlichen DTD enthält.</param><param name="systemId">Ein <see cref="T:System.String"/>, der den Systembezeichner einer externen privaten DTD enthält.</param><param name="internalSubset">Ein <see cref="T:System.String"/>, der die interne Teilmenge für eine interne DTD enthält.</param>
    public X_DocumentType(string name, string publicId, string systemId, string internalSubset)
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
    public X_DocumentType(X_DocumentType other)
    {
      if (other == null) throw new ArgumentNullException("other");
      name = other.name;
      publicId = other.publicId;
      systemId = other.systemId;
      internalSubset = other.internalSubset;
    }

    internal X_DocumentType(XmlReader r)
    {
      name = r.Name;
      publicId = r.GetAttribute("PUBLIC");
      systemId = r.GetAttribute("SYSTEM");
      internalSubset = r.Value;
      r.Read();
    }

    /// <summary>
    /// Schreibt diesen <see cref="T:System.Xml.Linq.XDocumentType"/> in einen <see cref="T:System.Xml.XmlWriter"/>.
    /// </summary>
    /// <param name="writer">Ein <see cref="T:System.Xml.XmlWriter"/>, in den diese Methode schreibt.</param><filterpriority>2</filterpriority>
    public override void WriteTo(XmlWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      writer.WriteDocType(name, publicId, systemId, internalSubset);
    }

    internal override X_Node CloneNode()
    {
      return new X_DocumentType(this);
    }

    internal override bool DeepEquals(X_Node node)
    {
      var xdocumentType = node as X_DocumentType;
      if (xdocumentType != null && name == xdocumentType.name && (publicId == xdocumentType.publicId && systemId == xdocumentType.SystemId))
        return internalSubset == xdocumentType.internalSubset;
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return name.GetHashCode() ^ (publicId != null ? publicId.GetHashCode() : 0) ^ (systemId != null ? systemId.GetHashCode() : 0) ^ (internalSubset != null ? internalSubset.GetHashCode() : 0);
    }
  }
}
