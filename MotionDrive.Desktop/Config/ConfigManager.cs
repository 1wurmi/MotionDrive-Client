using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Config;
public class ConfigManager
{

    public string GetConfigPath()
    {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        string appFolder = System.IO.Path.Combine(appDataFolder, "MotionDrive");

        if (!System.IO.Directory.Exists(appFolder))
        {
            System.IO.Directory.CreateDirectory(appFolder);
        }

        return Path.Combine(appFolder, "config.json");
    }

    public ConfigModel LoadConfig()
    {
        string path = GetConfigPath();

        if (!File.Exists(path))
        {
            return new ConfigModel();
        }

        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<ConfigModel>(json);
    }

    public void SaveConfig(ConfigModel configModel)
    {
        string path = GetConfigPath();
        string json = JsonSerializer.Serialize(configModel, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public string GetConfigAsString()
    {
        string path = GetConfigPath();

        return File.ReadAllText(path);
    }
}
