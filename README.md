![Sprintaholic logo](assets/logo.png)

# Sprintaholic 🏃‍➡️

- Gotta go fast!
- Turn your sprint key into a toggle so you can run indefinitely!
- Customise your movement speed and zoom to your heart's content!
- Optional premade config files to suit your preferences!

Tested compatible with Supermarket Simulator v1.1.7 on both Steam and PC Game Pass.\
Has not been tested in multiplayer - if you use it in multiplayer, please [get in touch](#need-help) letting me know how it goes!

## Installation

Extremely easy! Just follow these 3 simple steps:

1. If you haven't already, install [Tobey's BepInEx Pack for Supermarket Simulator](https://github.com/toebeann/BepInEx.SupermarketSimulator) (make sure to read the [quick start](https://github.com/toebeann/BepInEx.SupermarketSimulator#quick-start), I promise it's easy!)
2. [Download the latest release of Sprintaholic from the releases page](https://github.com/toebeann/Sprintaholic/releases/latest/download/Tobey.Sprintaholic.SupermarketSimulator.zip)
3. Extract the `BepInEx` folder from the downloaded Sprintaholic zip file into your game folder - an easy way to do this is simply double-click on the zip file and then drag the `BepInEx` folder out into your game folder

That's all there is to it!

## Customisation

The default settings of Sprintaholic offer a similar experience to playing with a gamepad - the sprint keybind becomes a toggle which is automatically disabled whenever you stop moving. However, there are several settings that you can tweak, enabling you to fully customise your sprint addiction.

To save you some work, I have created some config presets for various use cases, available exclusively in the [optional files](https://www.nexusmods.com/supermarketsimulator/mods/792?tab=files) section of [Sprintaholic's Nexus Mods page](https://www.nexusmods.com/supermarketsimulator/mods/792), so make sure to check them out. My personal favourite preset is **Inverted movement speed**, which has you sprinting by default and holding `shift` to walk.

If further customisation is desired, you can edit the config file to modify various options (see below headings for details):

1. Make sure to run the game once with Sprintaholic installed to generate Sprintaholic's config file
2. Find the config file at the location: `BepInEx` > `config` > `Tobey.Sprintaholic.SupermarketSimulator.cfg`
3. Open it in a text editor of your choice such as Notepad, Visual Studio Code, etc.
4. Set the values as desired, making sure to read the comments (the lines beginning with `#`)
5. Reload the game for your changes to take effect

> [!NOTE]
> Some default config entries will only be generated once you have loaded into a save.

### Controls

- **Hold to sprint** (default `false`) - When enabled, hold the sprint keybind to keep sprinting. When disabled, pressing the sprint keybind will cause the player to continue sprinting until they press the sprint keybind again.
- **Auto disable sprint** (default `true`) - When enabled, sprint will automatically be toggled off when you stop moving. Ignored when **Hold to sprint** is enabled.

### Movement

- **Speed multiplier** (default `1`) - Walk and sprint speed will be multiplied by this number.
- **Walk speed** (default parsed from game at runtime) - Move speed of the character in m/s.
- **Sprint sprint** (default parsed from game at runtime) - Sprint speed of the character in m/s.

## Need help?

You can use the following channels to ask for help:

-   [Modded Supermarket Simulator Discord](https://discord.gg/hjGpjB3GXA)
-   [Nexus Mods posts tab](https://www.nexusmods.com/supermarketsimulator/mods/792?tab=posts)
-   [GitHub issues](https://github.com/toebeann/Sprintaholic/issues)

## Additional Credits

"sprint" icon by Adrien Coquet from [Noun Project](https://thenounproject.com/browse/icons/term/sprint/) (CC BY 3.0)
