namespace Tobey.Sprintaholic;

internal static class Definitions
{
    public enum SprintMode
    {
        Hold,
        Toggle,
        Always,
    }

    public static readonly ConfigDefinition<SprintMode> SprintControlMode = new(
        Definition: new("Controls", "Sprint control mode"),
        Description: new("Which sprint control mode to use."),
        DefaultValue: SprintMode.Toggle);

    public static readonly ConfigDefinition<bool> AutoDisableSprint = new(
        Definition: new("Controls", "Auto disable sprint"),
        Description: new("When Sprint control mode is Toggle, whether to automatically to stop sprinting when you stop moving."),
        DefaultValue: true);

    public static readonly ConfigDefinition<float> SpeedMultiplier = new(
        Definition: new("Movement", "Speed multiplier"),
        Description: new("Walk and sprint speed will be multiplied by this number."),
        DefaultValue: 1f);

    public static readonly ConfigDefinition<object> WalkSpeed = new(
       Definition: new("Movement", "Walk speed"),
       Description: new("Walk speed of the character. Ignored when Sprint control mode is Always."));

    public static readonly ConfigDefinition<object> SprintSpeed = new(
        Definition: new("Movement", "Sprint speed"),
        Description: new("Sprint speed of the character."));
}
