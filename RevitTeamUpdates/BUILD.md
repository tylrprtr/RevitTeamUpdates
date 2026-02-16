# Quick Start: Building the Add-in

## Prerequisites

1. **Visual Studio 2022** (Community, Professional, or Enterprise)
   - Install workload: ".NET desktop development"

2. **Autodesk Revit 2024, 2025, and/or 2026**

3. **.NET 8.0 SDK** (for Revit 2025/2026)
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Or install via Visual Studio Installer

4. **.NET Framework 4.8 Developer Pack** (for Revit 2024)
   - Download from: https://dotnet.microsoft.com/download/dotnet-framework/net48

## Build Steps

### Method 1: Build All Versions (Recommended)

Run the batch script to build for Revit 2024, 2025, and 2026 in one step:

```batch
build-all.bat
```

This produces a ready-to-deploy `TeamUpdates.bundle\` folder.

### Method 2: Build for a Specific Revit Version

```batch
:: Build for Revit 2024 (.NET Framework 4.8)
dotnet build TeamUpdates.csproj -c Release /p:RevitVersion=2024

:: Build for Revit 2025 (.NET 8.0)
dotnet build TeamUpdates.csproj -c Release /p:RevitVersion=2025

:: Build for Revit 2026 (.NET 8.0)
dotnet build TeamUpdates.csproj -c Release /p:RevitVersion=2026
```

### Method 3: Using Visual Studio

1. **Open** `TeamUpdates.csproj` in Visual Studio
2. By default, it builds for Revit 2025
3. To change the target version, edit the project file or pass `/p:RevitVersion=2024` in build options
4. Build with `F6` or `Ctrl+Shift+B`

## Deployment

### Using the .bundle (Recommended)

After building, a `TeamUpdates.bundle` folder is created in the project directory with this structure:

```
TeamUpdates.bundle/
├── PackageContents.xml
└── Contents/
    ├── 2024/
    │   ├── TeamUpdates.addin
    │   ├── TeamUpdates.dll
    │   └── Newtonsoft.Json.dll
    ├── 2025/
    │   ├── TeamUpdates.addin
    │   ├── TeamUpdates.dll
    │   └── Newtonsoft.Json.dll
    └── 2026/
        ├── TeamUpdates.addin
        ├── TeamUpdates.dll
        └── Newtonsoft.Json.dll
```

**To install:**

1. Copy the entire `TeamUpdates.bundle` folder to:
   ```
   C:\ProgramData\Autodesk\ApplicationPlugins\
   ```
2. Restart Revit
3. The "Team Updates" tab appears in the ribbon

Revit automatically reads `PackageContents.xml` and loads the correct version-specific DLL.

### Manual Deployment (Alternative)

If you prefer to deploy for a single Revit version manually:

1. Build for the target version (e.g., `/p:RevitVersion=2025`)
2. Copy `TeamUpdates.dll` and `TeamUpdates.addin` from `TeamUpdates.bundle\Contents\2025\` to:
   ```
   %APPDATA%\Autodesk\Revit\Addins\2025\
   ```
3. Also copy `Newtonsoft.Json.dll` to the same folder
4. Restart Revit

## Revit API Paths

The build expects Revit API DLLs at the default install location:

| Version | Path |
|---------|------|
| 2024 | `C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll` |
| 2025 | `C:\Program Files\Autodesk\Revit 2025\RevitAPI.dll` |
| 2026 | `C:\Program Files\Autodesk\Revit 2026\RevitAPI.dll` |

You only need the API DLLs for the versions you're building. If you don't have a specific Revit version installed, skip building for that version and use `dotnet build /p:RevitVersion=XXXX` instead of `build-all.bat`.

## Troubleshooting Build Issues

### "Could not find RevitAPI.dll"
- Verify Revit is installed at the expected path
- You only need the Revit versions installed that you want to build for
- Use `/p:RevitVersion=XXXX` to build only for versions you have

### "Newtonsoft.Json not found"
- Run `dotnet restore TeamUpdates.csproj` to restore NuGet packages

### Build succeeds but add-in doesn't load in Revit
1. Verify the bundle is in `C:\ProgramData\Autodesk\ApplicationPlugins\`
2. Check Revit's Add-In Manager (Manage tab > Add-Ins button)
3. Check journal file: `%LOCALAPPDATA%\Autodesk\Revit\Autodesk Revit 20XX\Journals`

## Development Tips

### Debug Mode
- Faster builds, includes debug symbols
- Can attach Visual Studio debugger to Revit process

### Release Mode
- Optimized and smaller — use for distribution

### Debugging in Visual Studio
1. Build in Debug mode for a specific version
2. Start Revit normally
3. In Visual Studio: Debug > Attach to Process > Select Revit.exe
4. Set breakpoints and trigger the command in Revit

## Distribution

### For Your Team
1. Run `build-all.bat` (builds Release for all versions)
2. Share the `TeamUpdates.bundle` folder
3. Team members copy it to `C:\ProgramData\Autodesk\ApplicationPlugins\`
4. Restart Revit — done

## Need Help?

- Check README.md for full documentation
- Review MIGRATION.md if coming from pyRevit
- Contact Tyler Porter for support
