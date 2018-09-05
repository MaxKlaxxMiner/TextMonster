using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  public class SchemaImporterExtensionCollection : CollectionBase
  {
    Hashtable exNames;

    internal Hashtable Names
    {
      get
      {
        if (exNames == null)
          exNames = new Hashtable();
        return exNames;
      }
    }

    internal SchemaImporterExtensionCollection Clone()
    {
      SchemaImporterExtensionCollection clone = new SchemaImporterExtensionCollection();
      clone.exNames = (Hashtable)this.Names.Clone();
      foreach (object o in this.List)
      {
        clone.List.Add(o);
      }
      return clone;
    }

    public SchemaImporterExtension this[int index]
    {
      get { return (SchemaImporterExtension)List[index]; }
      set { List[index] = value; }
    }

    internal int Add(string name, SchemaImporterExtension extension)
    {
      if (Names[name] != null)
      {
        if (Names[name].GetType() != extension.GetType())
        {
          throw new InvalidOperationException(Res.GetString(Res.XmlConfigurationDuplicateExtension, name));
        }
        return -1;
      }
      Names[name] = extension;
      return List.Add(extension);
    }

    public void Insert(int index, SchemaImporterExtension extension)
    {
      List.Insert(index, extension);
    }

    public int IndexOf(SchemaImporterExtension extension)
    {
      return List.IndexOf(extension);
    }

    public bool Contains(SchemaImporterExtension extension)
    {
      return List.Contains(extension);
    }

    public void Remove(SchemaImporterExtension extension)
    {
      List.Remove(extension);
    }

    public void CopyTo(SchemaImporterExtension[] array, int index)
    {
      List.CopyTo(array, index);
    }
  }
}