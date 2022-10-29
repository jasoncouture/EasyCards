namespace EasyCards.EnumGenerator.EnumScannerModels
{

    public class EnumDefinition
    {
        public string Name { get; set; }
        public string BaseType { get; set; }
        public bool Flags { get; set; }
        public EnumMemberDefinition[] Members { get; set; }
    }
}
