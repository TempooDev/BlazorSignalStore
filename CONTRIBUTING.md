# Contributing to BlazorSignalStore

Thank you for your interest in contributing to BlazorSignalStore! We welcome contributions from the community.

## Code of Conduct

This project adheres to a code of conduct. By participating, you are expected to uphold this code.

## How to Contribute

### Reporting Issues

Before creating an issue, please:
1. Check if the issue already exists
2. Use the provided issue templates
3. Include as much detail as possible

### Contributing Code

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**
   - Follow the coding standards
   - Add tests for new functionality
   - Update documentation as needed

4. **Test your changes**
   ```bash
   dotnet test
   ```

5. **Commit your changes**
   - Use conventional commit messages
   - Example: `feat: add new state synchronization feature`

6. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **Create a Pull Request**

## Development Setup

### Prerequisites

- .NET 8.0 SDK or later
- Git
- Your favorite IDE (Visual Studio, VS Code, Rider)

### Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/TempooDev/BlazorSignalStore.git
   cd BlazorSignalStore
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run tests:
   ```bash
   dotnet test
   ```

## Coding Standards

### General Guidelines

- Use meaningful names for variables, methods, and classes
- Follow C# naming conventions
- Add XML documentation for public APIs
- Keep methods small and focused
- Write unit tests for new functionality

### Code Formatting

We use the built-in .NET formatting rules. Run the following before submitting:

```bash
dotnet format
```

### Testing

- All public methods should have unit tests
- Aim for high test coverage
- Use descriptive test names
- Follow the Arrange-Act-Assert pattern

## Pull Request Guidelines

### Before Submitting

- [ ] Code builds without warnings
- [ ] All tests pass
- [ ] Code is properly formatted
- [ ] Documentation is updated
- [ ] Breaking changes are documented

### PR Requirements

- Clear description of changes
- Link to related issues
- Screenshots/examples for UI changes
- Breaking changes noted in description

## Release Process

1. Version numbers follow [Semantic Versioning](https://semver.org/)
2. Releases are created from the `main` branch
3. Release notes are generated automatically
4. NuGet packages are published automatically on release

## Getting Help

- Check the [documentation](docs/)
- Look through existing [issues](https://github.com/TempooDev/BlazorSignalStore/issues)
- Create a new issue with the question label

## Recognition

Contributors will be recognized in:
- Release notes
- Contributors section of README
- Package acknowledgments

Thank you for contributing! 🎉