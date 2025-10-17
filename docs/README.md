# BlazorSignalStore Documentation

Welcome to the BlazorSignalStore documentation! This library provides a powerful state management solution for Blazor applications with real-time synchronization via SignalR.

## Table of Contents

- [Getting Started](getting-started.md) - Quick start guide and basic usage
- [API Reference](api-reference.md) - Complete API documentation
- [Advanced Scenarios](advanced-scenarios.md) - Complex use cases and patterns
- [Samples](../samples/) - Example applications and code samples

## What is BlazorSignalStore?

BlazorSignalStore is a state management library that combines local state management with real-time synchronization across multiple clients using SignalR. It's perfect for:

- **Collaborative Applications** - Multiple users working on the same data
- **Real-time Dashboards** - Live data updates across multiple screens
- **Chat Applications** - Message synchronization across clients
- **Live Gaming** - Real-time game state synchronization
- **Notification Systems** - Broadcasting updates to all connected clients

## Project Features

### 🔄 Project Structure
- **Source Code Organization** - Clean separation of concerns
- **Test Coverage** - Comprehensive unit test setup
- **Documentation** - Complete developer documentation

### 🌐 Distribution Ready
- **NuGet Packaging** - Configured for package distribution
- **GitHub Actions** - Automated CI/CD pipeline
- **Version Management** - Semantic versioning support

### 🎯 Development Ready
- **Build Configuration** - Optimized build settings
- **Code Quality** - Linting and formatting rules
- **Open Source** - MIT license and contribution guidelines

## Project Structure

This is a complete .NET open source project ready for development and distribution.

## Project Overview

```
├── src/
│   └── BlazorSignalStore/          # Main library project
├── tests/
│   └── BlazorSignalStore.Tests/    # Unit tests
├── docs/                           # Documentation
├── samples/                        # Example projects
├── .github/workflows/              # CI/CD automation
├── README.md                       # Project overview
├── LICENSE                         # MIT license
├── Directory.Build.props           # Build configuration
└── global.json                     # .NET SDK version
```

## When to Use BlazorSignalStore

### ✅ Great For:
- Applications with multiple concurrent users
- Real-time collaboration features
- Live data dashboards
- Chat and messaging systems
- Gaming applications
- Notification systems

### ❌ Consider Alternatives For:
- Single-user applications
- Applications that don't need real-time updates
- Simple form-based applications
- Applications with very large state objects
- High-frequency updates (> 100/second)

## Browser Support

BlazorSignalStore works in all modern browsers that support:
- WebAssembly (for Blazor WebAssembly)
- SignalR JavaScript client
- WebSockets or Server-Sent Events

This includes:
- Chrome 57+
- Firefox 52+
- Safari 11+
- Edge 16+

## Performance Considerations

- **State Size**: Keep state objects reasonably sized for efficient serialization
- **Update Frequency**: Avoid rapid-fire updates that could overwhelm the connection
- **Connection Management**: Handle connection state changes gracefully
- **Memory Management**: Always dispose of subscriptions to prevent memory leaks

## Getting Help

- 📖 **Documentation**: Browse these docs for detailed information
- 🐛 **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/TempooDev/BlazorSignalStore/issues)
- 💬 **Discussions**: Ask questions in [GitHub Discussions](https://github.com/TempooDev/BlazorSignalStore/discussions)
- 📧 **Email**: Contact the maintainers for private questions

## Contributing

We welcome contributions! Please see our [Contributing Guide](../CONTRIBUTING.md) for details on:
- Setting up the development environment
- Coding standards and guidelines
- Submitting pull requests
- Reporting issues

## License

BlazorSignalStore is licensed under the [MIT License](../LICENSE). Feel free to use it in your projects!

---

Ready to get started? Head over to the [Getting Started Guide](getting-started.md)!