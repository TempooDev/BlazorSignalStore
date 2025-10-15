# API Reference

BlazorSignalStore provides a reactive state management system inspired by Angular Signals with React-like hooks integration for Blazor applications.

## Core Classes

### Signal&lt;T&gt;

A reactive container that notifies subscribers when its value changes.

#### Constructor

```csharp
public Signal(T initialValue)
```

Creates a new signal with the specified initial value.

#### Properties

```csharp
public T Value { get; set; }
```

Gets or sets the current value. Setting a new value automatically notifies all subscribers.

#### Methods

```csharp
public IDisposable Subscribe(Action<T> observer)
```

Subscribes to value changes.

**Parameters:**
- `observer` - Callback invoked when the value changes

**Returns:** IDisposable to unsubscribe

#### Operators

```csharp
public static implicit operator Func<T>(Signal<T> signal)
```

Enables React-like function syntax: `signal()` instead of `signal.Value`.

#### Example

```csharp
var counter = new Signal<int>(0);

// Subscribe to changes
var subscription = counter.Subscribe(value => 
    Console.WriteLine($"Counter: {value}"));

// Update value
counter.Value = 5; // Prints: "Counter: 5"

// Function syntax
Func<int> getCounter = counter;
Console.WriteLine(getCounter()); // Prints: 5

// Cleanup
subscription.Dispose();
```

### Computed&lt;T&gt;

A read-only signal that derives its value from other signals and automatically updates when dependencies change.

#### Constructor

```csharp
public Computed(Func<T> computation)
```

Creates a computed signal with the specified computation function.

#### Properties

```csharp
public T Value { get; }
```

Gets the computed value (read-only).

#### Methods

```csharp
public IDisposable Subscribe(Action<T> observer)
```

Subscribes to computed value changes.

#### Example

```csharp
var firstName = new Signal<string>("John");
var lastName = new Signal<string>("Doe");

var fullName = new Computed<string>(() => 
    $"{firstName.Value} {lastName.Value}");

Console.WriteLine(fullName.Value); // "John Doe"

firstName.Value = "Jane";
Console.WriteLine(fullName.Value); // "Jane Doe"
```

### SignalHooks

Provides Blazor component integration for automatic re-rendering when signals change.

#### Methods

```csharp
public Signal<T> UseSignal<T>(T initialValue)
```

Creates a signal that automatically triggers component re-renders when its value changes.

**Parameters:**
- `initialValue` - The initial value for the signal

**Returns:** A new Signal&lt;T&gt; instance

#### Example

```razor
@inject SignalHooks Signals

<p>Count: @count()</p>
<button @onclick="Increment">+</button>

@code {
    private Signal<int> count = default!;
    
    protected override void OnInitialized()
    {
        count = Signals.UseSignal(0);
    }
    
    private void Increment() => count.Value++;
}
```

## Extension Methods

### ServiceCollectionExtensions

#### AddBlazorSignalStore

```csharp
public static IServiceCollection AddBlazorSignalStore(this IServiceCollection services)
```

Registers BlazorSignalStore services with the dependency injection container.

**Parameters:**
- `services` - The service collection

**Returns:** The service collection for chaining

#### Example

```csharp
// Program.cs
builder.Services.AddBlazorSignalStore();
```

## Patterns and Best Practices

### Component Integration

Use `SignalHooks` in your Blazor components for automatic UI updates:

```razor
@inject SignalHooks Signals

@code {
    private Signal<bool> isLoading = default!;
    private Signal<string> message = default!;
    
    protected override void OnInitialized()
    {
        isLoading = Signals.UseSignal(false);
        message = Signals.UseSignal("Hello World");
    }
    
    private async Task LoadData()
    {
        isLoading.Value = true;
        try
        {
            // Load data...
            message.Value = "Data loaded successfully";
        }
        finally
        {
            isLoading.Value = false;
        }
    }
}
```

### State Management

Create complex state objects using signals:

```csharp
public class AppState
{
    public Signal<User?> CurrentUser { get; } = new(null);
    public Signal<bool> IsLoading { get; } = new(false);
    public Signal<List<Notification>> Notifications { get; } = new(new());
    
    public Computed<bool> IsAuthenticated { get; }
    public Computed<int> NotificationCount { get; }
    
    public AppState()
    {
        IsAuthenticated = new Computed<bool>(() => CurrentUser.Value != null);
        NotificationCount = new Computed<int>(() => Notifications.Value.Count);
    }
}
```

