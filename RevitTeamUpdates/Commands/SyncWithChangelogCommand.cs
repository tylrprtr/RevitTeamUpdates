using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TeamUpdates.Managers;
using TeamUpdates.UI;

namespace TeamUpdates.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SyncWithChangelogCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            try
            {
                // Check if model is workshared
                if (!doc.IsWorkshared)
                {
                    TaskDialog.Show("Not Workshared",
                        "This command only works with workshared models.\n\n" +
                        "Please enable worksharing first.");
                    return Result.Cancelled;
                }

                // Check if model is central or local
                if (!doc.IsModelInCloud && doc.GetWorksharingCentralModelPath() == null)
                {
                    TaskDialog.Show("No Central Model",
                        "This model is not linked to a central model.\n\n" +
                        "Please open a local copy of a workshared model.");
                    return Result.Cancelled;
                }

                // Initialize changelog manager
                var changelogMgr = new ChangelogManager(doc);

                // Get model name for display
                var modelName = changelogMgr.GetCentralModelName();

                // Check if project folder is configured
                var projectFolder = changelogMgr.GetProjectFolder();
                if (string.IsNullOrEmpty(projectFolder))
                {
                    TaskDialog.Show("Project Folder Not Configured",
                        "The 'Project Directory Filepath' parameter is not set.\n\n" +
                        "Please add this parameter to Project Information with the path to your project folder.\n\n" +
                        "Example: \\\\server\\share\\Projects\\2025\\Project Name\\Models");
                    return Result.Cancelled;
                }

                // Show changelog input dialog
                var dialog = new ChangelogInputWindow(modelName);
                var result = dialog.ShowDialog();

                if (result != true || string.IsNullOrEmpty(dialog.ChangelogText))
                {
                    return Result.Cancelled;
                }

                var changelogText = dialog.ChangelogText;

                // Get username
                var username = uiApp.Application.Username;

                // Save changelog
                var (success, saveMessage) = changelogMgr.SaveChangelog(username, changelogText);

                if (!success)
                {
                    TaskDialog.Show("Error Saving Changelog",
                        $"Failed to save changelog:\n\n{saveMessage}");
                    return Result.Failed;
                }

                // Perform sync to central
                bool syncSuccess = false;
                string syncError = null;

                try
                {
                    // Configure sync options
                    var syncOptions = new SynchronizeWithCentralOptions();
                    var transactOptions = new TransactWithCentralOptions();

                    // Configure relinquish options
                    var relinquishOptions = new RelinquishOptions(true);
                    syncOptions.SetRelinquishOptions(relinquishOptions);

                    // Add comment (truncate if too long)
                    var comment = changelogText.Length > 100
                        ? changelogText.Substring(0, 100) + "..."
                        : changelogText;
                    syncOptions.Comment = $"Synced with changelog: {comment}";

                    // Don't compact for cloud models
                    if (!doc.IsModelInCloud)
                    {
                        syncOptions.Compact = true;
                    }

                    // Perform the sync
                    doc.SynchronizeWithCentral(transactOptions, syncOptions);

                    syncSuccess = true;

                    // Show success message
                    TaskDialog.Show("Sync Complete - Changelog Saved",
                        $"{modelName}\n\n" +
                        "Sync completed successfully!\n\n" +
                        $"Changelog saved:\n{changelogText}");
                }
                catch (Exception ex)
                {
                    syncError = ex.Message;
                    
                    // Try to open native sync dialog as fallback
                    try
                    {
                        var syncCommandId = RevitCommandId.LookupPostableCommandId(PostableCommand.SynchronizeAndModifySettings);
                        uiApp.PostCommand(syncCommandId);

                        TaskDialog.Show("Changelog Saved - Complete Sync Manually",
                            $"{modelName}\n\n" +
                            $"Changelog saved:\n{changelogText}\n\n" +
                            "Automatic sync encountered an issue.\n" +
                            "Revit's sync dialog has been opened - please complete the sync.");
                        
                        syncSuccess = true; // Consider it handled
                    }
                    catch
                    {
                        TaskDialog.Show("Changelog Saved - Sync Manually",
                            $"{modelName}\n\n" +
                            $"Changelog saved:\n{changelogText}\n\n" +
                            "However, automatic sync failed.\n" +
                            "Please use the regular 'Synchronize with Central' button to complete the sync.");
                        
                        syncSuccess = true; // Consider it handled
                    }
                }

                return syncSuccess ? Result.Succeeded : Result.Failed;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Error", $"An error occurred:\n\n{ex.Message}");
                return Result.Failed;
            }
        }
    }
}
