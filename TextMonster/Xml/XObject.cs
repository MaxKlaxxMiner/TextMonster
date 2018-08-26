
namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Knoten oder ein Attribut in einer XML-Struktur dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public abstract class XObject : IXmlLineInfo
  {
    internal XContainer parent;
    internal object annotations;

    /// <summary>
    /// Ruft den Basis-URI für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>, der den Basis-URI für dieses <see cref="T:System.Xml.Linq.XObject"/> enthält.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public string BaseUri
    {
      get
      {
        XObject xobject = this;
        while (true)
        {
          for (; xobject == null || xobject.annotations != null; xobject = (XObject)xobject.parent)
          {
            if (xobject == null)
              return string.Empty;
            BaseUriAnnotation baseUriAnnotation = xobject.Annotation<BaseUriAnnotation>();
            if (baseUriAnnotation != null)
              return baseUriAnnotation.baseUri;
          }
          xobject = (XObject)xobject.parent;
        }
      }
    }

    /// <summary>
    /// Ruft das <see cref="T:System.Xml.Linq.XDocument"/> für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das <see cref="T:System.Xml.Linq.XDocument"/> für dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    public XDocument Document
    {
      [__DynamicallyInvokable]
      get
      {
        XObject xobject = this;
        while (xobject.parent != null)
          xobject = (XObject)xobject.parent;
        return xobject as XDocument;
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp für dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public abstract XmlNodeType NodeType { [__DynamicallyInvokable] get; }

    /// <summary>
    /// Ruft das übergeordnete <see cref="T:System.Xml.Linq.XElement"/> dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das übergeordnete <see cref="T:System.Xml.Linq.XElement"/> dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public XElement Parent
    {
      [__DynamicallyInvokable]
      get
      {
        return this.parent as XElement;
      }
    }

    [__DynamicallyInvokable]
    int IXmlLineInfo.LineNumber
    {
      [__DynamicallyInvokable]
      get
      {
        LineInfoAnnotation lineInfoAnnotation = this.Annotation<LineInfoAnnotation>();
        if (lineInfoAnnotation != null)
          return lineInfoAnnotation.lineNumber;
        return 0;
      }
    }

    [__DynamicallyInvokable]
    int IXmlLineInfo.LinePosition
    {
      [__DynamicallyInvokable]
      get
      {
        LineInfoAnnotation lineInfoAnnotation = this.Annotation<LineInfoAnnotation>();
        if (lineInfoAnnotation != null)
          return lineInfoAnnotation.linePosition;
        return 0;
      }
    }

    internal bool HasBaseUri
    {
      get
      {
        return this.Annotation<BaseUriAnnotation>() != null;
      }
    }

    /// <summary>
    /// Wird ausgelöst, wenn dieses <see cref="T:System.Xml.Linq.XObject"/> oder eines seiner untergeordneten Elemente geändert wurde.
    /// </summary>
    [__DynamicallyInvokable]
    public event EventHandler<XObjectChangeEventArgs> Changed
    {
      [__DynamicallyInvokable]
      add
      {
        if (value == null)
          return;
        XObjectChangeAnnotation changeAnnotation = this.Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
        {
          changeAnnotation = new XObjectChangeAnnotation();
          this.AddAnnotation((object)changeAnnotation);
        }
        changeAnnotation.changed += value;
      }
      [__DynamicallyInvokable]
      remove
      {
        if (value == null)
          return;
        XObjectChangeAnnotation changeAnnotation = this.Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
          return;
        changeAnnotation.changed -= value;
        if (changeAnnotation.changing != null || changeAnnotation.changed != null)
          return;
        this.RemoveAnnotations<XObjectChangeAnnotation>();
      }
    }

    /// <summary>
    /// Wird ausgelöst, wenn dieses <see cref="T:System.Xml.Linq.XObject"/> oder eines seiner untergeordneten Elemente gerade geändert wird.
    /// </summary>
    [__DynamicallyInvokable]
    public event EventHandler<XObjectChangeEventArgs> Changing
    {
      [__DynamicallyInvokable]
      add
      {
        if (value == null)
          return;
        XObjectChangeAnnotation changeAnnotation = this.Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
        {
          changeAnnotation = new XObjectChangeAnnotation();
          this.AddAnnotation((object)changeAnnotation);
        }
        changeAnnotation.changing += value;
      }
      [__DynamicallyInvokable]
      remove
      {
        if (value == null)
          return;
        XObjectChangeAnnotation changeAnnotation = this.Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
          return;
        changeAnnotation.changing -= value;
        if (changeAnnotation.changing != null || changeAnnotation.changed != null)
          return;
        this.RemoveAnnotations<XObjectChangeAnnotation>();
      }
    }

    internal XObject()
    {
    }

    /// <summary>
    /// Fügt der Anmerkungsliste dieses <see cref="T:System.Xml.Linq.XObject"/> ein Objekt hinzu.
    /// </summary>
    /// <param name="annotation">Ein <see cref="T:System.Object"/>, das die hinzuzufügende Anmerkung enthält.</param>
    [__DynamicallyInvokable]
    public void AddAnnotation(object annotation)
    {
      // ISSUE: unable to decompile the method.
    }

    /// <summary>
    /// Ruft das erste Anmerkungsobjekt des angegebenen Typs aus diesem <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das <see cref="T:System.Object"/> mit dem ersten Anmerkungsobjekt, das mit dem angegebenen Typ übereinstimmt, oder null, wenn keine Anmerkung den angegebenen Typ aufweist.
    /// </returns>
    /// <param name="type">Der <see cref="T:System.Type"/> der abzurufenden Anmerkung.</param>
    [__DynamicallyInvokable]
    public object Annotation(Type type)
    {
      if (type == (Type)null)
        throw new ArgumentNullException("type");
      if (this.annotations != null)
      {
        object[] objArray = this.annotations as object[];
        if (objArray == null)
        {
          if (type.IsInstanceOfType(this.annotations))
            return this.annotations;
        }
        else
        {
          for (int index = 0; index < objArray.Length; ++index)
          {
            object o = objArray[index];
            if (o != null)
            {
              if (type.IsInstanceOfType(o))
                return o;
            }
            else
              break;
          }
        }
      }
      return (object)null;
    }

    /// <summary>
    /// Ruft das erste Anmerkungsobjekt des angegebenen Typs aus diesem <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das erste Anmerkungsobjekt, das mit dem angegebenen Typ übereinstimmt, oder null, wenn keine Anmerkung den angegebenen Typ aufweist.
    /// </returns>
    /// <typeparam name="T">Der Typ der abzurufenden Anmerkung.</typeparam>
    [__DynamicallyInvokable]
    public T Annotation<T>() where T : class
    {
      if (this.annotations != null)
      {
        object[] objArray = this.annotations as object[];
        if (objArray == null)
          return this.annotations as T;
        for (int index = 0; index < objArray.Length; ++index)
        {
          object obj1 = objArray[index];
          if (obj1 != null)
          {
            T obj2 = obj1 as T;
            if ((object)obj2 != null)
              return obj2;
          }
          else
            break;
        }
      }
      return default(T);
    }

    /// <summary>
    /// Ruft eine Auflistung von Anmerkungen des angegebenen Typs für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/> vom Typ <see cref="T:System.Object"/>, das die Anmerkungen enthält, die mit dem angegebenen Typ für dieses <see cref="T:System.Xml.Linq.XObject"/> übereinstimmen.
    /// </returns>
    /// <param name="type">Der <see cref="T:System.Type"/> der abzurufenden Anmerkungen.</param>
    [__DynamicallyInvokable]
    public IEnumerable<object> Annotations(Type type)
    {
      if (type == (Type)null)
        throw new ArgumentNullException("type");
      return this.AnnotationsIterator(type);
    }

    private IEnumerable<object> AnnotationsIterator(Type type)
    {
      if (this.annotations != null)
      {
        object[] a = this.annotations as object[];
        if (a == null)
        {
          if (type.IsInstanceOfType(this.annotations))
            yield return this.annotations;
        }
        else
        {
          for (int i = 0; i < a.Length; ++i)
          {
            object o = a[i];
            if (o != null)
            {
              if (type.IsInstanceOfType(o))
                yield return o;
            }
            else
              break;
          }
        }
        a = (object[])null;
      }
    }

    /// <summary>
    /// Ruft eine Auflistung von Anmerkungen des angegebenen Typs für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.Collections.Generic.IEnumerable`1"/>, das die Anmerkungen für dieses <see cref="T:System.Xml.Linq.XObject"/> enthält.
    /// </returns>
    /// <typeparam name="T">Der Typ der abzurufenden Anmerkungen.</typeparam>
    [__DynamicallyInvokable]
    public IEnumerable<T> Annotations<T>() where T : class
    {
      if (this.annotations != null)
      {
        object[] a = this.annotations as object[];
        if (a == null)
        {
          T obj = this.annotations as T;
          if ((object)obj != null)
            yield return obj;
        }
        else
        {
          for (int i = 0; i < a.Length; ++i)
          {
            object obj1 = a[i];
            if (obj1 != null)
            {
              T obj2 = obj1 as T;
              if ((object)obj2 != null)
                yield return obj2;
            }
            else
              break;
          }
        }
        a = (object[])null;
      }
    }

    /// <summary>
    /// Entfernt die Anmerkungen vom angegebenen Typ aus diesem <see cref="T:System.Xml.Linq.XObject"/>.
    /// </summary>
    /// <param name="type">Der <see cref="T:System.Type"/> der zu entfernenden Anmerkungen.</param>
    [__DynamicallyInvokable]
    public void RemoveAnnotations(Type type)
    {
      if (type == (Type)null)
        throw new ArgumentNullException("type");
      if (this.annotations == null)
        return;
      object[] objArray = this.annotations as object[];
      if (objArray == null)
      {
        if (!type.IsInstanceOfType(this.annotations))
          return;
        this.annotations = (object)null;
      }
      else
      {
        int index = 0;
        int num = 0;
        for (; index < objArray.Length; ++index)
        {
          object o = objArray[index];
          if (o != null)
          {
            if (!type.IsInstanceOfType(o))
              objArray[num++] = o;
          }
          else
            break;
        }
        if (num == 0)
        {
          this.annotations = (object)null;
        }
        else
        {
          while (num < index)
            objArray[num++] = (object)null;
        }
      }
    }

    /// <summary>
    /// Entfernt die Anmerkungen vom angegebenen Typ aus diesem <see cref="T:System.Xml.Linq.XObject"/>.
    /// </summary>
    /// <typeparam name="T">Der Typ der zu entfernenden Anmerkungen.</typeparam>
    [__DynamicallyInvokable]
    public void RemoveAnnotations<T>() where T : class
    {
      if (this.annotations == null)
        return;
      object[] objArray = this.annotations as object[];
      if (objArray == null)
      {
        if (!(this.annotations is T))
          return;
        this.annotations = (object)null;
      }
      else
      {
        int index = 0;
        int num = 0;
        for (; index < objArray.Length; ++index)
        {
          object obj = objArray[index];
          if (obj != null)
          {
            if (!(obj is T))
              objArray[num++] = obj;
          }
          else
            break;
        }
        if (num == 0)
        {
          this.annotations = (object)null;
        }
        else
        {
          while (num < index)
            objArray[num++] = (object)null;
        }
      }
    }

    [__DynamicallyInvokable]
    bool IXmlLineInfo.HasLineInfo()
    {
      return this.Annotation<LineInfoAnnotation>() != null;
    }

    internal bool NotifyChanged(object sender, XObjectChangeEventArgs e)
    {
      bool flag = false;
      XObject xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (XObject)xobject.parent)
        {
          if (xobject == null)
            return flag;
          XObjectChangeAnnotation changeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
          if (changeAnnotation != null)
          {
            flag = true;
            if (changeAnnotation.changed != null)
              changeAnnotation.changed(sender, e);
          }
        }
        xobject = (XObject)xobject.parent;
      }
    }

    internal bool NotifyChanging(object sender, XObjectChangeEventArgs e)
    {
      bool flag = false;
      XObject xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (XObject)xobject.parent)
        {
          if (xobject == null)
            return flag;
          XObjectChangeAnnotation changeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
          if (changeAnnotation != null)
          {
            flag = true;
            if (changeAnnotation.changing != null)
              changeAnnotation.changing(sender, e);
          }
        }
        xobject = (XObject)xobject.parent;
      }
    }

    internal void SetBaseUri(string baseUri)
    {
      this.AddAnnotation((object)new BaseUriAnnotation(baseUri));
    }

    internal void SetLineInfo(int lineNumber, int linePosition)
    {
      this.AddAnnotation((object)new LineInfoAnnotation(lineNumber, linePosition));
    }

    internal bool SkipNotify()
    {
      XObject xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (XObject)xobject.parent)
        {
          if (xobject == null)
            return true;
          if (xobject.Annotations<XObjectChangeAnnotation>() != null)
            return false;
        }
        xobject = (XObject)xobject.parent;
      }
    }

    internal SaveOptions GetSaveOptionsFromAnnotations()
    {
      XObject xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (XObject)xobject.parent)
        {
          if (xobject == null)
            return SaveOptions.None;
          object obj = xobject.Annotation(typeof(SaveOptions));
          if (obj != null)
            return (SaveOptions)obj;
        }
        xobject = (XObject)xobject.parent;
      }
    }
  }
}
