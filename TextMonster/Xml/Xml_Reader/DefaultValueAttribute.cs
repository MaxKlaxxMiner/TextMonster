using System;

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
  }
}