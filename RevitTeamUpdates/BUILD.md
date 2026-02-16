# Quick Start: Building the Add-in

## Prerequisites

1. **Visual Studio 2022** (Community, Professional, or Enterprise)
   - Install workload: ".NET desktop development"
   
2. **Autodesk Revit 2025**

3. **.NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Or install via Visual Studio Installer

## Build Steps

### Method 1: Using Visual Studio (Recommended)

1. **Open the project**
   ```
   Double-click TeamUpdates.csproj or open in Visual Studio
   ```

2. **Verify Revit API paths** (if needed)
   - Open `TeamUpdates.csproj` in text editor
   - Verify these paths match your Revit installation:
     ```xml
     <HintPath>C:\Program Files\Autodesk\Revit 2025\RevitAPI.dll</HintPath>
     <HintPath>C:\Program Files\Autodesk\Revit 2025\RevitAPIUI.dll</HintPath>
     ```

3. **Restore NuGet packages**
   - Visual Studio should do this automatically
   - Or: Right-click solution → "Restore NuGet Packages"

4. **Build the solution**
   - Press `F6` or `Ctrl+Shift+B`
   - Or: Menu → Build → Build Solution

5. **Check output**
   - Build output will be in `bin\Debug\` or `bin\Release\`
   - Post-build event automatically copies files to Revit Add-ins folder

6. **Restart Revit**
   - Close and reopen Revit
   - Look for "Team Updates" tab in ribbon

### Method 2: Using Command Line

```batch
# Restore NuGet packages
dotnet restore TeamUpdates.csproj

# Build (Debug)
msbuild TeamUpdates.csproj /p:Configuration=Debug

# Build (Release - recommended for production)
msbuild TeamUpdates.csproj /p:Configuration=Release
```

## First Time Setup

### If Building for Different Revit Version

**Revit 2025:** Uses .NET 8.0 (current project configuration)

**Revit 2024 and earlier:** Use .NET Framework 4.8

To adapt for Revit 2024:

1. Update .csproj file:
   ```xml
   <TargetFramework>net48</TargetFramework>
   ```
   (change from `net8.0-windows` to `net48`)

2. Update Revit API paths:
   ```xml
   <HintPath>C:\Program Files\Autodesk\Revit 2024\RevitAPI.dll</HintPath>
   <HintPath>C:\Program Files\Autodesk\Revit 2024\RevitAPIUI.dll</HintPath>
   ```

3. Update post-build copy destination:
   ```xml
   <Target Name="AfterBuild">
     <Copy SourceFiles="$(TargetDir)TeamUpdates.dll" 
           DestinationFolder="$(AppData)\Autodesk\Revit\Addins\2024" />
     <Copy SourceFiles="$(ProjectDir)TeamUpdates.addin" 
           DestinationFolder="$(AppData)\Autodesk\Revit\Addins\2024" />
   </Target>
   ```

4. Rebuild solution

## Troubleshooting Build Issues

### "Could not find RevitAPI.dll"
- Verify Revit is installed at the expected path
- Update HintPath in .csproj to match your installation

### "Newtonsoft.Json not found"
- Right-click solution → Manage NuGet Packages
- Search for "Newtonsoft.Json"
- Install version 13.0.3 or later

### "Post-build event failed"
- Check that you have write permissions to the Revit Add-ins folder
- If running Visual Studio as non-admin, you may need to manually copy files

### Build succeeds but add-in doesn't load in Revit
1. Check Revit's Add-In Manager (Manage tab → Add-Ins button)
2. Look for error messages
3. Check journal file: `%LOCALAPPDATA%\Autodesk\Revit\Autodesk Revit 2025\Journals`

## Output Files

After successful build, these files should be in your Revit Add-ins folder:
- `%APPDATA%\Autodesk\Revit\Addins\2025\TeamUpdates.dll`
- `%APPDATA%\Autodesk\Revit\Addins\2025\TeamUpdates.addin`

## Development Tips

### Debug Mode
- Faster builds
- Includes debug symbols
- Can attach Visual Studio debugger to Revit process
- Larger file size

### Release Mode
- Optimized code
- Smaller file size
- Slightly faster execution
- Use for distribution

### Debugging in Visual Studio
1. Build in Debug mode
2. Start Revit normally
3. In Visual Studio: Debug → Attach to Process
4. Select Revit.exe
5. Set breakpoints in your code
6. Trigger the command in Revit

### Hot Reload During Development
Unfortunately, Revit locks the DLL once loaded. To update:
1. Close Revit
2. Rebuild solution
3. Restart Revit

To speed this up during development:
- Keep a test model open
- Save frequently
- Use quick test workflows

## Distribution

### For Your Team
1. Build in Release mode
2. Collect these files:
   - `TeamUpdates.dll` (from bin\Release)
   - `TeamUpdates.addin`
3. Share with team members
4. Team members copy to their Add-ins folder

### Creating an Installer (Optional)
Consider using:
- **Inno Setup** - Free Windows installer creator
- **WiX Toolset** - MSI installer creation
- **ClickOnce** - Simple deployment option

## Next Steps

After successful build:
1. Test both commands (Sync and View)
2. Verify changelog files are created
3. Test with existing pyRevit changelogs
4. Review MIGRATION.md for rollout strategy

## Need Help?

- Check README.md for full documentation
- Review MIGRATION.md if coming from pyRevit
- Contact Tyler Porter for support
