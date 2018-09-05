using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_fixed : Datatype_decimal
  {
    public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
    {
      Exception exception;

      try
      {
        Numeric10FacetsChecker facetsChecker = this.FacetsChecker as Numeric10FacetsChecker;
        decimal value = XmlConvert.ToDecimal(s);
        exception = facetsChecker.CheckTotalAndFractionDigits(value, 14 + 4, 4, true, true);
        if (exception != null) goto Error;

        return value;
      }
      catch (XmlSchemaException e)
      {
        throw e;
      }
      catch (Exception e)
      {
        throw new XmlSchemaException(Res.GetString(Res.Sch_InvalidValue, s), e);
      }
    Error:
      throw exception;
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      decimal decimalValue;
      exception = XmlConvert.TryToDecimal(s, out decimalValue);
      if (exception != null) goto Error;

      Numeric10FacetsChecker facetsChecker = this.FacetsChecker as Numeric10FacetsChecker;
      exception = facetsChecker.CheckTotalAndFractionDigits(decimalValue, 14 + 4, 4, true, true);
      if (exception != null) goto Error;

      typedValue = decimalValue;

      return null;

    Error:
      return exception;
    }
  }
}
