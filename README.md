# Wiremod for S&box

## Current status

Very early WIP (as is S&box itself). There's basic wiring of thrusters/lights to buttons.  
Players/Testers: there's nothing to do yet

## Setup

S&box is currently focused around 'Gamemodes', not modular addons,
however [[A]lex is working on a modular Gamemode framework](https://github.com/Ceveos/minimal-extended) which might work for us. There's thus two install approaches:

### Using minimal-extended modular gamemode framework
```
cd steamapps/common/sbox/addons
git clone https://github.com/Ceveos/minimal-extended.git
cd minimal-extended/code/addons
git clone https://github.com/wiremod/wirebox.git
cd wirebox
./unpack-assets.bat
```

Alternatively, use the latest Github Release as a base

### Copying overtop of the base [sandbox gamemode](https://github.com/facepunch/sandbox)

1. Load in-game or `git clone https://github.com/facepunch/sandbox.git` to `sbox/addons/`
2. Copy this repo on-top
