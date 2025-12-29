@echo off
echo Building 7D2D ESP DLL...
echo.

REM Create output directory
if not exist "bin\Release" mkdir "bin\Release"

REM Try to compile with csc.exe if available
set CSC_PATH=
for /f "tokens=*" %%i in ('where csc 2^>nul') do set CSC_PATH=%%i

if defined CSC_PATH (
    echo Found C# compiler at: %CSC_PATH%
    
    REM Compile with all required references
    echo Compiling with CSC...
    "%CSC_PATH%" /target:library /out:"bin\Release\Game_7D2D.dll" /reference:"Assm\UnityEngine.CoreModule.dll" /reference:"Assm\UnityEngine.dll" /reference:"Assm\UnityEngine.UIModule.dll" /reference:"Assm\UnityEngine.IMGUIModule.dll" /reference:"Assm\UnityEngine.InputModule.dll" /reference:"Assm\Assembly-CSharp.dll" /reference:"Assm\Assembly-CSharp-firstpass.dll" /reference:"Assm\UnityEngine.ImageConversionModule.dll" /reference:"Assm\Unity.Postprocessing.Runtime.dll" /optimize /unsafe /nologo /nowarn:0618 Class1.cs Hacks.cs Render.cs Modules\ESP.cs Modules\UI.cs Modules\Hotkeys.cs
    
    if %ERRORLEVEL% EQU 0 (
        echo Compilation SUCCESSFUL!
        echo Output: bin\Release\Game_7D2D.dll
        dir "bin\Release\Game_7D2D.dll"
    ) else (
        echo Compilation FAILED with CSC
    )
) else (
    echo C# compiler not found. Trying alternative methods...
    
    REM Try using dotnet if available
    dotnet --version >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo Using dotnet build...
        dotnet build 7D2D.csproj --configuration Release --verbosity minimal
    ) else (
        echo ERROR: No C# compiler found!
        echo Please install Visual Studio or .NET SDK.
    )
)

echo.
pause
