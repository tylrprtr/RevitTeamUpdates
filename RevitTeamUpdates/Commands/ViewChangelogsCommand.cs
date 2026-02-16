using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TeamUpdates.Managers;
using TeamUpdates.UI;

namespace TeamUpdates.Commands
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class ViewChangelogsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // Initialize changelog manager
                var changelogMgr = new ChangelogManager(doc);

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

                // Show date range selection dialog
                var dateDialog = new DateRangeWindow();
                var result = dateDialog.ShowDialog();

                if (result != true)
                {
                    return Result.Cancelled;
                }

                var days = dateDialog.Days;

                // Get changelogs
                var changelogs = changelogMgr.GetChangelogs(days);

                if (changelogs.Count == 0)
                {
                    TaskDialog.Show("No Changelogs Found",
                        "No changelogs found for the selected date range.\n\n" +
                        "Make sure team members are using 'Sync with Changelog' button.");
                    return Result.Cancelled;
                }

                // Format report
                var reportText = changelogMgr.FormatChangelogReport(changelogs, includeStats: true);

                // Show report dialog
                var reportDialog = new ChangelogReportWindow(reportText);
                reportDialog.ShowDialog();

                return Result.Succeeded;
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
