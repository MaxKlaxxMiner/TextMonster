﻿using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public sealed class XmlSchemaObjectCollection : CollectionBase
  {
    XmlSchemaObject parent;

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.XmlSchemaObjectCollection"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSchemaObjectCollection()
    {
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.this"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public XmlSchemaObject this[int index]
    {
      get { return (XmlSchemaObject)List[index]; }
      set { List[index] = value; }
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.GetEnumerator"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public new XmlSchemaObjectEnumerator GetEnumerator()
    {
      return new XmlSchemaObjectEnumerator(InnerList.GetEnumerator());
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.Add"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int Add(XmlSchemaObject item)
    {
      return List.Add(item);
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.Insert"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Insert(int index, XmlSchemaObject item)
    {
      List.Insert(index, item);
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.IndexOf"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public int IndexOf(XmlSchemaObject item)
    {
      return List.IndexOf(item);
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.Contains"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public bool Contains(XmlSchemaObject item)
    {
      return List.Contains(item);
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.Remove"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Remove(XmlSchemaObject item)
    {
      List.Remove(item);
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.CopyTo"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void CopyTo(XmlSchemaObject[] array, int index)
    {
      List.CopyTo(array, index);
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.OnInsert"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected override void OnInsert(int index, object item)
    {
      if (parent != null)
      {
        parent.OnAdd(this, item);
      }
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.OnSet"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected override void OnSet(int index, object oldValue, object newValue)
    {
      if (parent != null)
      {
        parent.OnRemove(this, oldValue);
        parent.OnAdd(this, newValue);
      }
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.OnClear"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected override void OnClear()
    {
      if (parent != null)
      {
        parent.OnClear(this);
      }
    }

    /// <include file='doc\XmlSchemaObjectCollection.uex' path='docs/doc[@for="XmlSchemaObjectCollection.OnRemove"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    protected override void OnRemove(int index, object item)
    {
      if (parent != null)
      {
        parent.OnRemove(this, item);
      }
    }

    internal XmlSchemaObjectCollection Clone()
    {
      XmlSchemaObjectCollection coll = new XmlSchemaObjectCollection();
      coll.Add(this);
      return coll;
    }

    private void Add(XmlSchemaObjectCollection collToAdd)
    {
      this.InnerList.InsertRange(0, collToAdd);
    }
  }
}
