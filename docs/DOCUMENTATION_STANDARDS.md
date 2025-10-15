# Code Style and Documentation Guidelines

## Language Standards

- **All code comments and documentation must be in English**
- **All XML documentation comments must be in English**
- **All README files and documentation must be in English**
- **All commit messages should be in English**

## XML Documentation Requirements

All public APIs in the **main library** must have complete XML documentation:

```csharp
/// <summary>
/// Brief description of what the method/class does.
/// </summary>
/// <typeparam name="T">Description of generic type parameter.</typeparam>
/// <param name="parameter">Description of the parameter.</param>
/// <returns>Description of what is returned.</returns>
public T ExampleMethod<T>(string parameter)
{
    // Implementation
}
```

### Projects Requiring XML Documentation

- ✅ **Main Library** (`src/BlazorSignalStore/`): Full XML documentation required
- ❌ **Test Projects** (`tests/`): XML documentation warnings suppressed with `<NoWarn>CS1591</NoWarn>`
- ❌ **Demo Projects** (`samples/`): XML documentation warnings suppressed with `<NoWarn>CS1591</NoWarn>`

### Suppressing XML Documentation Warnings

For projects that don't need XML documentation (tests, demos, examples), add this to the `.csproj` file:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);CS1591</NoWarn>
</PropertyGroup>
```

## Comment Guidelines

- Use clear, concise English
- Explain the "why", not just the "what"
- Use complete sentences with proper punctuation
- Be consistent with terminology throughout the codebase

## Examples

### ✅ Good Documentation

```csharp
/// <summary>
/// Represents an observable signal that automatically notifies subscribers when its value changes.
/// This is the core building block for reactive state management in Blazor applications.
/// </summary>
/// <typeparam name="T">The type of value held by the signal.</typeparam>
public class Signal<T>
{
    /// <summary>
    /// Gets or sets the current value of the signal.
    /// Setting a new value will notify all subscribers if the value has changed.
    /// </summary>
    public T Value { get; set; }
}
```

### ❌ Avoid

```csharp
// Spanish comments
/// <summary>
/// Representa una señal observable que notifica a los suscriptores...
/// </summary>

// Incomplete documentation
/// <summary>
/// Signal class
/// </summary>

// No documentation at all
public class Signal<T>
{
    public T Value { get; set; }
}
```

## Documentation Files

- **README.md**: English only, clear and professional
- **API Documentation**: Auto-generated from XML comments
- **Getting Started Guides**: Step-by-step in English
- **Examples**: Well-commented code samples in English

## Quality Standards

1. **Professional English**: Use proper grammar and spelling
2. **Technical Accuracy**: Ensure descriptions match implementation
3. **Consistency**: Use consistent terminology across all documentation
4. **Completeness**: Document all public APIs and important concepts
5. **Clarity**: Write for developers who may not be native English speakers