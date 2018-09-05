using System;
using System.Collections;
using System.Globalization;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class XPathComparerHelper : IComparer
  {
    private XmlSortOrder order;
    private XmlCaseOrder caseOrder;
    private CultureInfo cinfo;
    private XmlDataType dataType;

    public XPathComparerHelper(XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
    {
      if (lang == null)
      {
        this.cinfo = System.Threading.Thread.CurrentThread.CurrentCulture;
      }
      else
      {
        try
        {
          this.cinfo = new CultureInfo(lang);
        }
        catch (System.ArgumentException)
        {
          throw;  // Throwing an XsltException would be a breaking change
        }
      }

      if (order == XmlSortOrder.Descending)
      {
        if (caseOrder == XmlCaseOrder.LowerFirst)
        {
          caseOrder = XmlCaseOrder.UpperFirst;
        }
        else if (caseOrder == XmlCaseOrder.UpperFirst)
        {
          caseOrder = XmlCaseOrder.LowerFirst;
        }
      }

      this.order = order;
      this.caseOrder = caseOrder;
      this.dataType = dataType;
    }

    public int Compare(object x, object y)
    {
      switch (this.dataType)
      {
        case XmlDataType.Text:
          string s1 = Convert.ToString(x, this.cinfo);
          string s2 = Convert.ToString(y, this.cinfo);
          int result = string.Compare(s1, s2, /*ignoreCase:*/ this.caseOrder != XmlCaseOrder.None, this.cinfo);

          if (result != 0 || this.caseOrder == XmlCaseOrder.None)
            return (this.order == XmlSortOrder.Ascending) ? result : -result;

          // If we came this far, it means that strings s1 and s2 are
          // equal to each other when case is ignored. Now it's time to check
          // and see if they differ in case only and take into account the user
          // requested case order for sorting purposes.
          result = string.Compare(s1, s2, /*ignoreCase:*/ false, this.cinfo);
          return (this.caseOrder == XmlCaseOrder.LowerFirst) ? result : -result;

        case XmlDataType.Number:
          double r1 = XmlConvert.ToXPathDouble(x);
          double r2 = XmlConvert.ToXPathDouble(y);
          result = r1.CompareTo(r2);
          return (this.order == XmlSortOrder.Ascending) ? result : -result;

        default:
          // dataType doesn't support any other value
          throw new InvalidOperationException(Res.GetString(Res.Xml_InvalidOperation));
      }
    } // Compare ()
  }
}