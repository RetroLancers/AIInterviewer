# 28. Create Generic Provider Tests

## Objectives
- Create tests for generic AI providers (specifically generating text to a basic schema answer).
- Ensure these tests use dotnet secrets for configuration.
- Exclude these tests from the CI/CD pipeline (update workflows in `.github/workflows`).
- Update `Tests/README.md` with instructions on how to add the necessary secrets.

## Context
We need to verify that our generic providers (e.g., Gemini, OpenAI) can correctly generate a basic schema answer from text input. However, since these tests will require actual API keys (via dotnet secrets), they cannot be run in the public CI/CD pipeline.

## Requirements
1.  **Test Project**: Add a new test class/project for these integration/functional tests.
2.  **Dotnet Secrets**: Configure the test project to use dotnet secrets for API keys.
3.  **CI/CD Exclusion**: Modify `.github/workflows/build.yml` (and others if necessary) to exclude these specific tests from running during the build process.
4.  **Documentation**: Update `Tests/README.md` to explain:
    *   Which secrets are needed.
    *   How to set them using `dotnet user-secrets`.
    *   How to run the tests locally.

## Outcome
- A new test suite exists for generic provider schema generation.
- The tests pass locally when secrets are configured.
- The tests are skipped in CI/CD.
- Developers know how to set up the secrets to run these tests.
