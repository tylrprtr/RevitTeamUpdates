using System;
using Newtonsoft.Json;

namespace TeamUpdates.Models
{
    /// <summary>
    /// Represents a single changelog entry
    /// </summary>
    public class ChangelogEntry
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("changelog")]
        public string Changelog { get; set; }

        public ChangelogEntry()
        {
        }

        public ChangelogEntry(string username, string changelog)
        {
            Username = username;
            Timestamp = DateTime.Now;
            Changelog = changelog;
        }
    }
}
