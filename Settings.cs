using System;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace MouseHighlighter
{
    public class Settings
    {
        public Color CursorColor { get; set; } = Color.Yellow;
        public Color ClickColor { get; set; } = Color.Red;
        public float Opacity { get; set; } = 0.5f;
        public int CircleSize { get; set; } = 30;

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MouseHighlighter",
            "settings.json");

        public static Settings Load()
        {
            try
            {
                string directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var settingsData = JsonSerializer.Deserialize<SettingsData>(json);
                    return new Settings
                    {
                        CursorColor = Color.FromArgb(settingsData.CursorColorArgb),
                        ClickColor = Color.FromArgb(settingsData.ClickColorArgb),
                        Opacity = settingsData.Opacity,
                        CircleSize = settingsData.CircleSize
                    };
                }
            }
            catch
            {
                // If anything goes wrong, return default settings
            }

            return new Settings();
        }

        public void Save()
        {
            try
            {
                var settingsData = new SettingsData
                {
                    CursorColorArgb = CursorColor.ToArgb(),
                    ClickColorArgb = ClickColor.ToArgb(),
                    Opacity = Opacity,
                    CircleSize = CircleSize
                };

                string json = JsonSerializer.Serialize(settingsData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        // Helper class for JSON serialization (since Color isn't directly serializable)
        private class SettingsData
        {
            public int CursorColorArgb { get; set; }
            public int ClickColorArgb { get; set; }
            public float Opacity { get; set; }
            public int CircleSize { get; set; }
        }
    }
}
