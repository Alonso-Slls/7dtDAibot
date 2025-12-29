@echo off
echo Compiling 7D2D ESP with Windows C# Compiler (.NET Framework 4.8 + Harmony)...
echo.

REM Find Windows C# compiler
set CSC_PATH=
for /f "tokens=*" %%i in ('where csc.exe 2^>nul') do set CSC_PATH=%%i

if defined CSC_PATH (
    echo Found C# compiler at: %CSC_PATH%
    
    REM Clean previous build
    if exist "bin\Release\Game_7D2D.dll" del "bin\Release\Game_7D2D.dll"
    if exist "bin\Release\Game_7D2D.pdb" del "bin\Release\Game_7D2D.pdb"
    
    REM Compile with all Unity references including Harmony
    echo Compiling...
    "%CSC_PATH%" /target:library /out:"bin\Release\Game_7D2D.dll" /reference:"Assm\UnityEngine.CoreModule.dll" /reference:"Assm\UnityEngine.dll" /reference:"Assm\UnityEngine.UIModule.dll" /reference:"Assm\UnityEngine.IMGUIModule.dll" /reference:"Assm\UnityEngine.InputModule.dll" /reference:"Assm\UnityEngine.InputLegacyModule.dll" /reference:"Assm\UnityEngine.ImageConversionModule.dll" /reference:"Assm\Assembly-CSharp.dll" /reference:"Assm\Assembly-CSharp-firstpass.dll" /reference:"Assm\Unity.Postprocessing.Runtime.dll" /reference:"Assm\0Harmony.dll" /optimize /unsafe /nologo /nowarn:0618 /debug:pdbonly /define:TRACE /langversion:latest Class1.cs Hacks.cs Render.cs Modules\ESP.cs Modules\UI.cs Modules\Hotkeys.cs Properties\AssemblyInfo.cs
    
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo COMPILATION SUCCESSFUL!
        if exist "bin\Release\Game_7D2D.dll" (
            echo Output DLL: bin\Release\Game_7D2D.dll
            dir "bin\Release\Game_7D2D.dll"
            echo.
            echo Ready for injection with MonoSharpInjector:
            echo   Namespace: Game_7D2D
            echo   Class: Loader
            echo   Method: init
        )
    ) else (
        echo COMPILATION FAILED with error code %ERRORLEVEL%
        echo Check error messages above for details.
    )
) else (
    echo ERROR: C# compiler not found!
    echo.
    echo To compile this project, you need:
    echo 1. Visual Studio 2019/2022 with .NET Framework development tools
    echo 2. OR .NET Framework 4.8 Developer Pack
    echo 3. OR Build Tools for Visual Studio
    echo.
    echo Download from: https://dotnet.microsoft.com/download/dotnet-framework/net48
)

echo.
pause
