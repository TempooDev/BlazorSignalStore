# Sample Projects

This directory contains sample applications demonstrating various use cases for BlazorSignalStore.

## Available Samples

### Basic Counter (Planned)
`/BasicCounter/`
- Simple shared counter application
- Demonstrates basic state synchronization
- Perfect for getting started

### Chat Application (Planned)
`/ChatApp/`
- Real-time chat with multiple rooms
- User presence indicators
- Message history
- Demonstrates complex state management

### Collaborative Todo List (Planned)
`/CollaborativeTodo/`
- Multi-user todo list
- Real-time updates when items are added/completed
- User assignments and notifications
- Shows optimistic updates

### Live Dashboard (Planned)
`/LiveDashboard/`
- Real-time data visualization
- Multiple chart types
- Data broadcasting from server
- Demonstrates one-way data flow

### Gaming Example (Planned)
`/SimpleGame/`
- Simple multiplayer game
- Player movements and interactions
- Game state synchronization
- High-frequency updates

## Running the Samples

Each sample includes:
- `README.md` - Specific setup instructions
- `Program.cs` - Configuration examples
- Multiple projects (Client/Server for WebAssembly samples)

### Prerequisites
- .NET 8.0 SDK
- BlazorSignalStore NuGet package (or reference to local build)

### General Steps
1. Navigate to the sample directory
2. Run `dotnet restore`
3. Run `dotnet run` (or follow sample-specific instructions)
4. Open multiple browser windows to see real-time sync

## Sample Structure

```
samples/
├── BasicCounter/
│   ├── BasicCounter.csproj
│   ├── Program.cs
│   ├── Pages/
│   └── README.md
├── ChatApp/
│   ├── ChatApp.Client/
│   ├── ChatApp.Server/
│   └── README.md
└── ...
```

## Contributing Samples

Have an idea for a sample? We'd love to include it! Please:

1. Follow the existing sample structure
2. Include comprehensive README.md
3. Add comments explaining key concepts
4. Test with multiple browser instances
5. Submit a pull request

Ideas for new samples:
- Form validation with real-time collaboration
- Drawing/whiteboard application
- Music player with synchronized playlists
- Code editor with real-time collaboration
- Shopping cart with inventory sync

## Learning Path

We recommend exploring samples in this order:

1. **BasicCounter** - Learn the fundamentals
2. **CollaborativeTodo** - Understand complex state updates
3. **ChatApp** - See real-world application patterns
4. **LiveDashboard** - Explore one-way data flows
5. **SimpleGame** - Handle high-frequency updates

Each sample builds on concepts from the previous ones while introducing new patterns and techniques.