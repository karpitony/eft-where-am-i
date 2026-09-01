# Repository Guidelines

## Project Purpose & Architecture

This Windows companion app helps Escape from Tarkov players locate themselves from coordinates embedded in game screenshots. It streamlines sending the latest screenshot to the `tarkov-market` map and displays the resulting position through WebView2. Preserve this core flow when changing screenshot discovery, coordinate handling, map navigation, or browser integration.

## Project Structure & Module Organization

The application lives in `eft-where-am-i/`. `Program.cs` and `Form1.cs` provide the WinForms entry point and main window. Reusable UI components are under `UserControls/`; services and helpers are under `Classes/`. Browser panels and styles live in `html/`; runtime images, generated CSS, JavaScript, and defaults live in `assets/`. Keep `translations/en.json` and `translations/ko.json` synchronized. `floor_db.json` contains map-floor data. Root `assets/` holds README media, and `.github/workflows/release.yml` defines releases.

## Build, Test, and Development Commands

Run commands from the repository root on Windows:

- `dotnet restore eft-where-am-i/eft-where-am-i.csproj` restores NuGet packages.
- `dotnet build eft-where-am-i/eft-where-am-i.csproj` installs npm dependencies when needed, compiles Tailwind CSS, and builds the app.
- `dotnet run --project eft-where-am-i/eft-where-am-i.csproj` launches the development build.
- `npm --prefix eft-where-am-i run watch:css` rebuilds Tailwind CSS while editing HTML or styles.
- `dotnet publish eft-where-am-i/eft-where-am-i.csproj -c Release -r win-x64 --self-contained false` creates a framework-dependent release build.

Use .NET 10 and Node.js 24 to match CI. WebView2 Runtime is required for local UI testing.

## Coding Style & Naming Conventions

Use four-space indentation, braces on new lines, `PascalCase` for types and public members, and `camelCase` for locals and private fields. Keep nullable-reference warnings enabled and prefer `async`/`await` for WebView2 and file operations. Do not manually edit `*.Designer.cs`, generated resource classes, or `assets/css/tailwind.css`; update the designer, `.resx`, or `html/styles.css` source instead. Preserve established JSON key casing.

## Testing Guidelines

There is currently no automated test project. Before submitting, run `dotnet build` and manually exercise the affected WinForms/WebView2 flow. For UI changes, verify both light and dark modes, English and Korean text, and relevant map/quest behavior. Add focused tests if introducing logic that can be isolated from WinForms.

## Commit & Pull Request Guidelines

History uses short, outcome-focused subjects, sometimes with `feat:` and `fix:` prefixes. Prefer an imperative subject like `fix: preserve selected quests after map change`. Keep commits scoped to one change. Pull requests should explain the solution, list manual verification, link issues, and include screenshots or recordings for UI changes. Update the project version only when preparing a release.
