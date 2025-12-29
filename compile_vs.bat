@echo off
echo Attempting to build with Visual Studio MSBuild...

REM Try different MSBuild paths
set MSBUILD1=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
set MSBUILD2=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
set MSBUILD3=C:\Program Files\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe

if exist "%MSBUILD1%" (
    echo Using VS2022 MSBuild
    "%MSBUILD1%" 7D2D.csproj /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal
) else if exist "%MSBUILD2%" (
    echo Using VS2019 Community MSBuild
    "%MSBUILD2%" 7D2D.csproj /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal
) else if exist "%MSBUILD3%" (
    echo Using VS2019 Enterprise MSBuild
    "%MSBUILD3%" 7D2D.csproj /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal
) else (
    echo ERROR: No MSBuild found!
    echo Please install Visual Studio with .NET Framework development tools.
    pause
    exit /b 1
)

if %ERRORLEVEL% EQU 0 (
    echo.
    echo BUILD SUCCESSFUL!
    if exist "bin\Release\Game_7D2D.dll" (
        echo Output DLL: bin\Release\Game_7D2D.dll
        dir "bin\Release\Game_7D2D.dll"
    )
) else (
    echo BUILD FAILED with error code %ERRORLEVEL%
)

pause
