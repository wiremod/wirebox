# Wiremod for S&box

## Current status

Very early WIP (as is S&box itself).

- Wiring Tool is equivalent to G-Wiremod's non-Adv version, with basic Debugger and Gate spawning functionalities too
- Outputs: Buttons, GPS, Gyroscope, and a Wire Keyboard (effectively a Pod Controller with a builtin vehicle)
- Inputs: Thrusters, Lights, Constraint Controllers, HardLight Bridge
- Gates: the basic arithmetic/logic/constant

Message Nebual on Discord with any questions about contributing! See [todos](https://github.com/wiremod/wirebox/projects/1) for ideas.

## Setup

S&box is currently focused around 'Gamemodes', not modular addons,
however [[A]lex is working on a modular Gamemode framework](https://github.com/Ceveos/minimal-extended) which might work for us.
We also use [SandboxPlus](https://github.com/Nebual/sandbox-plus), an extension of FP's Sandbox gamemode, modified to be more extendable.  
Thus, grabbing the latest Github Release with that already setup is easiest.

### Using the latest Github Release

1. Download it to `steamapps/common/sbox/workspace/`
2. `cd workspace/modules/wirebox && git pull`
3. `cd workspace/ && .\watcher.ps1 wirebox -build`


### Building from scratch

1. `cd steamapps/common/sbox/`
2. `git clone https://github.com/Ceveos/minimal-extended.git workspace && cd workspace`
3. `.\watcher.ps1 -create`, say yes to prompt to download SandboxPlus
4. `git clone https://github.com/wiremod/wirebox.git modules/wirebox`
5. `.\watcher.ps1 wirebox -build`

The watcher script will combine all modules/* (so you can throw extra mods there), and output a new "gamemode" into `steamapps/sbox/addons/wirebox/`, where S&Box is looking for it.


## Developing

Often the base Sandbox gamemode isn't extendable enough; direct PR's to enhance flexibility to [SandboxPlus](https://github.com/Nebual/sandbox-plus),
which is wire-agnostic, but obviously quite wire-friendly :D

Hotreloading is neat, and generally works well - limitations include:
- seems a bit less reliable for non-host players, though often works
- some Model properties don't reload, such as Prop breakability
- You'll need `./watcher.ps1 wirebox` running to copy changes from workspace/modules/* to sbox/addons/wirebox (the output gamemode); this lets us keep git repos separate while merging together assets from multiple modules into their respective top-level folders in the resulting gamemde.
