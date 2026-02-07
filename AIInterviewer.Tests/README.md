# AIInteviewer Tests

This project contains both unit tests and integration tests for the AI Interviewer application.

## Integration Tests

Integration tests for generic AI providers (Gemini, OpenAI) are included. These tests interact with the actual AI APIs and require valid API keys. They are excluded from the CI/CD pipeline by default.

### Prerequisites

To run these integration tests locally, you must configure User Secrets for the test project.

1.  **Initialize User Secrets** (if not already done):
    ```bash
    cd AIInterviewer.Tests
    dotnet user-secrets init
    ```

2.  **Configure Secrets**:
    You need to set the API keys for the providers you want to test.

    *   **Gemini**:
        ```bash
        dotnet user-secrets set "Gemini:ApiKey" "<YOUR_GEMINI_API_KEY>"
        # Optional: Set a specific model ID
        dotnet user-secrets set "Gemini:ModelId" "gemini-2.0-flash-exp"
        ```

    *   **OpenAI**:
        ```bash
        dotnet user-secrets set "OpenAI:ApiKey" "<YOUR_OPENAI_API_KEY>"
        # Optional: Set a specific model ID
        dotnet user-secrets set "OpenAI:ModelId" "gpt-4o"
        ```

### Running Tests

*   **Run Unit Tests Only**:
    ```bash
    dotnet test --filter "TestCategory!=Integration"
    ```

*   **Run Integration Tests**:
    ```bash
    dotnet test --filter "TestCategory=Integration"
    ```

*   **Run All Tests**:
    ```bash
    dotnet test
    ```
    *Note: Integration tests marked with `[Explicit]` attribute will be skipped by default unless explicitly included or configured.*
