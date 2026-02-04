using BepInEx.Configuration;

namespace Tobey.Sprintaholic.Config;

public record ConfigDefinition<T>(ConfigDefinition Definition, ConfigDescription Description, T DefaultValue = default);
