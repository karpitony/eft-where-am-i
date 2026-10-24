using eft_where_am_i.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

Run("Import legacy settings without modifying the original", paths =>
{
    var expected = new AppSettings
    {
        language = "ko",
        tarkov_market_language = "ko",
        theme_mode = "dark",
        screenshot_path = @"C:\Screenshots",
        log_path = @"C:\EFT\Logs",
        latest_map = "reserve",
        dead_zone_percent = 75,
        auto_screenshot_detection = true,
        auto_map_detection = true,
        auto_panning = false,
        auto_screenshot_cleanup = true,
        screenshot_paths_list = new List<string> { @"Documents\Escape from Tarkov\Screenshots" },
        panel_hidden_per_map = new Dictionary<string, bool> { ["reserve"] = true, ["woods"] = false }
    };
    string original = JsonConvert.SerializeObject(expected);
    File.WriteAllText(paths.Legacy, original);
    File.WriteAllText(paths.Defaults, "{}");

    AssertSettings(expected, paths.CreateHandler().GetSettings());
    AssertSettings(expected, ReadSettings(paths.User));
    Assert(File.ReadAllText(paths.Legacy) == original, "Migration changed the original file.");
});

Run("Existing user settings take precedence over legacy files and defaults", paths =>
{
    var expected = new AppSettings { language = "ko", theme_mode = "dark", latest_map = "woods" };
    Directory.CreateDirectory(Path.GetDirectoryName(paths.User)!);
    string original = JsonConvert.SerializeObject(expected);
    File.WriteAllText(paths.User, original);
    File.WriteAllText(paths.Legacy, "invalid legacy JSON");
    File.WriteAllText(paths.Defaults, "invalid defaults JSON");

    AssertSettings(expected, paths.CreateHandler().GetSettings());
    Assert(File.ReadAllText(paths.User) == original, "Loading rewrote existing user settings.");
});

Run("Saved settings survive replacing the application files", paths =>
{
    File.WriteAllText(paths.Legacy, "{\"language\":\"ko\",\"theme_mode\":\"dark\"}");
    var handler = paths.CreateHandler();
    handler.SetValue<string>(settings => settings.screenshot_path = @"D:\EFT\Screenshots");
    handler.SetValue<int>(settings => settings.dead_zone_percent = 65);
    handler.SetValue<bool>(settings => settings.panel_hidden_per_map["customs"] = true);
    var expected = ReadSettings(paths.User);

    // An update supplies a different application directory, with new packaged defaults.
    string nextAssets = Path.Combine(paths.Root, "next-version", "assets");
    Directory.CreateDirectory(nextAssets);
    string nextDefaults = Path.Combine(nextAssets, "settings.defaults.json");
    File.WriteAllText(nextDefaults, "{\"language\":\"en\",\"theme_mode\":\"light\",\"dead_zone_percent\":99}");
    File.Delete(paths.Legacy);
    var restarted = new SettingsHandler(paths.User, Path.Combine(nextAssets, "settings.json"), nextDefaults);

    AssertSettings(expected, restarted.GetSettings());
    AssertSettings(expected, ReadSettings(paths.User));
});

Run("First launch seeds user settings from the packaged defaults", paths =>
{
    string template = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "assets", "settings.defaults.json"));
    File.WriteAllText(paths.Defaults, template);
    var handler = paths.CreateHandler();

    AssertSettings(ReadSettings(paths.Defaults), handler.GetSettings());
    AssertSettings(handler.GetSettings(), ReadSettings(paths.User));
    handler.SetValue<string>(settings => settings.language = "ko");
    Assert(File.ReadAllText(paths.Defaults) == template, "Saving modified the packaged defaults.");
    Assert(!File.Exists(paths.Legacy), "Saving created a legacy settings file.");
});

Run("Missing settings files fall back to model defaults", paths =>
{
    AssertSettings(new AppSettings(), paths.CreateHandler().GetSettings());
    AssertSettings(new AppSettings(), ReadSettings(paths.User));
});

Run("Older settings retain their values and receive defaults for missing fields", paths =>
{
    File.WriteAllText(paths.Legacy, "{\"language\":\"ko\",\"auto_panning\":false}");
    AssertSettings(new AppSettings { language = "ko", auto_panning = false }, paths.CreateHandler().GetSettings());
});

Run("Invalid legacy settings are preserved instead of being replaced with defaults", paths =>
{
    const string invalid = "invalid legacy JSON";
    File.WriteAllText(paths.Legacy, invalid);
    File.WriteAllText(paths.Defaults, "{}");
    AssertLoadFails(paths);
    Assert(!File.Exists(paths.User), "Failed migration created a user settings file.");
    Assert(File.ReadAllText(paths.Legacy) == invalid, "Failed migration modified the original file.");
});

Run("Invalid user settings are preserved instead of falling back to another file", paths =>
{
    Directory.CreateDirectory(Path.GetDirectoryName(paths.User)!);
    const string invalid = "invalid user JSON";
    File.WriteAllText(paths.User, invalid);
    File.WriteAllText(paths.Legacy, "{}");
    File.WriteAllText(paths.Defaults, "{}");
    AssertLoadFails(paths);
    Assert(File.ReadAllText(paths.User) == invalid, "Failed load overwrote user settings.");
});

Console.WriteLine("All 8 settings persistence checks passed.");

static void Run(string name, Action<SettingsPaths> check)
{
    using var paths = new SettingsPaths();
    check(paths);
    Console.WriteLine($"PASS: {name}");
}

static AppSettings ReadSettings(string path)
{
    return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path))
        ?? throw new Exception($"No settings in {path}.");
}

static void AssertSettings(AppSettings expected, AppSettings actual)
{
    Assert(JToken.DeepEquals(JToken.FromObject(expected), JToken.FromObject(actual)), "Settings were not preserved.");
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new Exception(message);
    }
}

static void AssertLoadFails(SettingsPaths paths)
{
    try
    {
        paths.CreateHandler();
    }
    catch (Exception ex) when (ex.Message.StartsWith("Error loading JSON file:", StringComparison.Ordinal))
    {
        return;
    }

    throw new Exception("Invalid JSON did not fail to load.");
}

sealed class SettingsPaths : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), $"eft-settings-tests-{Guid.NewGuid():N}");
    public string User => Path.Combine(Root, "user-data", "EFT-Where-Am-I", "settings.json");
    public string Legacy => Path.Combine(Root, "current", "assets", "settings.json");
    public string Defaults => Path.Combine(Root, "current", "assets", "settings.defaults.json");

    public SettingsPaths()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Legacy)!);
    }

    public SettingsHandler CreateHandler() => new SettingsHandler(User, Legacy, Defaults);

    public void Dispose()
    {
        // Only remove the unique fixture directory directly under the system temp directory.
        if (!string.Equals(Path.GetDirectoryName(Path.GetFullPath(Root)),
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.GetTempPath())), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Test directory is outside the expected temp directory.");
        }

        Directory.Delete(Root, true);
    }
}
