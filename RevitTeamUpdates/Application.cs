using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace TeamUpdates
{
    /// <summary>
    /// Main application class that creates the ribbon UI
    /// </summary>
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Create ribbon tab
                const string tabName = "Team Updates";
                application.CreateRibbonTab(tabName);

                // Get assembly path
                var assemblyPath = Assembly.GetExecutingAssembly().Location;

                // Create Sync panel
                var syncPanel = application.CreateRibbonPanel(tabName, "Sync");
                
                var syncButtonData = new PushButtonData(
                    "SyncWithChangelog",
                    "Sync with\nChangelog",
                    assemblyPath,
                    "TeamUpdates.Commands.SyncWithChangelogCommand")
                {
                    ToolTip = "Sync to central and record a changelog entry",
                    LongDescription = "Opens a dialog to enter changelog information, then performs a sync to central and saves the changelog to the project folder."
                };

                var syncButton = syncPanel.AddItem(syncButtonData) as PushButton;
                try
                {
                    var asm = Assembly.GetExecutingAssembly();
                    using (var stream = asm.GetManifestResourceStream("TeamUpdates.icons.icon-sync.png"))
                    {
                        if (stream != null && syncButton != null)
                        {
                            byte[] data;
                            using (var ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                data = ms.ToArray();
                            }

                            using (var msLarge = new MemoryStream(data))
                            {
                                var bmpLarge = new BitmapImage();
                                bmpLarge.BeginInit();
                                bmpLarge.StreamSource = msLarge;
                                bmpLarge.DecodePixelWidth = 32;
                                bmpLarge.CacheOption = BitmapCacheOption.OnLoad;
                                bmpLarge.EndInit();
                                bmpLarge.Freeze();
                                syncButton.LargeImage = bmpLarge;
                            }

                            using (var msSmall = new MemoryStream(data))
                            {
                                var bmpSmall = new BitmapImage();
                                bmpSmall.BeginInit();
                                bmpSmall.StreamSource = msSmall;
                                bmpSmall.DecodePixelWidth = 16;
                                bmpSmall.CacheOption = BitmapCacheOption.OnLoad;
                                bmpSmall.EndInit();
                                bmpSmall.Freeze();
                                syncButton.Image = bmpSmall;
                            }
                        }
                    }
                }
                catch { }

                // Create Reports panel
                var reportsPanel = application.CreateRibbonPanel(tabName, "Reports");
                
                var viewButtonData = new PushButtonData(
                    "ViewChangelogs",
                    "View\nChangelogs",
                    assemblyPath,
                    "TeamUpdates.Commands.ViewChangelogsCommand")
                {
                    ToolTip = "View changelog reports for sync activity",
                    LongDescription = "Generate and view reports of all changelog entries over a selected time period."
                };

                var viewButton = reportsPanel.AddItem(viewButtonData) as PushButton;
                try
                {
                    var asm = Assembly.GetExecutingAssembly();
                    using (var stream = asm.GetManifestResourceStream("TeamUpdates.icons.icon-view-changelog.png"))
                    {
                        if (stream != null && viewButton != null)
                        {
                            byte[] data;
                            using (var ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                data = ms.ToArray();
                            }

                            using (var msLarge = new MemoryStream(data))
                            {
                                var bmpLarge = new BitmapImage();
                                bmpLarge.BeginInit();
                                bmpLarge.StreamSource = msLarge;
                                bmpLarge.DecodePixelWidth = 32;
                                bmpLarge.CacheOption = BitmapCacheOption.OnLoad;
                                bmpLarge.EndInit();
                                bmpLarge.Freeze();
                                viewButton.LargeImage = bmpLarge;
                            }

                            using (var msSmall = new MemoryStream(data))
                            {
                                var bmpSmall = new BitmapImage();
                                bmpSmall.BeginInit();
                                bmpSmall.StreamSource = msSmall;
                                bmpSmall.DecodePixelWidth = 16;
                                bmpSmall.CacheOption = BitmapCacheOption.OnLoad;
                                bmpSmall.EndInit();
                                bmpSmall.Freeze();
                                viewButton.Image = bmpSmall;
                            }
                        }
                    }
                }
                catch { }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Failed to initialize Team Updates add-in:\n\n{ex.Message}");
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
