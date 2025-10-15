# BlazorSignalStore

[![NuGet Version](https://img.shields.io/nuget/v/BlazorSignalStore.svg)](https://www.nuget.org/packages/BlazorSignalStore)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorSignalStore.svg)](https://www.nuget.org/packages/BlazorSignalStore)
[![GitHub License](https://img.shields.io/github/license/TempooDev/BlazorSignalStore)](LICENSE)
[![Build Status](https://github.com/TempooDev/BlazorSignalStore/workflows/CI/badge.svg)](https://github.com/TempooDev/BlazorSignalStore/actions)

> A reactive state management library for Blazor applications inspired by Angular Signals

## 🚀 Features

- ✅ **Angular Signals-inspired API** - Familiar reactive patterns for Angular developers
- ✅ **React-like Hooks** - `useSignal()` and function syntax support for React developers  
- ✅ **Automatic UI Updates** - Components re-render automatically when signals change
- ✅ **Computed Signals** - Derived values that update when dependencies change
- ✅ **Type Safe** - Full TypeScript-like experience with nullable reference types
- ✅ **Dependency Injection** - Native .NET DI integration
- ✅ **Blazor Server & WebAssembly** - Works with both hosting models
- ✅ **.NET 8+ Ready** - Built for modern .NET applications

## 📦 Installation

```bash
dotnet add package BlazorSignalStore
```

## 🔧 Quick Start

### 1. Register Services

```csharp
// Program.cs
builder.Services.AddSignalStore<CounterStore>();
```

### 2. Create a Store

```csharp
using BlazorSignalStore.Core;

public class CounterStore : StoreBase
{
    public Signal<int> Count { get; } = new(0);
    public Computed<string> Label { get; }
    
    public CounterStore()
    {
        Label = new Computed<string>(() => $"Count: {Count.Value}", Count);
    }
    
    public void Increment() => Count.Value++;
    public void Decrement() => Count.Value--;
}
```

### 3. Use in Components

```razor
@using BlazorSignalStore.Core
@inject CounterStore Store

<h3>@label()</h3>
<button @onclick="Store.Increment">+</button>
<button @onclick="Store.Decrement">-</button>

@code {
    private Func<int>? count;
    private Func<string>? label;
    
    protected override void OnInitialized()
    {
        count = this.useSignal(Store.Count);
        label = this.useSignal(Store.Label);
    }
}
```

## 💡 Core Concepts

### Signals
Reactive containers that notify subscribers when their value changes:

```csharp
// Create a signal
var signal = new Signal<string>("initial value");

// Subscribe to changes
var subscription = signal.Subscribe(newValue => 
    Console.WriteLine($"Value changed to: {newValue}"));

// Update value (triggers subscribers)
signal.Value = "new value";

// Clean up
subscription.Dispose();
```

### Computed Signals
Derived values that automatically recalculate when dependencies change:

```csharp
var firstName = new Signal<string>("John");
var lastName = new Signal<string>("Doe");

var fullName = new Computed<string>(() => $"{firstName.Value} {lastName.Value}");

Console.WriteLine(fullName.Value); // "John Doe"

firstName.Value = "Jane"; 
Console.WriteLine(fullName.Value); // "Jane Doe" (automatically updated)
```

### Blazor Integration
Use the `useSignal` extension method for automatic component re-rendering:

```csharp
@using BlazorSignalStore.Core
@inject MyStore Store

@code {
    private Func<bool>? isLoading;
    
    protected override void OnInitialized()
    {
        isLoading = this.useSignal(Store.IsLoading);
        
        // Component automatically re-renders when isLoading changes
    }
}
```

## 📋 API Reference

### Signal&lt;T&gt;
- `Value` - Get/set the current value
- `Subscribe(Action<T>)` - Subscribe to value changes
- Implicit conversion to `Func<T>` for React-like syntax

### Computed&lt;T&gt;  
- `Value` - Get the computed value (readonly)
- `Subscribe(Action<T>)` - Subscribe to computed value changes
- Constructor takes computation function and dependency signals

### SignalHooks Extensions
- `useSignal<T>(Signal<T>)` - Create a function that triggers component re-renders
- `useSignal<T>(Computed<T>)` - Subscribe to computed signals in components
- Integrates with Blazor component lifecycle

### StoreBase
- Base class for creating organized signal stores
- Inherit from this class to group related signals and computed values

## 🎯 Patterns

### Store-based Architecture
```csharp
public class CounterStore : StoreBase
{
    public Signal<int> Count { get; } = new(0);
    public Computed<string> DisplayText { get; }
    
    public CounterStore()
    {
        DisplayText = new Computed<string>(() => $"Count: {Count.Value}", Count);
    }
}

// In component
@inject CounterStore Store

<p>@displayText()</p>

@code {
    private Func<string>? displayText;
    
    protected override void OnInitialized()
    {
        displayText = this.useSignal(Store.DisplayText);
    }
}
```

### Computed Dependencies
```csharp
public class ItemStore : StoreBase
{
    public Signal<List<Item>> Items { get; } = new(new());
    public Computed<int> ItemCount { get; }
    public Computed<bool> HasItems { get; }
    
    public ItemStore()
    {
        ItemCount = new Computed<int>(() => Items.Value.Count, Items);
        HasItems = new Computed<bool>(() => ItemCount.Value > 0, ItemCount);
    }
}
```

### Complex State Management
```csharp
public class AppStore : StoreBase
{
    public Signal<User?> CurrentUser { get; } = new(null);
    public Signal<bool> IsLoading { get; } = new(false);
    public Computed<bool> IsAuthenticated { get; }
    
    public AppStore()
    {
        IsAuthenticated = new Computed<bool>(() => CurrentUser.Value != null, CurrentUser);
    }
}
```

## 🏗️ Architecture

BlazorSignalStore is built with:
- **Signal&lt;T&gt;** - Core reactive primitive with subscription management
- **Computed&lt;T&gt;** - Derived state with explicit dependency tracking  
- **useSignal Extensions** - Blazor component integration methods
- **StoreBase** - Base class for organizing related signals
- **Reflection-based updates** - Automatic `StateHasChanged()` calls via `useSignal`
- **Dependency Injection** - Native .NET DI integration via `AddSignalStore<T>()`

## 🧪 Testing

The library includes comprehensive tests covering:
- Core signal functionality (11 tests) ✅
- Dependency injection integration (5 tests) ✅  
- Computed signal behavior
- Blazor component integration
- Memory leak prevention
- Thread safety

Run tests:
```bash
dotnet test
```

## 📚 Documentation

- [Getting Started Guide](docs/getting-started.md)
- [API Reference](docs/api-reference.md)
- [Sample Applications](samples/)

## 🤝 Contributing

Contributions are welcome! Please read our [contribution guidelines](CONTRIBUTING.md).

## 📄 License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for more details.

## 🙏 Acknowledgments

- Inspired by [Angular Signals](https://angular.io/guide/signals)
- Built for the [Blazor](https://blazor.net/) ecosystem
- Follows [React Hooks](https://reactjs.org/docs/hooks-intro.html) patterns