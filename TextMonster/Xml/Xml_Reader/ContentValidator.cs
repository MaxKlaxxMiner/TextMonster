using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Basic ContentValidator
  /// </summary>
  class ContentValidator
  {
    XmlSchemaContentType contentType;
    bool isOpen;  //For XDR Content Models or ANY
    bool isEmptiable;

    public static readonly ContentValidator Empty = new ContentValidator(XmlSchemaContentType.Empty);
    public static readonly ContentValidator TextOnly = new ContentValidator(XmlSchemaContentType.TextOnly, false, false);
    public static readonly ContentValidator Mixed = new ContentValidator(XmlSchemaContentType.Mixed);
    public static readonly ContentValidator Any = new ContentValidator(XmlSchemaContentType.Mixed, true, true);

    public ContentValidator(XmlSchemaContentType contentType)
    {
      this.contentType = contentType;
      this.isEmptiable = true;
    }

    protected ContentValidator(XmlSchemaContentType contentType, bool isOpen, bool isEmptiable)
    {
      this.contentType = contentType;
      this.isOpen = isOpen;
      this.isEmptiable = isEmptiable;
    }

    public XmlSchemaContentType ContentType
    {
      get { return contentType; }
    }

    public bool PreserveWhitespace
    {
      get { return contentType == XmlSchemaContentType.TextOnly || contentType == XmlSchemaContentType.Mixed; }
    }

    public bool IsOpen
    {
      get
      {
        if (contentType == XmlSchemaContentType.TextOnly || contentType == XmlSchemaContentType.Empty)
          return false;
        else
          return isOpen;
      }
      set { isOpen = value; }
    }

    public virtual void InitValidation(ValidationState context)
    {
      // do nothin'
    }

    public virtual object ValidateElement(XmlQualifiedName name, ValidationState context, out int errorCode)
    {
      if (contentType == XmlSchemaContentType.TextOnly || contentType == XmlSchemaContentType.Empty)
      { //Cannot have elements in TextOnly or Empty content
        context.NeedValidateChildren = false;
      }
      errorCode = -1;
      return null;
    }

    public virtual bool CompleteValidation(ValidationState context)
    {
      return true;
    }

    public virtual ArrayList ExpectedElements(ValidationState context, bool isRequiredOnly)
    {
      return null;
    }

    public virtual ArrayList ExpectedParticles(ValidationState context, bool isRequiredOnly, XmlSchemaSet schemaSet)
    {
      return null;
    }

    public static void AddParticleToExpected(XmlSchemaParticle p, XmlSchemaSet schemaSet, ArrayList particles)
    {
      AddParticleToExpected(p, schemaSet, particles, false);
    }

    public static void AddParticleToExpected(XmlSchemaParticle p, XmlSchemaSet schemaSet, ArrayList particles, bool global)
    {
      if (!particles.Contains(p))
      {
        particles.Add(p);
      }
      //Only then it can be head of substitutionGrp, if it is, add its members 
      XmlSchemaElement elem = p as XmlSchemaElement;
      if (elem != null && (global || !elem.RefName.IsEmpty))
      {
        XmlSchemaObjectTable substitutionGroups = schemaSet.SubstitutionGroups;
        XmlSchemaSubstitutionGroup grp = (XmlSchemaSubstitutionGroup)substitutionGroups[elem.QualifiedName];
        if (grp != null)
        {
          //Grp members wil contain the head as well, so filter head as we added it already
          for (int i = 0; i < grp.Members.Count; ++i)
          {
            XmlSchemaElement member = (XmlSchemaElement)grp.Members[i];
            if (!elem.QualifiedName.Equals(member.QualifiedName) && !particles.Contains(member))
            { //A member might have been directly present as an element in the content model
              particles.Add(member);
            }
          }
        }
      }
    }
  }
}