### Memory Management

Always dispose of subscriptions to prevent memory leaks:

```csharp
public class MyService : IDisposable
{
    private readonly List<IDisposable> _subscriptions = new();
    
    public MyService()
    {
        var signal = new Signal<string>("test");
        
        _subscriptions.Add(signal.Subscribe(value => 
        {
            // Handle value change
        }));
    }
    
    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }
        _subscriptions.Clear();
    }
}
```

### Function Syntax

Use the implicit conversion for React-like patterns:

```csharp
// Traditional approach
<p>Value: @signal.Value</p>

// Function syntax (React-like)
<p>Value: @signal()</p>

// In code
var getValue = (Func<string>)signal;
string currentValue = getValue();
```

**Example:**
```csharp
var counterState = store.GetState<CounterState>();
```

#### UpdateStateAsync<T>

```csharp
Task UpdateStateAsync<T>(Func<T, T> updater, bool broadcast = true) where T : class, new()
```

Updates the state using the provided updater function.

**Type Parameters:**
- `T` - The state type

**Parameters:**
- `updater` - Function that receives current state and returns updated state
- `broadcast` - Whether to broadcast the change via SignalR (default: true)

**Returns:** A task representing the asynchronous operation

**Example:**
```csharp
await store.UpdateStateAsync<CounterState>(state =>
{
    state.Count++;
    return state;
});
```

#### Subscribe<T>

```csharp
IDisposable Subscribe<T>(Action<T> callback) where T : class, new()
```

Subscribes to state changes of the specified type.

**Type Parameters:**
- `T` - The state type

**Parameters:**
- `callback` - Action to execute when state changes

**Returns:** IDisposable to unsubscribe

**Example:**
```csharp
var subscription = store.Subscribe<CounterState>(state =>
{
    Console.WriteLine($"Count is now: {state.Count}");
});

// Later...
subscription.Dispose();
```

### Properties

#### IsConnected

```csharp
bool IsConnected { get; }
```

Gets a value indicating whether the store is connected to the SignalR hub.

### Events

#### ConnectionStateChanged

```csharp
event Action<bool> ConnectionStateChanged
```

Event that fires when the connection state changes.

**Example:**
```csharp
store.ConnectionStateChanged += isConnected =>
{
    if (isConnected)
    {
        Console.WriteLine("Connected to hub");
    }
    else
    {
        Console.WriteLine("Disconnected from hub");
    }
};
```

## ServiceCollectionExtensions

Extension methods for registering BlazorSignalStore in the dependency injection container.

### AddBlazorSignalStore (with URL)

```csharp
public static IServiceCollection AddBlazorSignalStore(
    this IServiceCollection services,
    string hubUrl,
    Action<IHubConnectionBuilder>? configureConnection = null)
```

Registers BlazorSignalStore with a simple hub URL.

**Parameters:**
- `services` - The service collection
- `hubUrl` - The SignalR hub URL
- `configureConnection` - Optional hub connection configuration

**Returns:** The service collection for method chaining

**Example:**
```csharp
services.AddBlazorSignalStore("https://localhost:5001/signalhub");
```

**With configuration:**
```csharp
services.AddBlazorSignalStore("https://localhost:5001/signalhub", builder =>
{
    builder.WithAutomaticReconnect();
});
```

### AddBlazorSignalStore (with factory)

```csharp
public static IServiceCollection AddBlazorSignalStore(
    this IServiceCollection services,
    Func<IServiceProvider, HubConnection> configureConnection)
```

Registers BlazorSignalStore with a custom hub connection factory.

**Parameters:**
- `services` - The service collection
- `configureConnection` - Factory function to create HubConnection

**Returns:** The service collection for method chaining

**Example:**
```csharp
services.AddBlazorSignalStore(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new HubConnectionBuilder()
        .WithUrl(config.GetConnectionString("SignalRHub"))
        .WithAutomaticReconnect()
        .Build();
});
```

## State Class Requirements

State classes used with BlazorSignalStore must meet these requirements:

### 1. Reference Type

