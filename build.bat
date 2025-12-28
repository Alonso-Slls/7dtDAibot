@echo off
setlocal enabledelayedexpansion

REM 7D2D Basic ESP Framework - Windows Batch Script
REM Usage: build.bat [command]

set PROJECT_NAME=7D2D Basic ESP Framework
set DLL_NAME=SevenDtDAibot.dll
set BUILD_DIR=bin\Release

:main
if "%1"=="" goto help
if "%1"=="build" goto build
if "%1"=="clean" goto clean
if "%1"=="rebuild" goto rebuild
if "%1"=="run" goto run
if "%1"=="status" goto status
if "%1"=="help" goto help
goto help

:help
echo %PROJECT_NAME% - Build Script
echo.
echo Usage: %0 [command]
echo.
echo Commands:
echo   build       Build the project in Release mode
echo   clean       Clean build artifacts
echo   rebuild     Clean and rebuild the project
echo   run         Build and prepare for injection
echo   status      Show project status
echo   help        Show this help message
echo.
echo Examples:
echo   %0 build    # Build the project
echo   %0 clean    # Clean build artifacts
echo   %0 run      # Build and prepare for injection
goto end

:check_dotnet
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] dotnet CLI not found. Please install .NET SDK.
    exit /b 1
)
goto :eof

:clean
echo [INFO] Cleaning build artifacts...

if exist bin (
    rmdir /s /q bin
    echo [SUCCESS] Removed bin directory
)

if exist obj (
    rmdir /s /q obj
    echo [SUCCESS] Removed obj directory
)

echo [SUCCESS] Build artifacts cleaned
goto end

:build
echo [INFO] Building %PROJECT_NAME%...

call :check_dotnet

REM Build in Release mode
dotnet build 7D2D.csproj --configuration Release --verbosity minimal

if %errorlevel% equ 0 (
    echo [SUCCESS] Build completed successfully
    
    REM Check if DLL exists
    if exist "%BUILD_DIR%\%DLL_NAME%" (
        for %%A in ("%BUILD_DIR%\%DLL_NAME%") do (
            set size=%%~zA
            set /a sizeKB=!size!/1024
            echo [SUCCESS] DLL created: %BUILD_DIR%\%DLL_NAME% (!sizeKB! KB)
        )
    ) else (
        echo [ERROR] DLL not found at expected location: %BUILD_DIR%\%DLL_NAME%
        exit /b 1
    )
) else (
    echo [ERROR] Build failed
    exit /b 1
)
goto end

:rebuild
echo [INFO] Rebuilding %PROJECT_NAME%...
call :clean
call :build
goto end

:run
echo [INFO] Preparing for game injection...
call :build

if exist "%BUILD_DIR%\%DLL_NAME%" (
    echo [SUCCESS] Ready for injection!
    echo [INFO] DLL location: %BUILD_DIR%\%DLL_NAME%
    echo [INFO] Injection settings:
    echo   Namespace: SevenDtDAibot
    echo   Class: Loader
    echo   Method: init
) else (
    echo [ERROR] DLL not found. Build failed.
    exit /b 1
)
goto end

:status
echo [INFO] %PROJECT_NAME% Status
echo.

REM Git status
if exist .git (
    echo [INFO] Git Status:
    git status --porcelain
    echo.
    
    echo [INFO] Git Branch:
    git branch --show-current
    echo.
    
    echo [INFO] Last Commit:
    git log -1 --oneline
    echo.
) else (
    echo [WARNING] Not a git repository
)

REM Build status
if exist "%BUILD_DIR%\%DLL_NAME%" (
    for %%A in ("%BUILD_DIR%\%DLL_NAME%") do (
        set size=%%~zA
        set /a sizeKB=!size!/1024
        echo [SUCCESS] DLL Status: Built (!sizeKB! KB)
    )
    
    for %%A in ("%BUILD_DIR%\%DLL_NAME%") do (
        echo [INFO] Last Build: %%~tA
    )
) else (
    echo [WARNING] DLL not built
)

REM Project files
echo.
echo [INFO] Project Files:
dir /b *.cs 2>nul

:end
echo.
pause
