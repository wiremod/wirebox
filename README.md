# Wiremod for S&box

## Current status

Early WIP (as is S&box itself), compiles as of August 2023.

- Wiring Tool is equivalent to G-Wiremod's non-Adv version, with basic Debugger and Gate spawning functionalities too
- Outputs: Buttons, GPS, Gyroscope, Ranger, Speedometer, and a Wire Keyboard (effectively a Pod Controller with a builtin vehicle)
- Inputs: Thrusters, Lights, Constraint Controllers, HardLight Bridge, Number Screen, RT Camera/Screen, Forcer
- Gates: the basic arithmetic/logic/constant

Message Nebual on Discord with any questions about contributing! See [todos](https://github.com/wiremod/wirebox/projects/1) for ideas.

## Setup

Wirebox is an addon intended to be used with on top of [SandboxPlus](https://github.com/Nebual/sandbox-plus), a fork of FP's Sandbox gamemode, modified to be more extendable for addons and have more critical tools and utilities.

You'll likely want to clone both repos, open them both in the same SBox Editor Project + VSCode Workspace, then configure SandboxPlus to load Wirebox as an addon (eg. via the Create Server menu).

```
git clone https://github.com/Nebual/sandbox-plus.git
git clone https://github.com/wiremod/wirebox.git
```

## Developing

Often the base Sandbox gamemode isn't extendable enough; send PR's to enhance flexibility to [SandboxPlus](https://github.com/Nebual/sandbox-plus).

Hotreloading is neat, and generally works well - limitations include:
- seems a bit less reliable for non-host players, though often works
- some Model properties don't reload, such as Prop breakability
- occasionally a hotreload fails and you get error spam about prediction or identical sounding types not being compatible - just restart the map when that happens

# Wirelib

Wirelib is a separate Library addon, intended to be a lightweight set of interfaces (eg. `IWireInputEntity`) for wire-compatible third party addons to implement, without needing to depend on Wirebox directly. This is be used by the Stargate addon to implement optional Wire support, which would have no gameplay impact unless Wirebox was also enabled.
