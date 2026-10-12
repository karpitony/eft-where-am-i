# Settings persistence checks

Run from the repository root with .NET 10 on Windows:

```powershell
dotnet run --project tests/SettingsPersistence/SettingsPersistence.csproj
```

The checks use the application's actual `SettingsHandler` and packaged defaults in temporary directories. They cover legacy migration, precedence of saved settings, persistence across a new application directory, first launch, missing fields, and invalid JSON. They do not access the user's real settings or require WinForms/WebView2.

For an installed-app update check, use a disposable Windows profile: change the language, theme, screenshot/log paths, dead zone, and panel visibility; restart and confirm the values; then apply a local Velopack update and confirm the same values after restart. Test both the startup update prompt and the Settings page update button. When starting from a version that still saves `assets/settings.json`, copy it to `%LOCALAPPDATA%\EFT-Where-Am-I\settings.json` before applying the first update.
