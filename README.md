# Connect4 AI Game

A Connect Four game implementation with an intelligent AI opponent, built with ASP.NET Core Blazor and C#. Features smooth animations, configurable difficulty levels, and a clean web interface.

## 🎮 Live Demo

Play online: [connect4ai2.azurewebsites.net](https://connect4ai2.azurewebsites.net/connect4)

## 🏗️ Architecture

The project consists of three main components:

### **Connect4AIEngine** - Core Game Logic
- **Board.cs**: Manages the 7×6 game board, win detection (horizontal, vertical, diagonal), and move validation
- **GameEngine.cs**: Implements the AI using NegaMax algorithm with alpha-beta pruning
- Supports board cloning for AI lookahead and evaluation

### **BlazorApp** - Web Interface
- ASP.NET Core Blazor Server application
- Interactive game board with animated disk drops (gravity + bounce effect)
- Winning sequence flash animation (scale pulse + brightness)
- Real-time AI analysis progress indicator
- Configurable AI difficulty (search depth)

### **Connect4Console** - CLI Interface
- Simple text-based interface for terminal play
- Supports undo and quit commands
- Uses the same AI engine as the web app

## 🤖 AI Algorithm

The AI uses **NegaMax with Alpha-Beta Pruning** - a variant of the minimax algorithm optimized for two-player zero-sum games.

### How It Works

1. **Fast Heuristic Check**: First tries `GetBestMoveBasic()` to detect immediate wins or necessary blocks
2. **Deep Analysis**: If no forced move, runs NegaMax to specified depth (default: 7-12 moves)
3. **Position Evaluation**: Uses "deadly spots" (threatening positions) to score board states
4. **Move Ordering**: Prioritizes center columns `[3, 2, 4, 1, 5, 0, 6]` for better alpha-beta pruning

### Key Features

- **Alpha-Beta Pruning**: Dramatically reduces search space by eliminating branches that can't improve the result
- **Configurable Depth**: Higher depth = stronger AI but slower response
- **Progress Reporting**: Real-time feedback during AI thinking
- **Async Execution**: Non-blocking UI while AI calculates

### Performance

- **Win Value**: 100 points for winning position
- **Evaluation**: Based on number of "deadly spots" (positions that threaten a win)
- **Fallback Strategy**: If facing inevitable loss, tries lower depth to find best defensive move

## 🎨 Features

- **Smooth Animations**: Disks fall with realistic gravity and bounce effects
- **Winning Animation**: Winning 4-disk sequence pulses with scale and brightness effects
- **Undo Move**: Take back your last move (Blue player only)
- **Configurable AI**: Adjust difficulty from 1-15 (higher = harder)
- **Responsive Design**: Works on desktop and mobile browsers

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK

### Run Locally

```bash
# Clone the repository
git clone https://github.com/arturl/Connect4.git
cd Connect4

# Run the web app
cd BlazorApp
dotnet run

# Or run the console version
cd Connect4Console
dotnet run
```

Navigate to `https://localhost:5001/connect4` to play!

### Run Tests

```bash
dotnet test ConnectAITests/ConnectAITests.csproj
```

## 📝 Game Rules

- Players alternate dropping colored disks into a 7-column, 6-row grid
- Disks fall to the lowest available position in the selected column
- Win by connecting 4 disks horizontally, vertically, or diagonally
- Blue (human) plays first, Red (AI) responds

## 🛠️ Technical Details

- **Framework**: .NET 8, ASP.NET Core Blazor Server
- **Language**: C# 12
- **Testing**: NUnit
- **Deployment**: Azure App Service
- **CI/CD**: GitHub Actions

## 📄 License

MIT License - feel free to use and modify!

---

Built with ❤️ using .NET and Blazor
