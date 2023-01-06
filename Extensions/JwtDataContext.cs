using System.Text.Json.Serialization;
using dofdir_komek.Utils;

namespace dofdir_komek.Extensions;

[JsonSerializable(typeof(JwtService.JwtData))]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public sealed partial class JwtDataContext: JsonSerializerContext 
{}