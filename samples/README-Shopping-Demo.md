# Shopping Cart Demo - BlazorSignalStore

This demo showcases the power of **BlazorSignalStore** through a comprehensive shopping cart implementation where multiple components share the same reactive state.

## 🛒 Demo Features

### **Shared State Management**
- **Multiple components** automatically sync when the cart state changes
- **Real-time updates** across all UI components
- **Zero manual state management** - everything is reactive

### **Components Involved**

1. **Shopping.razor** - Main product listing page
2. **ShoppingCartSummary.razor** - Cart overview sidebar
3. **CartStatusBar.razor** - Real-time statistics bar
4. **CartItemsList.razor** - Detailed cart items table

All components inject the same `ShoppingCartStore` and automatically re-render when any cart operation occurs.

## 🎯 What This Demonstrates

### **Reactive State Synchronization**
```csharp
// When you add an item in ANY component:
CartStore.AddToCart(product);

// ALL components automatically update:
// ✅ Product list shows updated quantities
// ✅ Cart summary shows new totals
// ✅ Status bar updates item count
// ✅ Items list reflects changes
```

### **Computed Properties**
The store uses computed signals that automatically recalculate:

```csharp
public Computed<int> ItemCount { get; }           // Total items
public Computed<decimal> TotalPrice { get; }      // Total cost
public Computed<string> FormattedTotal { get; }   // Formatted display
public Computed<bool> IsEmpty { get; }            // Empty state
```

### **Component Independence**
Each component only cares about its own UI logic:

```csharp
// ShoppingCartSummary.razor
protected override void OnInitialized()
{
    isEmpty = this.useSignal(CartStore.IsEmpty);
    itemCount = this.useSignal(CartStore.ItemCount);
    formattedTotal = this.useSignal(CartStore.FormattedTotal);
}
```

## 🚀 Try It Out

1. **Navigate to `/shopping`** in the demo
2. **Add products** by clicking the "Add" buttons
3. **Watch all components update** simultaneously:
   - Cart summary in the sidebar
   - Status bar at the bottom
   - Detailed items list
   - Product quantities in the grid

4. **Remove items** and see the reactive updates
5. **Clear the cart** and watch everything reset

## 💡 Key Benefits Demonstrated

### **No Manual State Synchronization**
- No need to pass callbacks between components
- No complex event handling
- No manual `StateHasChanged()` calls

### **Predictable State Updates**
- All state changes go through the store
- Computed properties automatically derive values
- Components stay in sync automatically

### **Scalable Architecture**
- Easy to add new components
- Components don't need to know about each other
- Store can be injected anywhere

### **Type Safety**
- Full IntelliSense support
- Compile-time validation
- Nullable reference type safety

## 🏗️ Architecture

```
ShoppingCartStore (Singleton)
├── Signals
│   ├── Products: Signal<List<Product>>
│   └── CartItems: Signal<List<CartItem>>
├── Computed Properties
│   ├── ItemCount: Computed<int>
│   ├── TotalPrice: Computed<decimal>
│   ├── FormattedTotal: Computed<string>
│   └── IsEmpty: Computed<bool>
└── Methods
    ├── AddToCart(product)
    ├── RemoveFromCart(productId)
    ├── ClearCart()
    └── GetProductQuantity(productId)
```

## 🎨 UI Features

- **Bootstrap 5** styling for modern look
- **Font Awesome** icons for better UX
- **Responsive design** works on all devices
- **Real-time progress bar** for free shipping threshold
- **Smooth animations** for state changes

This demo proves that BlazorSignalStore makes complex state management **simple, reactive, and maintainable**!