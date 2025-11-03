# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Connect4 is a .NET 8 C# application implementing the Connect Four game with an AI opponent. The project includes multiple interfaces: a Blazor web application, a console application, and an AI engine library with unit tests.

## Project Structure

The repository contains four main projects:

- **Connect4AIEngine**: Core game logic library containing the board state and AI engine
- **BlazorApp**: ASP.NET Core Blazor Server web application for playing against AI in a browser
- **Connect4Console**: Console application for command-line gameplay
- **ConnectAITests**: NUnit test project for the AI engine

## Build and Run Commands

### Build the entire solution
```bash
dotnet build
```

### Run tests
```bash
dotnet test
```

### Run the Blazor web application
```bash
cd BlazorApp
dotnet run
```

### Run the console application
```bash
cd Connect4Console
dotnet run
```

### Build for release
```bash
dotnet build --configuration Release
```

### Publish the application
```bash
dotnet publish -c Release -o ./publish
```

## Development Commands

### Run specific test by name
```bash
dotnet test --filter "FullyQualifiedName~BasicForkDetection01"
```

### Run tests in a specific project
```bash
dotnet test ConnectAITests/ConnectAITests.csproj
```

### Watch mode for continuous testing
```bash
dotnet watch test
```

### Watch mode for Blazor development
```bash
cd BlazorApp
dotnet watch run
```

## Architecture

### Core Game Engine (Connect4AIEngine)

The AI engine uses a **NegaMax algorithm with alpha-beta pruning** for move evaluation:

- **Board.cs**: Manages the 7x6 game board state, win detection (horizontal, vertical, diagonal), and move validation
  - Board dimensions: 7 columns × 6 rows
  - Supports board cloning for AI lookahead
  - Implements "deadly spot" detection for tactical evaluation

- **GameEngine.cs**: Implements the AI decision-making logic
  - `NegaMax()`: Main entry point for AI move calculation with configurable depth
  - `GetBestMoveBasic()`: Fast heuristic for immediate win/block detection
  - `NegaMaxWorker()`: Recursive minimax implementation with progress reporting
  - `Eval()`: Static position evaluation based on deadly spots (threatening positions)
  - Win value constant: `WinValue = 100`

### AI Evaluation Strategy

1. **Level 1 (Basic)**: Check for immediate wins or blocks
2. **Deep Analysis**: When basic moves insufficient, run NegaMax to specified depth
3. **Fallback**: If facing inevitable loss, try lower depth to find non-losing moves
4. **Move Ordering**: Prioritizes center columns for better alpha-beta pruning

### Blazor Web Application (BlazorApp)

- **Connect4.razor** (BlazorApp/Components/Pages/Connect4.razor): Main game interface
  - Interactive Server rendering mode
  - Progress bar for AI analysis visualization
  - Undo functionality for player moves
  - Configurable AI difficulty level (depth parameter)
  - Async AI computation to prevent UI blocking
  - Analysis timeout: 1 minute maximum
  - Uses image assets (blue.jpg, red.jpg, empty.jpg) for disk visualization

### Console Application (Connect4Console)

- Simple text-based interface for playing against AI
- Default AI depth: 12 moves
- Supports undo ('B' command) and quit ('E' command)
- Real-time progress indication during AI thinking

## Game State Representation

The game uses a string notation for move history:
- Format: `{Player}{Column}{Player}{Column}...`
- Example: `"B3R3B3R3B2R3B2"` means Blue column 3, Red column 3, Blue column 3, etc.
- Used in tests and board reconstruction

## Testing

Tests are located in ConnectAITests and use NUnit framework:
- **BoardTests.cs**: Board state and win detection tests
- **GameEngineTests.cs**: AI move selection and tactical tests
  - Tests include fork detection scenarios
  - Tests verify forced win recognition
  - Uses game history strings to set up specific board states

## Deployment

The project includes GitHub Actions workflows for Azure deployment:
- **main_connect4ai.yml**: Builds and deploys to Azure Web App
- Target framework: .NET 8
- Build runs on Windows runners

## Important Implementation Details

- The AI uses **asynchronous evaluation** in the Blazor app (`Task.Run()`) to prevent blocking the UI thread
- Progress reporting is implemented via `IProgressReport` interface with different implementations:
  - `DoNothingProgressReport`: Silent mode for internal calculations
  - `ConsoleProgressReport`: Console output with dots
  - `BrowserProgressReport`: Updates Blazor UI with progress percentage
- Move ordering prioritizes center columns: `[3, 2, 4, 1, 5, 0, 6]`
- The board uses coordinate system: `board[column, row]` where row 0 is top, row 5 is bottom
- Win detection is optimized with "fast" methods that only check populated regions
