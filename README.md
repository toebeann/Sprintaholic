![Sprintaholic logo](assets/logo.png)

# Sprintaholic 🏃‍➡️

- Gotta go fast!
- Turn your sprint key into a toggle so you can run indefinitely!
- Customise your movement speed and zoom to your heart's content!
- Optional premade config files to suit your preferences!
- Compatible with popular in-game Config Manager GUIs!
- And more!

| Compatibility           | Platform  | Tested game version |
| ----------------------- | --------- | ------------------- |
| Megastore Simulator     | Steam     | v0.1.5              |
| Supermarket Simulator\* | Steam     | v1.1.7              |
|                         | Game Pass | v1.1.9              |

\* I have not tested multiplayer - if you use it in multiplayer, please [get in touch](#need-help) letting me know how it goes!

## Installation

Extremely easy! Just follow these 3 simple steps:

1. If you haven't already, install Tobey's BepInEx Pack for your game - make sure to read the Quick Start for easy instructions:
    - [Megastore Simulator](https://github.com/toebeann/BepInEx.MegastoreSimulator#quick-start)
    - [Supermarket Simulator](https://github.com/toebeann/BepInEx.SupermarketSimulator#quick-start)
2. [Download the latest release of Sprintaholic from the releases page](https://github.com/toebeann/Sprintaholic/releases). Make sure to grab the .zip for your game!
3. Extract the `BepInEx` folder from the downloaded Sprintaholic zip file into your game folder - an easy way to do this is simply double-click on the zip file and then drag the `BepInEx` folder out into your game folder

That's all there is to it!

## Customisation

The default Sprintaholic settings convert sprint into a toggle which automatically disables whenever you stop moving. However, there are many more settings avaialble, enabling you to fully customise your sprint addiction! See the below headings for info on available settings.

To save you the hassle of fiddling with settings, I have created several config presets for various use cases, available exclusively from the "Optional files" section of Sprintaholic's Nexus Mods page(s), so make sure to check them out:

- [Megastore Simulator](https://www.nexusmods.com/megastoresimulator/mods/8?tab=files)
- [Supermarket Simualtor](https://www.nexusmods.com/supermarketsimulator/mods/792?tab=files)

Some examples of available config presets:
- **Tobey's choice** - My personal settings! You sprint by default and hold sprint to walk, with a slight boost to your movement speed. Additionally for Megastore Simulator players, head bobbing and movement acceleration/deceleration are disabled, for a smooth sprint sonata.
- **Run forever** - For those of you wondering "why would I ever walk?"
- **Gotta go fast!** - This one is probably _too_ fast for most... Except for perhaps a certain blue hedgehog...

If further customisation is desired, you can use the following tools to customise Sprintaholic in-game:

- [Configuration Manager for MegaStore Simulator](https://www.nexusmods.com/megastoresimulator/mods/7)
- [BepInEx Config Manager for Supermarket Simulator](https://www.nexusmods.com/supermarketsimulator/mods/1396)

Alternatively, you can edit your config file manually:

1. Make sure to run the game once with Sprintaholic installed to generate the config file
2. Find the config file at the location: `BepInEx` > `config` > `Tobey.Sprintaholic.SupermarketSimulator.cfg`
3. Open it in a text editor of your choice such as Notepad, Visual Studio Code, etc.
4. Set the values as desired, making sure to read the comments (the lines beginning with `#`)
5. Reload the game for your changes to take effect

> [!NOTE]
> Some default config entries will only be generated once you have loaded into a save.

### Controls

- **Sprint control mode** (default `Toggle`) - Which sprint control mode to use. Can be one of:
    - `Hold`: Like normal, you hold the sprint button to sprint.
    - `Toggle`: Toggle sprinting on and off whenever you press the sprint button.
    - `Always`: Run forever!
- **Auto disable sprint** (default `true`) - When `Sprint control mode` is `Toggle`, whether to automatically stop sprinting when you stop moving.
- **Sprint by default** (default `false`) - When enabled, you sprint by default and walk when sprint is activated.

### Movement

- **Speed multiplier** (default `1`) - Walk and sprint speed will be multiplied by this number.
- **Walk speed** (default parsed from game at runtime) - Walk speed of the character.
- **Sprint sprint** (default parsed from game at runtime) - Sprint speed of the character.

#### Megastore Simulator

_The following options are only available in Megastore Simulator!_

- **Instant acceleration** (default `false`) - Disables the acceleration/deceleration of movement, so that you always move at maximum speed.
- **Disable head bobbing** (default `false`).

## Need help?

You can use the following channels to ask for help:

- Megastore Simulator:
    - [Modded Megastore Simulator Discord](https://discord.gg/9KrRZx7akG)
    - [Nexus Mods posts tab](https://www.nexusmods.com/megastoresimulator/mods/8?tab=posts)
- Supermarket Simulator:
    - [Modded Supermarket Simulator Discord](https://discord.gg/hjGpjB3GXA)
    - [Nexus Mods posts tab](https://www.nexusmods.com/supermarketsimulator/mods/792?tab=posts)
- [GitHub issues](https://github.com/toebeann/Sprintaholic/issues)

## Additional Credits

"sprint" icon by Adrien Coquet from [Noun Project](https://thenounproject.com/browse/icons/term/sprint/) (CC BY 3.0)
