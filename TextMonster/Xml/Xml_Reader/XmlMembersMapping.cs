using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlMembersMapping.uex' path='docs/doc[@for="XmlMembersMapping"]/*' />
  ///<internalonly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlMembersMapping : XmlMapping
  {
    XmlMemberMapping[] mappings;

    internal XmlMembersMapping(TypeScope scope, ElementAccessor accessor, XmlMappingAccess access)
      : base(scope, accessor, access)
    {
      MembersMapping mapping = (MembersMapping)accessor.Mapping;
      StringBuilder key = new StringBuilder();
      key.Append(":");
      mappings = new XmlMemberMapping[mapping.Members.Length];
      for (int i = 0; i < mappings.Length; i++)
      {
        if (mapping.Members[i].TypeDesc.Type != null)
        {
          key.Append(GenerateKey(mapping.Members[i].TypeDesc.Type, null, null));
          key.Append(":");
        }
        mappings[i] = new XmlMemberMapping(mapping.Members[i]);
      }
      SetKeyInternal(key.ToString());
    }
  }
}