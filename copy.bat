@echo off
SETLOCAL EnableDelayedExpansion
chcp 65001 >nul

set /p commit="Enter commit: "
cd /d D:\Tools\batch\manager
git add .
git commit -m "%commit:"=%"
git push -u origin master

set fileLoc=D:\Tools\batch\manager\bin\Debug\WindowsFormsApplication1.exe
set fileName=AV管家.exe
copy %fileLoc% D:\Back\New\%fileName% /y>nul
echo.
echo D:\Back\New\%fileName% copied
echo.
copy %fileLoc% D:\Back\%fileName% /y>nul
echo D:\Back\%fileName% copied

for %%p in (E F G H I J K L M N O P Q R S T U V W X Y Z) do (
	if exist %%p:\%fileName% (
		echo.
		echo %%p:\%fileName% copied
		copy %fileLoc% %%p:\%fileName% /Y>nul
	)
)
echo.
pause