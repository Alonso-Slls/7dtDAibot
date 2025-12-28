@echo off
setlocal enabledelayedexpansion

REM Log Viewer for 7D2D ESP Framework
REM Usage: viewlogs.bat [logtype]

set LOG_DIR=logs

:main
if "%1"=="" goto help
if "%1"=="main" goto view_main
if "%1"=="error" goto view_error
if "%1"=="perf" goto view_perf
if "%1"="diag" goto view_diag
if "%1"=="all" goto view_all
if "%1"=="help" goto help
goto help

:help
echo 7D2D ESP Framework - Log Viewer
echo.
echo Usage: %0 [logtype]
echo.
echo Log Types:
echo   main    - View main debug log (esp_debug.log)
echo   error   - View error log (esp_errors.log)
echo   perf    - View performance log (esp_performance.log)
echo   diag    - List diagnostic reports
echo   all     - View all logs
echo   help    - Show this help
echo.
echo Examples:
echo   %0 main     # View main debug log
echo   %0 error    # View error log
echo   %0 perf     # View performance log
goto end

:view_main
echo [INFO] Viewing main debug log...
if exist "%LOG_DIR%\esp_debug.log" (
    echo.
    echo === Main Debug Log ===
    type "%LOG_DIR%\esp_debug.log"
) else (
    echo [WARNING] Main log not found: %LOG_DIR%\esp_debug.log
)
goto end

:view_error
echo [INFO] Viewing error log...
if exist "%LOG_DIR%\esp_errors.log" (
    echo.
    echo === Error Log ===
    type "%LOG_DIR%\esp_errors.log"
) else (
    echo [WARNING] Error log not found: %LOG_DIR%\esp_errors.log
)
goto end

:view_perf
echo [INFO] Viewing performance log...
if exist "%LOG_DIR%\esp_performance.log" (
    echo.
    echo === Performance Log ===
    type "%LOG_DIR%\esp_performance.log"
) else (
    echo [WARNING] Performance log not found: %LOG_DIR%\esp_performance.log
)
goto end

:view_diag
echo [INFO] Listing diagnostic reports...
if exist "%LOG_DIR%" (
    echo.
    echo === Diagnostic Reports ===
    dir /b "%LOG_DIR%\diagnostic_*.txt" 2>nul
    if %errorlevel% neq 0 (
        echo [WARNING] No diagnostic reports found
    )
) else (
    echo [WARNING] Log directory not found: %LOG_DIR%
)
goto end

:view_all
echo [INFO] Viewing all logs...
echo.

echo === Main Debug Log ===
if exist "%LOG_DIR%\esp_debug.log" (
    type "%LOG_DIR%\esp_debug.log"
) else (
    echo [WARNING] Main log not found
)
echo.
echo === Error Log ===
if exist "%LOG_DIR%\esp_errors.log" (
    type "%LOG_DIR%\esp_errors.log"
) else (
    echo [WARNING] Error log not found
)
echo.
echo === Performance Log ===
if exist "%LOG_DIR%\esp_performance.log" (
    type "%LOG_DIR%\esp_performance.log"
) else (
    echo [WARNING] Performance log not found
)
echo.
echo === Diagnostic Reports ===
dir /b "%LOG_DIR%\diagnostic_*.txt" 2>nul
if %errorlevel% neq 0 (
    echo [WARNING] No diagnostic reports found
)
goto end

:end
echo.
pause
