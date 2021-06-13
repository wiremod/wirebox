# Wiremod for S&box

## Current status

Very early WIP (as is S&box itself).

- Wiring Tool is equivalent to G-Wiremod's non-Adv version, with basic Debugger and Gate spawning functionalities too
- Outputs: Buttons and a Wire Keyboard (effectively a Pod Controller with a builtin vehicle)
- Inputs: Thrusters, Lights
- Gates: the basic arithmetic/logic

## Setup

S&box is currently focused around 'Gamemodes', not modular addons,
however [[A]lex is working on a modular Gamemode framework](https://github.com/Ceveos/minimal-extended) which might work for us.
Unfortunately, the base 'Sandbox' gamemode needs to be modified to be more extendable, so grabbing the latest Github Release with those changes already applied is easiest.

### Using the latest Github Release

1. Download it to `steamapps/common/sbox/addons/`
2. `cd wirebox/addons/code/wirebox && git pull`


### Building from scratch

1. Start with `minimal-extended` modular addon framework:
```
cd steamapps/common/sbox/addons
git clone https://github.com/Ceveos/minimal-extended.git
```
2. Clone Wirebox on top:
```
cd minimal-extended/code/addons
git clone https://github.com/wiremod/wirebox.git
cd wirebox
./asset-copier.bat
```
3. Apply Nebual's latest Sandbox extensions (todo: track in Git somehow)


## Developing

Hotreloading is neat, and generally works well - limitations include:
- Base UI, such as the Spawnmenu's tools/spawnlists, don't reload without a map change
- seems a bit less reliable for non-host players, though often works
- some Model properties don't reload, such as Prop breakability
- You'll need `./asset-watcher.ps1` running for assets/ui

### Assets Watcher
Some files need to be in the "root directory" of the gamemode (eg. `sbox/addons/minimal-extended/models/`).
`./asset-copier.bat` will delete old and copy in new files, or `./asset-watcher.ps1` will actively watch for new changes and copy them in as they occur.
