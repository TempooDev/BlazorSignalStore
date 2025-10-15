# Getting Started with BlazorSignalStore

This guide will help you get started with BlazorSignalStore, a reactive state management library for Blazor applications inspired by Angular Signals.

## Installation

Install the NuGet package in your Blazor project:

```bash
dotnet add package BlazorSignalStore
```

## Quick Setup

### 1. Configure Services

In your `Program.cs` file, register your signal stores:

```csharp
using BlazorSignalStore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add your stores
builder.Services.AddSignalStore<CounterStore>();
builder.Services.AddSignalStore<UserStore>();

// Or for Blazor Server:
// builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();
// builder.Services.AddSignalStore<CounterStore>();

var app = builder.Build();
await app.RunAsync();
```

### 2. Create a Store

Create a store class that inherits from `StoreBase`:

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

Inject your store and use signals in Blazor components:

```razor
@page "/counter"
@using BlazorSignalStore.Core
@inject CounterStore Store

<h3>@label()</h3>
<button @onclick="Store.Increment">+</button>
<button @onclick="Store.Decrement">-</button>
<p>Value: @count()</p>

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

That's it! Your component will automatically re-render whenever the signal value changes.

## Core Concepts

### Signals

Signals are reactive containers that notify subscribers when their value changes:

```csharp
// Create a signal
var message = new Signal<string>("Hello World");

// Subscribe to changes
var subscription = message.Subscribe(newValue => 
    Console.WriteLine($"Message changed: {newValue}"));

// Update the value (triggers subscribers)
message.Value = "Hello BlazorSignalStore!";

// Clean up
subscription.Dispose();
```

### Computed Signals

Computed signals derive their value from other signals and automatically update:

```csharp
var firstName = new Signal<string>("John");
var lastName = new Signal<string>("Doe");

var fullName = new Computed<string>(() => 
    $"{firstName.Value} {lastName.Value}");

Console.WriteLine(fullName.Value); // "John Doe"

firstName.Value = "Jane";
Console.WriteLine(fullName.Value); // "Jane Doe" (automatically updated)
```

### Blazor Integration

Use the `useSignal` extension method for automatic component re-rendering:

```razor
@using BlazorSignalStore.Core
@inject MyStore Store

@code {
    private Func<bool>? isLoading;
    private Func<List<string>>? items;
    
    protected override void OnInitialized()
    {
        isLoading = this.useSignal(Store.IsLoading);
        items = this.useSignal(Store.Items);
    }
    
    private async Task LoadItems()
    {
        Store.IsLoading.Value = true;
        try
        {
            // Simulate API call
            await Task.Delay(1000);
            Store.Items.Value = new List<string> { "Item 1", "Item 2", "Item 3" };
        }
        finally
        {
            Store.IsLoading.Value = false;
        }
    }
}
```

## Advanced Examples

### State Management

Create a comprehensive state management solution:

```csharp
// AppState.cs
using BlazorSignalStore.Core;

public class AppState : StoreBase
{
    public Signal<User?> CurrentUser { get; } = new(null);
    public Signal<bool> IsLoading { get; } = new(false);
    public Signal<List<Notification>> Notifications { get; } = new(new());
    
    public Computed<bool> IsAuthenticated { get; }
    public Computed<int> UnreadCount { get; }
    
    public AppState()
    {
        IsAuthenticated = new Computed<bool>(() => CurrentUser.Value != null, CurrentUser);
        UnreadCount = new Computed<int>(() => 
            Notifications.Value.Count(n => !n.IsRead), Notifications);
    }
}
```

Register as a store:

```csharp
// Program.cs
builder.Services.AddSignalStore<AppState>();
```

Use in components:

```razor
@using BlazorSignalStore.Core
@inject AppState State

@if (isAuthenticated())
{
    <p>Welcome, @currentUser()?.Name!</p>
    
    @if (unreadCount() > 0)
    {
        <div class="alert alert-info">
            You have @unreadCount() unread notifications
        </div>
    }
}
else
{
    <p>Please log in</p>
}

@code {
    private Func<bool>? isAuthenticated;
    private Func<User?>? currentUser;
    private Func<int>? unreadCount;
    
    protected override void OnInitialized()
    {
        isAuthenticated = this.useSignal(State.IsAuthenticated);
        currentUser = this.useSignal(State.CurrentUser);
        unreadCount = this.useSignal(State.UnreadCount);
    }
}
```

### Shopping Cart Example

```csharp
// ShoppingCartStore.cs
using BlazorSignalStore.Core;

