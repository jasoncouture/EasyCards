#pragma warning disable SYSLIB1037
public record EnumDefinition(string Name, string BaseType, bool Flags, EnumMemberDefinition[] Members);