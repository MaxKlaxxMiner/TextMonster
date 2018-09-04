namespace TextMonster.Xml.XmlReader
{
  internal sealed class ValidationState
  {

    public bool IsNill;
    public bool IsDefault;
    public bool NeedValidateChildren;  // whether need to validate the children of this element   
    public bool CheckRequiredAttribute; //PSVI
    public bool ValidationSkipped;
    public int Depth;         // The validation state  
    public XmlSchemaContentProcessing ProcessContents;
    public XmlSchemaValidity Validity;
    public SchemaElementDecl ElementDecl;            // ElementDecl
    public SchemaElementDecl ElementDeclBeforeXsi; //elementDecl before its changed by that of xsi:type's
    public string LocalName;
    public string Namespace;
    public ConstraintStruct[] Constr;

    public StateUnion CurrentState;

    //For content model validation
    public bool HasMatched;       // whether the element has been verified correctly

    //For NFAs
    public BitSet[] CurPos = new BitSet[2];

    //For all
    public BitSet AllElementsSet;

    //For MinMaxNFA
    public List<RangePositionInfo> RunningPositions;
    public bool TooComplex;
  };
}
