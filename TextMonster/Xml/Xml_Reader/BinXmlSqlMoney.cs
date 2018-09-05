using System;
using System.Globalization;

namespace TextMonster.Xml.Xml_Reader
{
  internal struct BinXmlSqlMoney
  {
    long data;

    public BinXmlSqlMoney(int v) { this.data = v; }
    public BinXmlSqlMoney(long v) { this.data = v; }

    public Decimal ToDecimal()
    {
      bool neg;
      ulong v;
      if (this.data < 0)
      {
        neg = true;
        v = (ulong)unchecked(-this.data);
      }
      else
      {
        neg = false;
        v = (ulong)this.data;
      }
      // SQL Server stores money8 as ticks of 1/10000.
      const byte MoneyScale = 4;
      return new Decimal(unchecked((int)v), unchecked((int)(v >> 32)), 0, neg, MoneyScale);
    }

    public override String ToString()
    {
      Decimal money = ToDecimal();
      // Formatting of SqlMoney: At least two digits after decimal point
      return money.ToString("#0.00##", CultureInfo.InvariantCulture);
    }
  }
}