```csharp
// ✅ Correct - class
public class CounterState
{
    public int Count { get; set; }
}

// ❌ Incorrect - struct
public struct CounterState // Won't work
{
    public int Count { get; set; }
}
```

### 2. Parameterless Constructor

```csharp
// ✅ Correct - has parameterless constructor
public class UserState
{
    public string Name { get; set; } = string.Empty;
    
    public UserState() { } // Explicit parameterless constructor
    
    public UserState(string name) // Additional constructors are fine
    {
        Name = name;
    }
}

// ✅ Also correct - implicit parameterless constructor
public class SimpleState
{
    public int Value { get; set; }
}
```

### 3. Serializable Properties

```csharp
public class MessageState
{
    // ✅ Simple types
    public string Text { get; set; } = string.Empty;
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    
    // ✅ Collections
    public List<string> Tags { get; set; } = new();
    
    // ✅ Nested objects (if they're also serializable)
    public UserInfo Author { get; set; } = new();
    
    // ❌ Avoid - not serializable
    // public Action OnClick { get; set; }
    // public Task SomeTask { get; set; }
}
```

## Error Handling

### Connection Errors

```csharp
try
{
    await store.ConnectAsync();
}
catch (HttpRequestException ex)
{
    // Handle connection failure
    logger.LogError(ex, "Failed to connect to SignalR hub");
}
catch (InvalidOperationException ex)
{
    // Handle invalid operation (e.g., already connected)
    logger.LogWarning(ex, "Store is already connected");
}
```

### State Update Errors

```csharp
try
{
    await store.UpdateStateAsync<CounterState>(state =>
    {
        if (state.Count < 0)
            throw new InvalidOperationException("Count cannot be negative");
        
        state.Count++;
        return state;
    });
}
catch (InvalidOperationException ex)
{
    // Handle business logic errors
    logger.LogWarning(ex, "State update failed validation");
}
catch (Exception ex)
{
    // Handle unexpected errors
    logger.LogError(ex, "Unexpected error updating state");
}
```

## Performance Considerations

### State Size

Keep state objects reasonably sized for efficient serialization:

```csharp
// ✅ Good - focused state
public class CounterState
{
    public int Count { get; set; }
    public string LastUpdatedBy { get; set; } = string.Empty;
}

// ❌ Avoid - too much data
public class MassiveState
{
    public byte[] LargeFile { get; set; } = Array.Empty<byte>();
    public List<ComplexObject> ThousandsOfItems { get; set; } = new();
}
```

### Update Frequency

Avoid rapid-fire updates that could overwhelm the SignalR connection:

```csharp
// ❌ Avoid - updating on every mouse move
private async Task OnMouseMove(MouseEventArgs e)
{
    await store.UpdateStateAsync<MouseState>(state =>
    {
        state.X = e.ClientX;
        state.Y = e.ClientY;
        return state;
    });
}

// ✅ Better - throttled updates
private readonly Timer _updateTimer = new(TimeSpan.FromMilliseconds(100));

private async Task OnMouseMove(MouseEventArgs e)
{
    _pendingMouseState.X = e.ClientX;
    _pendingMouseState.Y = e.ClientY;
    
    if (!_updateTimer.IsEnabled)
    {
        _updateTimer.Start();
        await FlushMouseState();
    }
}
```

## Thread Safety

BlazorSignalStore is thread-safe, but be aware of these considerations:

### Concurrent Updates

```csharp
// ✅ Safe - each update is atomic
await Task.WhenAll(
    store.UpdateStateAsync<CounterState>(state => { state.Count++; return state; }),
    store.UpdateStateAsync<CounterState>(state => { state.Count += 2; return state; })
);

// ❌ Potential issue - depends on external state
var currentCount = store.GetState<CounterState>().Count;
await store.UpdateStateAsync<CounterState>(state =>
{
    state.Count = currentCount + 1; // currentCount might be stale
    return state;
});
```

### Subscription Callbacks

```csharp
store.Subscribe<CounterState>(state =>
{
    // ✅ Safe - read operations
    var count = state.Count;
    
    // ✅ Safe - UI updates on UI thread
    InvokeAsync(() => StateHasChanged());
    
    // ❌ Avoid - modifying state in callback
    // state.Count++; // Don't do this!
    
    // ❌ Avoid - synchronous blocking operations
    // Thread.Sleep(1000); // Don't block the callback
});
```