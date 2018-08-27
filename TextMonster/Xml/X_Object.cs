using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TextMonster.Xml
{
  /// <summary>
  /// Stellt einen Knoten oder ein Attribut in einer XML-Struktur dar.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  // ReSharper disable once InconsistentNaming
  public abstract class X_Object
  {
    internal X_Container parent;
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
        var xobject = this;
        while (true)
        {
          for (; xobject == null || xobject.annotations != null; xobject = (X_Object)xobject.parent)
          {
            if (xobject == null)
              return string.Empty;
            var baseUriAnnotation = xobject.Annotation<BaseUriAnnotation>();
            if (baseUriAnnotation != null)
              return baseUriAnnotation.baseUri;
          }
          xobject = xobject.parent;
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
    public X_Document Document
    {
      get
      {
        var xobject = this;
        while (xobject.parent != null)
          xobject = xobject.parent;
        return xobject as X_Document;
      }
    }

    /// <summary>
    /// Ruft den Knotentyp für dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Der Knotentyp für dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    public abstract XmlNodeType NodeType { get; }

    /// <summary>
    /// Ruft das übergeordnete <see cref="T:System.Xml.Linq.XElement"/> dieses <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das übergeordnete <see cref="T:System.Xml.Linq.XElement"/> dieses <see cref="T:System.Xml.Linq.XObject"/>.
    /// </returns>
    public X_Element Parent
    {
      get
      {
        return parent as X_Element;
      }
    }

    internal bool HasBaseUri
    {
      get
      {
        return Annotation<BaseUriAnnotation>() != null;
      }
    }

    /// <summary>
    /// Wird ausgelöst, wenn dieses <see cref="T:System.Xml.Linq.XObject"/> oder eines seiner untergeordneten Elemente geändert wurde.
    /// </summary>
    public event EventHandler<X_ObjectChangeEventArgs> Changed
    {
      add
      {
        if (value == null)
          return;
        var changeAnnotation = Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
        {
          changeAnnotation = new XObjectChangeAnnotation();
          AddAnnotation(changeAnnotation);
        }
        changeAnnotation.changed += value;
      }
      remove
      {
        if (value == null)
          return;
        var changeAnnotation = Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
          return;
        changeAnnotation.changed -= value;
        if (changeAnnotation.changing != null || changeAnnotation.changed != null)
          return;
        RemoveAnnotations<XObjectChangeAnnotation>();
      }
    }

    /// <summary>
    /// Wird ausgelöst, wenn dieses <see cref="T:System.Xml.Linq.XObject"/> oder eines seiner untergeordneten Elemente gerade geändert wird.
    /// </summary>
    public event EventHandler<X_ObjectChangeEventArgs> Changing
    {
      add
      {
        if (value == null)
          return;
        var changeAnnotation = Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
        {
          changeAnnotation = new XObjectChangeAnnotation();
          AddAnnotation(changeAnnotation);
        }
        changeAnnotation.changing += value;
      }
      remove
      {
        if (value == null)
          return;
        var changeAnnotation = Annotation<XObjectChangeAnnotation>();
        if (changeAnnotation == null)
          return;
        changeAnnotation.changing -= value;
        if (changeAnnotation.changing != null || changeAnnotation.changed != null)
          return;
        RemoveAnnotations<XObjectChangeAnnotation>();
      }
    }

    internal X_Object()
    {
    }

    /// <summary>
    /// Fügt der Anmerkungsliste dieses <see cref="T:System.Xml.Linq.XObject"/> ein Objekt hinzu.
    /// </summary>
    /// <param name="annotation">Ein <see cref="T:System.Object"/>, das die hinzuzufügende Anmerkung enthält.</param>
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
    public object Annotation(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      if (annotations != null)
      {
        var objArray = annotations as object[];
        if (objArray == null)
        {
          if (type.IsInstanceOfType(annotations))
            return annotations;
        }
        else
        {
          for (int index = 0; index < objArray.Length; ++index)
          {
            var o = objArray[index];
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
      return null;
    }

    /// <summary>
    /// Ruft das erste Anmerkungsobjekt des angegebenen Typs aus diesem <see cref="T:System.Xml.Linq.XObject"/> ab.
    /// </summary>
    /// 
    /// <returns>
    /// Das erste Anmerkungsobjekt, das mit dem angegebenen Typ übereinstimmt, oder null, wenn keine Anmerkung den angegebenen Typ aufweist.
    /// </returns>
    /// <typeparam name="T">Der Typ der abzurufenden Anmerkung.</typeparam>
    public T Annotation<T>() where T : class
    {
      if (annotations != null)
      {
        var objArray = annotations as object[];
        if (objArray == null)
          return annotations as T;
        for (int index = 0; index < objArray.Length; ++index)
        {
          var obj1 = objArray[index];
          if (obj1 != null)
          {
            var obj2 = obj1 as T;
            if (obj2 != null)
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
    public IEnumerable<object> Annotations(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      return AnnotationsIterator(type);
    }

    IEnumerable<object> AnnotationsIterator(Type type)
    {
      if (annotations != null)
      {
        var a = annotations as object[];
        if (a == null)
        {
          if (type.IsInstanceOfType(annotations))
            yield return annotations;
        }
        else
        {
          for (int i = 0; i < a.Length; ++i)
          {
            var o = a[i];
            if (o != null)
            {
              if (type.IsInstanceOfType(o))
                yield return o;
            }
            else
              break;
          }
        }
        a = null;
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
    public IEnumerable<T> Annotations<T>() where T : class
    {
      if (annotations != null)
      {
        var a = annotations as object[];
        if (a == null)
        {
          var obj = annotations as T;
          if (obj != null)
            yield return obj;
        }
        else
        {
          for (int i = 0; i < a.Length; ++i)
          {
            var obj1 = a[i];
            if (obj1 != null)
            {
              var obj2 = obj1 as T;
              if (obj2 != null)
                yield return obj2;
            }
            else
              break;
          }
        }
        a = null;
      }
    }

    /// <summary>
    /// Entfernt die Anmerkungen vom angegebenen Typ aus diesem <see cref="T:System.Xml.Linq.XObject"/>.
    /// </summary>
    /// <param name="type">Der <see cref="T:System.Type"/> der zu entfernenden Anmerkungen.</param>
    public void RemoveAnnotations(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      if (annotations == null)
        return;
      var objArray = annotations as object[];
      if (objArray == null)
      {
        if (!type.IsInstanceOfType(annotations))
          return;
        annotations = null;
      }
      else
      {
        int index = 0;
        int num = 0;
        for (; index < objArray.Length; ++index)
        {
          var o = objArray[index];
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
          annotations = null;
        }
        else
        {
          while (num < index)
            objArray[num++] = null;
        }
      }
    }

    /// <summary>
    /// Entfernt die Anmerkungen vom angegebenen Typ aus diesem <see cref="T:System.Xml.Linq.XObject"/>.
    /// </summary>
    /// <typeparam name="T">Der Typ der zu entfernenden Anmerkungen.</typeparam>
    public void RemoveAnnotations<T>() where T : class
    {
      if (annotations == null)
        return;
      var objArray = annotations as object[];
      if (objArray == null)
      {
        if (!(annotations is T))
          return;
        annotations = null;
      }
      else
      {
        int index = 0;
        int num = 0;
        for (; index < objArray.Length; ++index)
        {
          var obj = objArray[index];
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
          annotations = null;
        }
        else
        {
          while (num < index)
            objArray[num++] = null;
        }
      }
    }

    internal void NotifyChanged(object sender, X_ObjectChangeEventArgs e)
    {
      var xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (X_Object)xobject.parent)
        {
          if (xobject == null)
            return;
          var changeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
          if (changeAnnotation != null)
          {
            if (changeAnnotation.changed != null)
              changeAnnotation.changed(sender, e);
          }
        }
        xobject = xobject.parent;
      }
    }

    internal bool NotifyChanging(object sender, X_ObjectChangeEventArgs e)
    {
      bool flag = false;
      var xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (X_Object)xobject.parent)
        {
          if (xobject == null)
            return flag;
          var changeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
          if (changeAnnotation != null)
          {
            flag = true;
            if (changeAnnotation.changing != null)
              changeAnnotation.changing(sender, e);
          }
        }
        xobject = xobject.parent;
      }
    }

    internal void SetBaseUri(string baseUri)
    {
      AddAnnotation(new BaseUriAnnotation(baseUri));
    }

    internal bool SkipNotify()
    {
      var xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (X_Object)xobject.parent)
        {
          if (xobject == null)
            return true;
          if (xobject.Annotations<XObjectChangeAnnotation>() != null)
            return false;
        }
        xobject = xobject.parent;
      }
    }

    internal SaveOptions GetSaveOptionsFromAnnotations()
    {
      var xobject = this;
      while (true)
      {
        for (; xobject == null || xobject.annotations != null; xobject = (X_Object)xobject.parent)
        {
          if (xobject == null)
            return SaveOptions.None;
          var obj = xobject.Annotation(typeof(SaveOptions));
          if (obj != null)
            return (SaveOptions)obj;
        }
        xobject = xobject.parent;
      }
    }
  }
}
