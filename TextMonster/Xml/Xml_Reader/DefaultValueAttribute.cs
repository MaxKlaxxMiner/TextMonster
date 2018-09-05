using System;
using System.ComponentModel;

namespace TextMonster.Xml.Xml_Reader
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
  [AttributeUsage(AttributeTargets.All)]
  public class DefaultValueAttribute : Attribute
  {
    /// <devdoc>
    ///     This is the default value.
    /// </devdoc>
    private object value;

    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class, converting the
    ///    specified value to the
    ///    specified type, and using the U.S. English culture as the
    ///    translation
    ///    context.</para>
    /// </devdoc>
    public DefaultValueAttribute(Type type, string value)
    {

      // The try/catch here is because attributes should never throw exceptions.  We would fail to
      // load an otherwise normal class.
      try
      {
        this.value = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
      }
      catch
      {
      }
    }

    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a Unicode
    ///    character.</para>
    /// </devdoc>
    public DefaultValueAttribute(char value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using an 8-bit unsigned
    ///    integer.</para>
    /// </devdoc>
    public DefaultValueAttribute(byte value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a 16-bit signed
    ///    integer.</para>
    /// </devdoc>
    public DefaultValueAttribute(short value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a 32-bit signed
    ///    integer.</para>
    /// </devdoc>
    public DefaultValueAttribute(int value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a 64-bit signed
    ///    integer.</para>
    /// </devdoc>
    public DefaultValueAttribute(long value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a
    ///    single-precision floating point
    ///    number.</para>
    /// </devdoc>
    public DefaultValueAttribute(float value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a
    ///    double-precision floating point
    ///    number.</para>
    /// </devdoc>
    public DefaultValueAttribute(double value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a <see cref='System.Boolean'/>
    /// value.</para>
    /// </devdoc>
    public DefaultValueAttribute(bool value)
    {
      this.value = value;
    }
    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/> class using a <see cref='System.String'/>.</para>
    /// </devdoc>
    public DefaultValueAttribute(string value)
    {
      this.value = value;
    }

    /// <devdoc>
    /// <para>Initializes a new instance of the <see cref='System.ComponentModel.DefaultValueAttribute'/>
    /// class.</para>
    /// </devdoc>
    public DefaultValueAttribute(object value)
    {
      this.value = value;
    }

    /// <devdoc>
    ///    <para>
    ///       Gets the default value of the property this
    ///       attribute is
    ///       bound to.
    ///    </para>
    /// </devdoc>
    public virtual object Value
    {
      get
      {
        return value;
      }
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
      {
        return true;
      }

      DefaultValueAttribute other = obj as DefaultValueAttribute;

      if (other != null)
      {
        if (Value != null)
        {
          return Value.Equals(other.Value);
        }
        else
        {
          return (other.Value == null);
        }
      }
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    protected void SetValue(object value)
    {
      this.value = value;
    }
  }
}