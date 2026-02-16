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
echo  Build complete!
echo ============================================================
echo.
echo Bundle location:
echo   %~dp0TeamUpdates.bundle\
echo.
echo To install, copy the TeamUpdates.bundle folder to:
echo   C:\ProgramData\Autodesk\ApplicationPlugins\
echo.
echo Revit will automatically load the correct version on startup.
echo ============================================================

endlocal
