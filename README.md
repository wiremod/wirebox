# Wiremod for S&box

## Current status

Broken while we migrate from 2023's Entity system to the new Scene system, and adapt to the removal of what little official addon support existed before.

For the old version using the Entity system, see [2023-entity-system](https://github.com/wiremod/wirebox/tree/2023-entity-system) branch.


Early WIP (as is S&box itself):

- Wiring Tool is equivalent to G-Wiremod's non-Adv version, with basic Debugger and Gate spawning functionalities too
- Outputs: Buttons, GPS, Gyroscope, Ranger, Speedometer, and a Wire Keyboard (effectively a Pod Controller with a builtin vehicle)
- Inputs: Thrusters, Lights, Constraint Controllers, HardLight Bridge, Number Screen, RT Camera/Screen, Forcer
- Gates: the basic arithmetic/logic/constant

Message Nebual on Discord with any questions about contributing! See [todos](https://github.com/wiremod/wirebox/projects/1) for ideas.

## Setup

Wirebox is an addon intended to be used with on top of [SandboxPlus](https://github.com/Nebual/sandbox-plus), a fork of FP's Sandbox gamemode, modified to be more extendable for addons and have more critical tools and utilities.

Still tbd how best to do addons as of the Scene system. For now:

#### Windows
```
# in the root directory of your game project
git submodule add https://github.com/wiremod/wirebox External\wirebox

mklink /J Code\wirebox External\wirebox\Code\wirebox
mklink /J Libraries\WireLib\Code External\wirebox\wirelib\Code
mklink /J Assets\spawnlists\wirebox External\wirebox\Assets\spawnlists
mklink /J Assets\entity\wirebox External\wirebox\Assets\entity\wirebox
mklink /J Assets\materials\wirebox External\wirebox\Assets\materials\wirebox
mklink /J Assets\models\wirebox External\wirebox\Assets\models\wirebox
mklink /J Assets\particles\wirebox External\wirebox\Assets\particles\wirebox
echo External/ >> .gitignore
echo Code/wirebox/ >> .gitignore
echo Assets/*/wirebox/ >> .gitignore
```

#### Linux
Could use submodule + symlinks (which can be committed to git as symlinks)


## Developing

Often the base Sandbox gamemode isn't extendable enough; send PR's to enhance flexibility to [SandboxPlus](https://github.com/Nebual/sandbox-plus).

Hotreloading is neat, and generally works well - limitations include:
- seems a bit less reliable for non-host players, though often works
- some Model properties don't reload, such as Prop breakability
- occasionally a hotreload fails and you get error spam about prediction or identical sounding types not being compatible - just restart the map when that happens

# Wirelib

Wirelib is a separate Library addon, intended to be a lightweight set of interfaces (eg. `IWireInputEntity`) for wire-compatible third party addons to implement, without needing to depend on Wirebox directly. This is be used by the Stargate addon to implement optional Wire support, which would have no gameplay impact unless Wirebox was also enabled.
