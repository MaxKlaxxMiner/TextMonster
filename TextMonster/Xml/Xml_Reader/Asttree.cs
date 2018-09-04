using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  // static, including an array of ForwardAxis  (this is the whole picture)
  internal class Asttree
  {
    // set private then give out only get access, to keep it intact all along
    private ArrayList fAxisArray;
    private string xpathexpr;
    private bool isField;                                   // field or selector
    private XmlNamespaceManager nsmgr;

    internal ArrayList SubtreeArray
    {
      get { return fAxisArray; }
    }

    // when making a new instance for Asttree, we do the compiling, and create the static tree instance
    public Asttree(string xPath, bool isField, XmlNamespaceManager nsmgr)
    {
      this.xpathexpr = xPath;
      this.isField = isField;
      this.nsmgr = nsmgr;
      // checking grammar... and build fAxisArray
      this.CompileXPath(xPath, isField, nsmgr);          // might throw exception in the middle
    }

    // this part is for parsing restricted xpath from grammar
    private static bool IsNameTest(Axis ast)
    {
      // Type = Element, abbrAxis = false
      // all are the same, has child:: or not
      return ((ast.TypeOfAxis == Axis.AxisType.Child) && (ast.NodeType == XPathNodeType.Element));
    }

    internal static bool IsAttribute(Axis ast)
    {
      return ((ast.TypeOfAxis == Axis.AxisType.Attribute) && (ast.NodeType == XPathNodeType.Attribute));
    }

    private static bool IsDescendantOrSelf(Axis ast)
    {
      return ((ast.TypeOfAxis == Axis.AxisType.DescendantOrSelf) && (ast.NodeType == XPathNodeType.All) && (ast.AbbrAxis));
    }

    internal static bool IsSelf(Axis ast)
    {
      return ((ast.TypeOfAxis == Axis.AxisType.Self) && (ast.NodeType == XPathNodeType.All) && (ast.AbbrAxis));
    }

    // don't return true or false, if it's invalid path, just throw exception during the process
    // for whitespace thing, i will directly trim the tree built here...
    public void CompileXPath(string xPath, bool isField, XmlNamespaceManager nsmgr)
    {
      if ((xPath == null) || (xPath.Length == 0))
      {
        throw new XmlSchemaException(Res.Sch_EmptyXPath, string.Empty);
      }

      // firstly i still need to have an ArrayList to store tree only...
      // can't new ForwardAxis right away
      string[] xpath = xPath.Split('|');
      ArrayList AstArray = new ArrayList(xpath.Length);
      this.fAxisArray = new ArrayList(xpath.Length);

      // throw compile exceptions
      // can i only new one builder here then run compile several times??
      try
      {
        for (int i = 0; i < xpath.Length; ++i)
        {
          // default ! isdesorself (no .//)
          Axis ast = (Axis)(XPathParser.ParseXPathExpresion(xpath[i]));
          AstArray.Add(ast);
        }
      }
      catch
      {
        throw new XmlSchemaException(Res.Sch_ICXpathError, xPath);
      }

      Axis stepAst;
      for (int i = 0; i < AstArray.Count; ++i)
      {
        Axis ast = (Axis)AstArray[i];
        // Restricted form
        // field can have an attribute:

        // throw exceptions during casting
        if ((stepAst = ast) == null)
        {
          throw new XmlSchemaException(Res.Sch_ICXpathError, xPath);
        }

        Axis top = stepAst;

        // attribute will have namespace too
        // field can have top attribute
        if (IsAttribute(stepAst))
        {
          if (!isField)
          {
            throw new XmlSchemaException(Res.Sch_SelectorAttr, xPath);
          }
          else
          {
            SetURN(stepAst, nsmgr);
            try
            {
              stepAst = (Axis)(stepAst.Input);
            }
            catch
            {
              throw new XmlSchemaException(Res.Sch_ICXpathError, xPath);
            }
          }
        }

        // field or selector
        while ((stepAst != null) && (IsNameTest(stepAst) || IsSelf(stepAst)))
        {
          // trim tree "." node, if it's not the top one
          if (IsSelf(stepAst) && (ast != stepAst))
          {
            top.Input = stepAst.Input;
          }
          else
          {
            top = stepAst;
            // set the URN
            if (IsNameTest(stepAst))
            {
              SetURN(stepAst, nsmgr);
            }
          }
          try
          {
            stepAst = (Axis)(stepAst.Input);
          }
          catch
          {
            throw new XmlSchemaException(Res.Sch_ICXpathError, xPath);
          }
        }

        // the rest part can only be .// or null
        // trim the rest part, but need compile the rest part first
        top.Input = null;
        if (stepAst == null)
        {      // top "." and has other element beneath, trim this "." node too
          if (IsSelf(ast) && (ast.Input != null))
          {
            this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree((Axis)(ast.Input)), false));
          }
          else
          {
            this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree(ast), false));
          }
          continue;
        }
        if (!IsDescendantOrSelf(stepAst))
        {
          throw new XmlSchemaException(Res.Sch_ICXpathError, xPath);
        }
        try
        {
          stepAst = (Axis)(stepAst.Input);
        }
        catch
        {
          throw new XmlSchemaException(Res.Sch_ICXpathError, xPath);
        }
        if ((stepAst == null) || (!IsSelf(stepAst)) || (stepAst.Input != null))
        {
          throw new XmlSchemaException(Res.Sch_ICXpathError, xPath);
        }

        // trim top "." if it's not the only node
        if (IsSelf(ast) && (ast.Input != null))
        {
          this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree((Axis)(ast.Input)), true));
        }
        else
        {
          this.fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree(ast), true));
        }
      }
    }

    // depending on axis.Name & axis.Prefix, i will set the axis.URN;
    // also, record urn from prefix during this
    // 4 different types of element or attribute (with @ before it) combinations: 
    // (1) a:b (2) b (3) * (4) a:*
    // i will check xpath to be strictly conformed from these forms
    // for (1) & (4) i will have URN set properly
    // for (2) the URN is null
    // for (3) the URN is empty
    private void SetURN(Axis axis, XmlNamespaceManager nsmgr)
    {
      if (axis.Prefix.Length != 0)
      {      // (1) (4)
        axis.Urn = nsmgr.LookupNamespace(axis.Prefix);

        if (axis.Urn == null)
        {
          throw new XmlSchemaException(Res.Sch_UnresolvedPrefix, axis.Prefix);
        }
      }
      else if (axis.Name.Length != 0)
      { // (2)
        axis.Urn = null;
      }
      else
      {                            // (3)
        axis.Urn = "";
      }
    }
  }// Asttree
}