public class ShoppingCartStore : StoreBase
{
    public Signal<List<Product>> Products { get; } = new(new List<Product>
    {
        new("Laptop", 999.99m),
        new("Mouse", 29.99m),
        new("Keyboard", 79.99m)
    });
    
    public Signal<List<Product>> CartItems { get; } = new(new());
    public Computed<decimal> CartTotal { get; }
    
    public ShoppingCartStore()
    {
        CartTotal = new Computed<decimal>(() => 
            CartItems.Value.Sum(p => p.Price), CartItems);
    }
    
    public void AddToCart(Product product)
    {
        var currentItems = new List<Product>(CartItems.Value) { product };
        CartItems.Value = currentItems;
    }
}

public record Product(string Name, decimal Price);
```

```razor
@using BlazorSignalStore.Core
@inject ShoppingCartStore Store

<h3>Shopping Cart</h3>

<div class="row">
    <div class="col-md-8">
        <h4>Products</h4>
        @foreach (var product in products())
        {
            <div class="card mb-2">
                <div class="card-body">
                    <h5>@product.Name - $@product.Price</h5>
                    <button class="btn btn-primary" 
                            @onclick="() => Store.AddToCart(product)">
                        Add to Cart
                    </button>
                </div>
            </div>
        }
    </div>
    
    <div class="col-md-4">
        <h4>Cart (@cartItems().Count items)</h4>
        <p><strong>Total: $@cartTotal()</strong></p>
        
        @foreach (var item in cartItems())
        {
            <div class="d-flex justify-content-between">
                <span>@item.Name</span>
                <span>$@item.Price</span>
            </div>
        }
    </div>
</div>

@code {
    private Func<List<Product>>? products;
    private Func<List<Product>>? cartItems;
    private Func<decimal>? cartTotal;
    
    protected override void OnInitialized()
    {
        products = this.useSignal(Store.Products);
        cartItems = this.useSignal(Store.CartItems);
        cartTotal = this.useSignal(Store.CartTotal);
    }
}
```

## Performance Tips

### 1. Use Computed for Derived Values

Instead of recalculating values in the UI, use computed signals in your store:

```csharp
// ❌ Don't do this in the component
<p>Total: @Store.Items.Value.Sum(x => x.Price)</p>

// ✅ Do this - create computed in store
public class MyStore : StoreBase 
{
    public Signal<List<Item>> Items { get; } = new(new());
    public Computed<decimal> TotalPrice { get; }
    
    public MyStore()
    {
        TotalPrice = new Computed<decimal>(() => 
            Items.Value.Sum(x => x.Price), Items);
    }
}

// Then use in component
<p>Total: @totalPrice()</p>

@code {
    private Func<decimal>? totalPrice;
    
    protected override void OnInitialized()
    {
        totalPrice = this.useSignal(Store.TotalPrice);
    }
}
```

### 2. Batch Updates

For multiple related updates, use a single state object in your store:

```csharp
// ❌ These will trigger separate re-renders
Store.UserName.Value = name;
Store.UserEmail.Value = email;

// ✅ Better: Use a single signal with a record/object
public class UserStore : StoreBase
{
    public Signal<UserProfile> Profile { get; } = new(new("", ""));
    
    public void UpdateProfile(string name, string email)
    {
        Profile.Value = Profile.Value with { Name = name, Email = email };
    }
}

public record UserProfile(string Name, string Email);
```

### 3. Memory Management

The `useSignal` extension automatically manages subscriptions for component lifecycle, but if you manually subscribe to signals, always dispose:

```csharp
public class MyService : IDisposable
{
    private readonly List<IDisposable> _subscriptions = new();
    
    public MyService(MyStore store)
    {
        _subscriptions.Add(store.SomeSignal.Subscribe(HandleChange));
    }
    
    public void Dispose()
    {
        foreach (var sub in _subscriptions)
            sub.Dispose();
    }
    
    private void HandleChange(string data) { }
}
```

## Next Steps

- Explore the [API Reference](api-reference.md) for detailed documentation
- Check out the [samples](../samples/) for more complex examples
- Learn about advanced patterns and best practices
- Contribute to the project on [GitHub](https://github.com/TempooDev/BlazorSignalStore)