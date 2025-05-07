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
        public int Thickness { get; set; } = 3;
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
                    try 
                    {
                        string json = File.ReadAllText(SettingsPath);
                        var settings = new Settings();
                        
                        // Handle versioning differences
                        try 
                        {
                            // Try to deserialize directly first
                            var settingsData = JsonSerializer.Deserialize<SettingsData>(json);
                            settings.CursorColor = Color.FromArgb(settingsData.CursorColorArgb);
                            settings.ClickColor = Color.FromArgb(settingsData.ClickColorArgb);
                            settings.CircleSize = settingsData.CircleSize;
                            
                            // Check if we have the new thickness property
                            if (json.Contains("\"Thickness\":"))
                            {
                                settings.Thickness = settingsData.Thickness;
                            }
                            // Handle old opacity format by converting to a reasonable thickness
                            else if (json.Contains("\"Opacity\":"))
                            {
                                // Get opacity directly from JsonElement to handle type changes
                                var options = new JsonDocumentOptions { AllowTrailingCommas = true };
                                using (JsonDocument document = JsonDocument.Parse(json, options))
                                {
                                    if (document.RootElement.TryGetProperty("Opacity", out JsonElement opacityElement))
                                    {
                                        if (opacityElement.ValueKind == JsonValueKind.Number)
                                        {
                                            float opacity = opacityElement.GetSingle();
                                            // Convert opacity to a reasonable thickness (1-10)
                                            settings.Thickness = Math.Max(1, Math.Min(10, (int)(opacity * 10) + 1));
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // If anything goes wrong with conversion, use defaults
                            settings = new Settings();
                        }
                        
                        return settings;
                    }
                    catch
                    {
                        // If anything goes wrong with file reading, use defaults
                        return new Settings();
                    }
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
                    Thickness = Thickness,
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
            public int Thickness { get; set; }
            public int CircleSize { get; set; }
        }
    }
}
