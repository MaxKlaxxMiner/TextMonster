using System;
using System.Globalization;

namespace TextMonster.Xml.Xml_Reader
{
  internal class TypedObject
  {

    private class DecimalStruct
    {
      bool isDecimal = false;         // rare case it will be used...
      decimal[] dvalue;               // to accelerate equals operation.  array <-> list

      public bool IsDecimal
      {
        get { return this.isDecimal; }
        set { this.isDecimal = value; }
      }

      public decimal[] Dvalue
      {
        get { return this.dvalue; }
      }

      public DecimalStruct()
      {
        this.dvalue = new decimal[1];
      }
      //list
      public DecimalStruct(int dim)
      {
        this.dvalue = new decimal[dim];
      }
    }

    DecimalStruct dstruct = null;
    object ovalue;
    string svalue;      // only for output
    XmlSchemaDatatype xsdtype;
    int dim = 1;
    bool isList = false;

    public int Dim
    {
      get { return this.dim; }
    }

    public bool IsList
    {
      get { return this.isList; }
    }

    public bool IsDecimal
    {
      get
      {
        return this.dstruct.IsDecimal;
      }
    }
    public decimal[] Dvalue
    {
      get
      {
        return this.dstruct.Dvalue;
      }
    }

    public object Value
    {
      get { return ovalue; }
    }

    public XmlSchemaDatatype Type
    {
      get { return xsdtype; }
    }

    public TypedObject(object obj, string svalue, XmlSchemaDatatype xsdtype)
    {
      this.ovalue = obj;
      this.svalue = svalue;
      this.xsdtype = xsdtype;
      if (xsdtype.Variety == XmlSchemaDatatypeVariety.List ||
          xsdtype is Datatype_base64Binary ||
          xsdtype is Datatype_hexBinary)
      {
        this.isList = true;
        this.dim = ((Array)obj).Length;
      }
    }

    public override string ToString()
    {
      // only for exception
      return this.svalue;
    }

    public void SetDecimal()
    {

      if (this.dstruct != null)
      {
        return;
      }

      // Debug.Assert(!this.IsDecimal);
      switch (xsdtype.TypeCode)
      {
        case XmlTypeCode.Byte:
        case XmlTypeCode.UnsignedByte:
        case XmlTypeCode.Short:
        case XmlTypeCode.UnsignedShort:
        case XmlTypeCode.Int:
        case XmlTypeCode.UnsignedInt:
        case XmlTypeCode.Long:
        case XmlTypeCode.UnsignedLong:
        case XmlTypeCode.Decimal:
        case XmlTypeCode.Integer:
        case XmlTypeCode.PositiveInteger:
        case XmlTypeCode.NonNegativeInteger:
        case XmlTypeCode.NegativeInteger:
        case XmlTypeCode.NonPositiveInteger:

        if (this.isList)
        {
          this.dstruct = new DecimalStruct(this.dim);
          for (int i = 0; i < this.dim; i++)
          {
            this.dstruct.Dvalue[i] = Convert.ToDecimal(((Array)this.ovalue).GetValue(i), NumberFormatInfo.InvariantInfo);
          }
        }
        else
        { //not list
          this.dstruct = new DecimalStruct();
          //possibility of list of length 1.
          this.dstruct.Dvalue[0] = Convert.ToDecimal(this.ovalue, NumberFormatInfo.InvariantInfo);
        }
        this.dstruct.IsDecimal = true;
        break;

        default:
        if (this.isList)
        {
          this.dstruct = new DecimalStruct(this.dim);
        }
        else
        {
          this.dstruct = new DecimalStruct();
        }
        break;

      }
    }

    private bool ListDValueEquals(TypedObject other)
    {
      for (int i = 0; i < this.Dim; i++)
      {
        if (this.Dvalue[i] != other.Dvalue[i])
        {
          return false;
        }
      }
      return true;
    }

    public bool Equals(TypedObject other)
    {
      // ? one is list with one member, another is not list -- still might be equal
      if (this.Dim != other.Dim)
      {
        return false;
      }

      if (this.Type != other.Type)
      {
        //Check if types are comparable
        if (!(this.Type.IsComparable(other.Type)))
        {
          return false;
        }
        other.SetDecimal(); // can't use cast and other.Type.IsEqual (value1, value2)
        this.SetDecimal();
        if (this.IsDecimal && other.IsDecimal)
        { //Both are decimal / derived types 
          return this.ListDValueEquals(other);
        }
      }

      // not-Decimal derivation or type equal
      if (this.IsList)
      {
        if (other.IsList)
        { //Both are lists and values are XmlAtomicValue[] or clrvalue[]. So use Datatype_List.Compare
          return this.Type.Compare(this.Value, other.Value) == 0;
        }
        else
        { //this is a list and other is a single value
          Array arr1 = this.Value as System.Array;
          XmlAtomicValue[] atomicValues1 = arr1 as XmlAtomicValue[];
          if (atomicValues1 != null)
          { // this is a list of union
            return atomicValues1.Length == 1 && atomicValues1.GetValue(0).Equals(other.Value);
          }
          else
          {
            return arr1.Length == 1 && arr1.GetValue(0).Equals(other.Value);
          }
        }
      }
      else if (other.IsList)
      {
        Array arr2 = other.Value as System.Array;
        XmlAtomicValue[] atomicValues2 = arr2 as XmlAtomicValue[];
        if (atomicValues2 != null)
        { // other is a list of union
          return atomicValues2.Length == 1 && atomicValues2.GetValue(0).Equals(this.Value);
        }
        else
        {
          return arr2.Length == 1 && arr2.GetValue(0).Equals(this.Value);
        }
      }
      else
      { //Both are not lists
        return this.Value.Equals(other.Value);
      }
    }
  }
}
