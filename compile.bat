@echo off
echo Compiling 7D2D ESP DLL...
echo.

REM Try to find Visual Studio MSBuild
set MSBUILD_PATH=
for /f "tokens=*" %%i in ('where msbuild 2^>nul') do set MSBUILD_PATH=%%i

if defined MSBUILD_PATH (
    echo Found MSBuild at: %MSBUILD_PATH%
    %MSBUILD_PATH% 7D2D.sln /p:Configuration=Release /p:Platform="Any CPU"
) else (
    echo MSBuild not found in PATH. Trying Visual Studio locations...
    
    REM Try common Visual Studio locations
    set VS2019_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe
    set VS2019_PATH2=C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe
    set VS2019_PATH3=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
    set VS2022_PATH=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe
    set VS2022_PATH2=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe
    set VS2022_PATH3=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
    
    if exist "%VS2022_PATH%" (
        echo Using VS2022 MSBuild
        "%VS2022_PATH%" 7D2D.sln /p:Configuration=Release /p:Platform="Any CPU"
    ) else if exist "%VS2022_PATH2%" (
        echo Using VS2022 Professional MSBuild
        "%VS2022_PATH2%" 7D2D.sln /p:Configuration=Release /p:Platform="Any CPU"
    ) else if exist "%VS2022_PATH3%" (
        echo Using VS2022 Community MSBuild
        "%VS2022_PATH3%" 7D2D.sln /p:Configuration=Release /p:Platform="Any CPU"
    ) else if exist "%VS2019_PATH%" (
        echo Using VS2019 Enterprise MSBuild
        "%VS2019_PATH%" 7D2D.sln /p:Configuration=Release /p:Platform="Any CPU"
    ) else if exist "%VS2019_PATH2%" (
        echo Using VS2019 Professional MSBuild
        "%VS2019_PATH2%" 7D2D.sln /p:Configuration=Release /p:Platform="Any CPU"
    ) else if exist "%VS2019_PATH3%" (
        echo Using VS2019 Community MSBuild
        "%VS2019_PATH3%" 7D2D.sln /p:Configuration=Release /p:Platform="Any CPU"
    ) else (
        echo ERROR: Visual Studio MSBuild not found!
        echo Please install Visual Studio with .NET Framework development tools.
        pause
        exit /b 1
    )
)

echo.
if %ERRORLEVEL% EQU 0 (
    echo Compilation SUCCESSFUL!
    echo Output DLL should be in: bin\Release\Game_7D2D.dll
    
    if exist "bin\Release\Game_7D2D.dll" (
        echo.
        echo DLL created successfully:
        dir "bin\Release\Game_7D2D.dll"
    ) else (
        echo WARNING: DLL not found in expected location
    )
) else (
    echo Compilation FAILED with error code %ERRORLEVEL%
    echo Please check the error messages above.
)

echo.
pause
