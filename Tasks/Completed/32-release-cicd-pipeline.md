# Task 32: Release CI/CD Pipeline

## Objective
Create a dedicated release CI/CD pipeline that builds and packages the application for distribution. This pipeline should:
- Only run on the `release` branch
- Skip tests (since they run on the main build pipeline)
- Build the application for multiple platforms
- Create release artifacts (tar.gz for Linux/macOS, zip for Windows)
- Publish these artifacts as GitHub releases

## Requirements

### 1. Update Existing Build Pipeline
- Modify `.github/workflows/build.yml` to exclude the `release` branch
- Ensure it continues to run on all other branches and pull requests

### 2. Create New Release Pipeline
Create `.github/workflows/release-build.yml` with the following features:

#### Trigger Configuration
- Only trigger on pushes to the `release` branch
- Support manual workflow dispatch for testing

#### Build Steps
- Checkout code
- Setup .NET 10.0.x
- Restore NuGet packages
- Build in Release configuration
- Publish self-contained builds for multiple platforms:
  - `win-x64` (Windows 64-bit)
  - `linux-x64` (Linux 64-bit)
  - `osx-x64` (macOS Intel)
  - `osx-arm64` (macOS Apple Silicon)

#### Artifact Creation
- Create compressed archives for each platform:
  - `.zip` files for Windows
  - `.tar.gz` files for Linux and macOS
- Include version information in artifact names
- Package should include:
  - Compiled binaries
  - Configuration files
  - Required dependencies

#### Release Publishing
- Create a GitHub release with the artifacts
- Use semantic versioning or date-based versioning
- Include release notes (can be auto-generated or from commit messages)

## Implementation Checklist

- [ ] Update `.github/workflows/build.yml` to exclude `release` branch
- [ ] Create `.github/workflows/release-build.yml`
- [ ] Configure build matrix for multiple platforms
- [ ] Add publish steps for each platform
- [ ] Add compression steps (zip/tar.gz)
- [ ] Configure GitHub release creation
- [ ] Add artifact upload to release
- [ ] Test the pipeline (may need to create a test release branch)
- [ ] Update documentation with release process

## Technical Notes

### Platform-Specific Publish Commands
```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/win-x64

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish/linux-x64

# macOS Intel
dotnet publish -c Release -r osx-x64 --self-contained true -o ./publish/osx-x64

# macOS ARM
dotnet publish -c Release -r osx-arm64 --self-contained true -o ./publish/osx-arm64
```

### Archive Creation
```bash
# For tar.gz
tar -czf aiinterviewer-linux-x64-v1.0.0.tar.gz -C ./publish/linux-x64 .

# For zip (using PowerShell on Windows runner or zip command)
zip -r aiinterviewer-win-x64-v1.0.0.zip ./publish/win-x64/*
```

### Version Strategy
Consider using one of:
- Git tags (e.g., `v1.0.0`)
- Date-based (e.g., `2026.02.07`)
- Commit SHA (short form)

## Dependencies
- None (new pipeline)

## Clood Groups to Update
- `cicd` (create if doesn't exist)
- `build-automation`

## Success Criteria
- [ ] Build pipeline no longer runs on `release` branch
- [ ] Release pipeline successfully builds for all platforms
- [ ] Artifacts are created and properly compressed
- [ ] GitHub release is created with all artifacts attached
- [ ] Release artifacts are downloadable and functional
