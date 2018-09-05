using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlAnyListConverter : XmlListConverter
  {
    protected XmlAnyListConverter(XmlBaseConverter atomicConverter)
      : base(atomicConverter)
    {
    }

    public static readonly XmlValueConverter ItemList = new XmlAnyListConverter((XmlBaseConverter)XmlAnyConverter.Item);
    public static readonly XmlValueConverter AnyAtomicList = new XmlAnyListConverter((XmlBaseConverter)XmlAnyConverter.AnyAtomic);


    //-----------------------------------------------
    // ChangeType
    //-----------------------------------------------

    public override object ChangeType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null) throw new ArgumentNullException("value");
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      // If source value does not implement IEnumerable, or it is a string or byte[],
      if (!(value is IEnumerable) || value.GetType() == StringType || value.GetType() == ByteArrayType)
      {
        // Then create a list from it
        value = new object[] { value };
      }

      return ChangeListType(value, destinationType, nsResolver);
    }
  }
}
