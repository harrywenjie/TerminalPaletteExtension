# Repository Guidelines

## Project Structure & Module Organization
- Root solution `TerminalPaletteExtension.sln` loads the Windows App SDK extension in `TerminalPaletteExtension/`.
- `Commands/`, `Pages/`, `Services/`, and `Models/` host feature code; add new commands under `Commands/` and share DTOs in `Models/`.
- `Assets/` contains MSIX icons; update manifest references when replacing art.
- `Properties/`, `Package.appxmanifest`, and `app.manifest` control packaging and COM metadata—verify versioning before publishing.
- Generated output lives in `bin/` and `obj/`; keep artifacts out of commits.

## Build, Test, and Development Commands
- `dotnet restore` — restores centrally managed packages defined in `Directory.Packages.props`.
- `dotnet build TerminalPaletteExtension.sln -c Debug` — compiles the extension for the active platform (x64 or ARM64).
- `dotnet publish TerminalPaletteExtension/TerminalPaletteExtension.csproj -c Release -r win-x64` — produces an MSIX payload for sideloading; switch runtime as needed.
- Launch the built binary with `TerminalPaletteExtension.exe -RegisterProcessAsComServer` when validating COM activation manually.

## Coding Style & Naming Conventions
- .NET analyzers and StyleCop are enabled; resolve warnings before submission.
- Use four-space indentation, PascalCase for types/methods, camelCase for locals, and `_camelCase` for private fields.
- Keep nullable reference types enabled and prefer async/await patterns, mirroring `Program.cs`.
- Place shared primitives in `Models/` and keep UI helpers scoped to `Pages/`.

## Testing Guidelines
- No automated suite exists yet; create new unit or integration tests under a sibling `TerminalPaletteExtension.Tests/` project.
- Prefer xUnit with descriptive method names (`MethodUnderTest_State_ExpectedResult`).
- Once tests exist, run `dotnet test` locally and document environment assumptions.
- For UI flows, capture manual test steps in the PR description until automation is added.

## Commit & Pull Request Guidelines
- Follow the imperative, capitalized summary style seen in history (“Add project files.”); keep the first line ≤72 characters.
- Reference linked issues with `Fixes #123` when applicable and describe the user-facing change.
- Before opening a PR, run the build/publish commands and note results; include screenshots for UI updates.
- Request at least one reviewer and call out risky areas (COM registration, packaging, assets) to focus the review.

## Security & Configuration Notes
- Do not commit secrets or machine-specific certificates; use environment variables for local experimentation.
- Keep `nuget.config` scoped feeds private and omit them from PRs unless they are required for the build.
