using BepInEx.Configuration;

namespace Tobey.Sprintaholic;

internal record ConfigDefinition<T>(ConfigDefinition Definition, ConfigDescription Description = null, T DefaultValue = default);
