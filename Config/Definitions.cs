namespace Tobey.Sprintaholic.Config;

public static class Definitions
{
    public static readonly ConfigDefinition<bool> HoldToSprint = new(
        Definition: new("Controls", "Hold to sprint"),
        Description: new("When enabled, hold the sprint keybind to keep sprinting. When disabled, pressing the sprint keybind will cause the player to continue sprinting until they press the sprint keybind again."));

    public static readonly ConfigDefinition<bool> AutoDisableSprint = new(
        Definition: new("Controls", "Auto disable sprint"),
        Description: new("When enabled, sprint will automatically be toggled off when you stop moving. Ignored when Hold to sprint is enabled."),
        DefaultValue: true);

    public static readonly ConfigDefinition<float> SpeedMultiplier = new(
        Definition: new("Movement", "Speed multiplier"),
        Description: new("Walk and sprint speed will be multiplied by this number"),
        DefaultValue: 1f);

    public static readonly ConfigDefinition<object> WalkSpeed = new(
       Definition: new("Movement", "Walk speed"),
       Description: new("Walk speed of the character"));

    public static readonly ConfigDefinition<object> SprintSpeed = new(
        Definition: new("Movement", "Sprint speed"),
        Description: new("Sprint speed of the character"));
}
