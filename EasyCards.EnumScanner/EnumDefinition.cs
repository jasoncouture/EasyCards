#pragma warning disable SYSLIB1037
public sealed record EnumDefinition(string Name, string BaseType, bool Flags, EnumMemberDefinition[] Members);
