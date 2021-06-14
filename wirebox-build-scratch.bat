REM This will produce a wirebox-gamemode, which can be thrown in
REM sbox/addons/

git clone https://github.com/Ceveos/minimal-extended.git wirebox-gamemode
cd wirebox-gamemode/code/addons
rmdir /q /s sandbox

git clone https://github.com/Nebual/sandbox-plus.git
cd sandbox-plus
call asset-copier.bat
cd ..

git clone https://github.com/wiremod/wirebox.git
cd wirebox
call asset-copier.bat
cd ..
