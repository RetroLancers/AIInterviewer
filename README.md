# AI Interviewer

AI Interviewer is a voice-enabled application designed to conduct simulated job interviews using advanced LLMs like Gemini. It provides an interactive, conversational experience to help users practice and improve their interviewing skills.

## Features

- **Voice-First Interaction**: Engage in real-time voice chats. The app uses Gemini to transcribe your voice and drive the conversation.
- **Multi-Model Support**: Designed to work with Gemini and potentially other language models.
- **Interview Workflow**:
  - **Interviewer Persona Generation**: Helpers to generate a specific interviewer prompt/persona to match your target role.
  - **Live Interview**: Conducts the interview based on the generated context.
  - **Comprehensive Reporting**: After the session, the app provides a detailed report including:
    - **Hiring Recommendation**: A hired/not hired verdict.
    - **Strengths & Weaknesses**: Analysis of your performance.
    - **Answer Review**: Highlights of what you got wrong and what you answered well.

## Architecture

- **Frontend**: `AIInterviewer.Client` (Vite based)
- **Backend**: C# / .NET ServiceStack (Solution: `AIInterviewer.sln`)

## Getting Started

See [DocumentationQuickStart.md](./DocumentationQuickStart.md) for initial setup instructions.
