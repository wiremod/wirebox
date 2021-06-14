rmdir /q /s ..\..\..\models\wirebox
xcopy /E /Y models\wirebox\ ..\..\..\models\wirebox\
rmdir /q /s ..\..\..\particles\wirebox
xcopy /E /Y particles\wirebox\ ..\..\..\particles\wirebox\
rmdir /q /s ..\..\..\code\ui\wirebox\
xcopy /E /Y code\ui\wirebox\ ..\..\..\code\ui\wirebox\
