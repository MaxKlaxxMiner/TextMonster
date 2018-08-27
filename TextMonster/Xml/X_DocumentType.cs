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

    /// <summary>
    /// Ruft die interne Teilmenge für die Dokumenttypdefinition (DTD) ab oder legt diese fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der die interne Teilmenge für diese Dokumenttypdefinition (DTD) enthält.
    /// </returns>
    public string InternalSubset { get; set; }

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
        name = value;
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
    public string PublicId { get; set; }

    /// <summary>
    /// Ruft den Systembezeichner für die Dokumenttypdefinition (DTD) ab oder legt diesen fest.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Systembezeichner für diese Dokumenttypdefinition (DTD) enthält.
    /// </returns>
    public string SystemId { get; set; }

    /// <summary>
    /// Initialisiert eine Instanz der <see cref="T:System.Xml.Linq.XDocumentType"/>-Klasse.
    /// </summary>
    /// <param name="name">Ein <see cref="T:System.String"/>, der den qualifizierten Namen der DTD enthält. Dieser stimmt mit dem qualifizierten Namen des Stammelements des XML-Dokuments überein.</param><param name="publicId">Ein <see cref="T:System.String"/>, der den öffentlichen Bezeichner einer externen öffentlichen DTD enthält.</param><param name="systemId">Ein <see cref="T:System.String"/>, der den Systembezeichner einer externen privaten DTD enthält.</param><param name="internalSubset">Ein <see cref="T:System.String"/>, der die interne Teilmenge für eine interne DTD enthält.</param>
    public X_DocumentType(string name, string publicId, string systemId, string internalSubset)
    {
      this.name = XmlConvert.VerifyName(name);
      PublicId = publicId;
      SystemId = systemId;
      InternalSubset = internalSubset;
    }

    /// <summary>
    /// Initialisiert eine Instanz der <see cref="T:System.Xml.Linq.XDocumentType"/>-Klasse mit einem anderen <see cref="T:System.Xml.Linq.XDocumentType"/>-Objekt
    /// </summary>
    /// <param name="other">Ein <see cref="T:System.Xml.Linq.XDocumentType"/>-Objekt, aus dem kopiert werden soll.</param>
    public X_DocumentType(X_DocumentType other)
    {
      if (other == null) throw new ArgumentNullException("other");
      name = other.name;
      PublicId = other.PublicId;
      SystemId = other.SystemId;
      InternalSubset = other.InternalSubset;
    }

    internal X_DocumentType(XmlReader r)
    {
      name = r.Name;
      PublicId = r.GetAttribute("PUBLIC");
      SystemId = r.GetAttribute("SYSTEM");
      InternalSubset = r.Value;
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
      writer.WriteDocType(name, PublicId, SystemId, InternalSubset);
    }

    internal override X_Node CloneNode()
    {
      return new X_DocumentType(this);
    }

    internal override bool DeepEquals(X_Node node)
    {
      var xdocumentType = node as X_DocumentType;
      if (xdocumentType != null && name == xdocumentType.name && (PublicId == xdocumentType.PublicId && SystemId == xdocumentType.SystemId))
        return InternalSubset == xdocumentType.InternalSubset;
      return false;
    }

    internal override int GetDeepHashCode()
    {
      return name.GetHashCode() ^ (PublicId != null ? PublicId.GetHashCode() : 0) ^ (SystemId != null ? SystemId.GetHashCode() : 0) ^ (InternalSubset != null ? InternalSubset.GetHashCode() : 0);
    }
  }
}
