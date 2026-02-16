using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Newtonsoft.Json;
using TeamUpdates.Models;

namespace TeamUpdates.Managers
{
    /// <summary>
    /// Manages changelog storage and retrieval for sync operations
    /// </summary>
    public class ChangelogManager
    {
        private const string PARAM_NAME = "Project Directory Filepath";
        private const string CHANGELOG_SUBFOLDER = "SyncChangelogs";

        private readonly Document _doc;

        public ChangelogManager(Document doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Get the project folder path from project parameter
        /// </summary>
        public string GetProjectFolder()
        {
            try
            {
                var projectInfo = _doc.ProjectInformation;
                var param = projectInfo.LookupParameter(PARAM_NAME);

                if (param != null && param.HasValue)
                {
                    var folderPath = param.AsString();
                    if (!string.IsNullOrWhiteSpace(folderPath))
                    {
                        return folderPath.Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting project folder: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get the full path to the changelog folder
        /// </summary>
        public string GetChangelogFolder()
        {
            var projectFolder = GetProjectFolder();
            if (string.IsNullOrEmpty(projectFolder))
                return null;

            return Path.Combine(projectFolder, CHANGELOG_SUBFOLDER);
        }

        /// <summary>
        /// Ensure the changelog folder exists
        /// </summary>
        /// <returns>Tuple of (success, folder path or error message)</returns>
        public (bool Success, string Message) EnsureChangelogFolder()
        {
            var changelogFolder = GetChangelogFolder();

            if (string.IsNullOrEmpty(changelogFolder))
            {
                return (false, $"Project folder not configured. Please set the '{PARAM_NAME}' project parameter in Project Information.");
            }

            var projectFolder = GetProjectFolder();

            // Check if base project folder exists
            if (!Directory.Exists(projectFolder))
            {
                return (false, $"Project folder does not exist:\n\n{projectFolder}\n\nPlease verify the '{PARAM_NAME}' parameter in Project Information points to a valid network path.");
            }

            try
            {
                // Create the SyncChangelogs subfolder if it doesn't exist
                if (!Directory.Exists(changelogFolder))
                {
                    Directory.CreateDirectory(changelogFolder);
                }

                return (true, changelogFolder);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating changelog folder:\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// Save a changelog entry
        /// </summary>
        public (bool Success, string Message) SaveChangelog(string username, string changelogText)
        {
            var (success, result) = EnsureChangelogFolder();
            if (!success)
                return (false, result);

            var changelogFolder = result;

            try
            {
                var entry = new ChangelogEntry(username, changelogText);
                
                // Generate filename with timestamp
                var filename = $"changelog_{entry.Timestamp:yyyyMMdd_HHmmss}.json";
                var filepath = Path.Combine(changelogFolder, filename);

                // Serialize to JSON
                var json = JsonConvert.SerializeObject(entry, Formatting.Indented);
                File.WriteAllText(filepath, json);

                return (true, "Changelog saved successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error saving changelog:\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// Get the central model name
        /// </summary>
        public string GetCentralModelName()
        {
            try
            {
                if (_doc.IsModelInCloud)
                {
                    // For cloud models - get the model GUID and use title instead
                    var modelGuid = _doc.GetCloudModelPath().GetModelGUID();
                    // Try to get a friendly name from document title
                    return Path.GetFileNameWithoutExtension(_doc.Title);
                }
                else
                {
                    // For non-cloud workshared models
                    var centralPath = _doc.GetWorksharingCentralModelPath();
                    if (centralPath != null)
                    {
                        var pathString = ModelPathUtils.ConvertModelPathToUserVisiblePath(centralPath);
                        return Path.GetFileNameWithoutExtension(pathString);
                    }
                    else if (!string.IsNullOrEmpty(_doc.PathName))
                    {
                        return Path.GetFileNameWithoutExtension(_doc.PathName);
                    }
                }
            }
            catch
            {
                // Fallback
                if (!string.IsNullOrEmpty(_doc.Title))
                    return _doc.Title;
                if (!string.IsNullOrEmpty(_doc.PathName))
                    return Path.GetFileNameWithoutExtension(_doc.PathName);
            }

            return "Unknown Model";
        }

        /// <summary>
        /// Get changelogs within a date range
        /// </summary>
        /// <param name="days">Number of days to look back (at midnight boundaries)</param>
        public List<ChangelogEntry> GetChangelogs(int days = 7)
        {
            var changelogFolder = GetChangelogFolder();

            if (string.IsNullOrEmpty(changelogFolder) || !Directory.Exists(changelogFolder))
                return new List<ChangelogEntry>();

            // Calculate date range with midnight boundaries
            var startDate = DateTime.Today.AddDays(-days);
            var endDate = DateTime.Today.AddDays(1).AddTicks(-1); // End of today

            var changelogs = new List<ChangelogEntry>();

            try
            {
                var jsonFiles = Directory.GetFiles(changelogFolder, "*.json");

                foreach (var filepath in jsonFiles)
                {
                    try
                    {
                        var json = File.ReadAllText(filepath);
                        var entry = JsonConvert.DeserializeObject<ChangelogEntry>(json);

                        if (entry != null && entry.Timestamp >= startDate && entry.Timestamp <= endDate)
                        {
                            changelogs.Add(entry);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error reading {filepath}: {ex.Message}");
                    }
                }

                // Sort by timestamp (newest first)
                changelogs = changelogs.OrderByDescending(c => c.Timestamp).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading changelogs: {ex.Message}");
            }

            return changelogs;
        }

        /// <summary>
        /// Format changelogs into a readable report
        /// </summary>
        public string FormatChangelogReport(List<ChangelogEntry> changelogs, bool includeStats = true)
        {
            if (changelogs == null || changelogs.Count == 0)
            {
                return "No changelogs found for the selected date range.";
            }

            var lines = new List<string>();

            if (includeStats)
            {
                var modelName = GetCentralModelName();
                var uniqueUsers = changelogs.Select(c => c.Username).Distinct().Count();

                lines.Add(new string('=', 60));
                lines.Add($"SYNC CHANGELOG REPORT - {modelName}");
                lines.Add(new string('=', 60));
                lines.Add("");
                lines.Add($"Total Syncs: {changelogs.Count}");
                lines.Add($"Unique Users: {uniqueUsers}");

                var firstTime = changelogs.Last().Timestamp;
                var lastTime = changelogs.First().Timestamp;
                lines.Add($"Date Range: {firstTime:yyyy-MM-dd} to {lastTime:yyyy-MM-dd}");

                lines.Add("");
                lines.Add(new string('=', 60));
                lines.Add("");
            }

            // Individual entries
            for (int i = 0; i < changelogs.Count; i++)
            {
                var entry = changelogs[i];
                lines.Add($"[{i + 1}] {entry.Timestamp:yyyy-MM-dd hh:mm:ss tt} - {entry.Username}");
                lines.Add(entry.Changelog);
                lines.Add("");
                lines.Add(new string('-', 60));
                lines.Add("");
            }

            return string.Join("\n", lines);
        }
    }
}
