using BepInEx.Configuration;

namespace Tobey.Sprintaholic.Config;

internal class ConfigDefinition<T>(ConfigDefinition definition, ConfigDescription description, T defaultValue = default)
{
    public readonly ConfigDefinition Definition = definition;
    public readonly ConfigDescription Description = description;
    public readonly T DefaultValue = defaultValue;
}

