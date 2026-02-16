@echo off
setlocal

echo ============================================================
echo  Building TeamUpdates for Revit 2024, 2025, and 2026
echo ============================================================
echo.

:: Clean previous bundle
if exist "%~dp0TeamUpdates.bundle" (
    echo Cleaning previous bundle...
    rmdir /s /q "%~dp0TeamUpdates.bundle"
    echo.
)

:: Build for Revit 2024 (.NET Framework 4.8)
echo [1/3] Building for Revit 2024 (.NET Framework 4.8)...
echo ------------------------------
dotnet build "%~dp0TeamUpdates.csproj" -c Release /p:RevitVersion=2024
if errorlevel 1 (
    echo.
    echo ERROR: Build failed for Revit 2024
    exit /b 1
)
echo.

:: Build for Revit 2025 (.NET 8.0)
echo [2/3] Building for Revit 2025 (.NET 8.0)...
echo ------------------------------
dotnet build "%~dp0TeamUpdates.csproj" -c Release /p:RevitVersion=2025
if errorlevel 1 (
    echo.
    echo ERROR: Build failed for Revit 2025
    exit /b 1
)
echo.

:: Build for Revit 2026 (.NET 8.0)
echo [3/3] Building for Revit 2026 (.NET 8.0)...
echo ------------------------------
dotnet build "%~dp0TeamUpdates.csproj" -c Release /p:RevitVersion=2026
if errorlevel 1 (
    echo.
    echo ERROR: Build failed for Revit 2026
    exit /b 1
)
echo.

echo ============================================================
echo  Build complete! Installing to Revit Addins folders...
echo ============================================================
echo.

:: Install to standard Revit Addins folders
set ADDINS_ROOT=%APPDATA%\Autodesk\Revit\Addins

for %%V in (2024 2025 2026) do (
    if exist "%~dp0TeamUpdates.bundle\Contents\%%V" (
        echo Installing for Revit %%V...
        if not exist "%ADDINS_ROOT%\%%V" mkdir "%ADDINS_ROOT%\%%V"
        xcopy /y /q "%~dp0TeamUpdates.bundle\Contents\%%V\*" "%ADDINS_ROOT%\%%V\" >nul
    )
)

echo.
echo ============================================================
echo  Installed to: %ADDINS_ROOT%\{2024,2025,2026}\
echo  Restart Revit to load the add-in.
echo ============================================================

endlocal
