namespace Tobey.Sprintaholic.Config;

internal static class Definitions
{
    public static readonly ConfigDefinition<bool> HoldToSprint = new(
        definition: new("Controls", "Hold to sprint"),
        description: new("When enabled, hold the sprint keybind to keep sprinting. When disabled, pressing the sprint keybind will cause the player to continue sprinting until they press the sprint keybind again."));

    public static readonly ConfigDefinition<bool> AutoDisableSprint = new(
        definition: new("Controls", "Auto disable sprint"),
        description: new("When enabled, sprint will automatically be toggled off when you stop moving. Ignored when Hold to sprint is enabled."),
        defaultValue: true);

    public static readonly ConfigDefinition<float> SpeedMultiplier = new(
        definition: new("Movement", "Speed multiplier"),
        description: new("Walk and sprint speed will be multiplied by this number"),
        defaultValue: 1f);

    public static readonly ConfigDefinition<object> WalkSpeed = new(
       definition: new("Movement", "Walk speed"),
       description: new("Walk speed of the character"));

    public static readonly ConfigDefinition<object> SprintSpeed = new(
        definition: new("Movement", "Sprint speed"),
        description: new("Sprint speed of the character"));
}
