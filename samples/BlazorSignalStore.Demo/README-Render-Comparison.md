# Render Performance Comparison

This demonstration compares the rendering behavior between Microsoft's **Container State Pattern** and **BlazorSignalStore** with signals.

## What are we measuring?

Each component has a **render counter** that shows:

- 🔄 **Number of re-renders**: How many times the component has been re-rendered
- ⏰ **Timestamp**: When the last render occurred (HH:mm:ss.fff)

## How to test

1. **Reset counters**: Click "Reset Render Counters" to start fresh
2. **Type in a field**: Type a character in any "First Name" field
3. **Observe the counters**: Notice which components re-render

## Expected results

### Container State Pattern (Left - Yellow)

```text
User types "J" in First Name
├── 🔄 Personal Information: +1 render
├── 🔄 First Name input: +1 render
├── 🔄 Last Name input: +1 render (unnecessary!)
├── 🔄 Email input: +1 render (unnecessary!)
├── 🔄 Progress bar: +1 render
└── 🔄 Validation: +1 render
Total: 6 re-renders for 1 change
```

### BlazorSignalStore (Right - Green)

```text
User types "J" in First Name
├── 🔄 First Name input: +1 render (only this field)
├── 🔄 Progress bar: +1 render (automatic computed dependency)
└── 🔄 Validation: +1 render (automatic computed dependency)
Total: 3 re-renders for 1 change (67% less than Container State)
```

## Why this difference?

### Container State

- **Single state object**: Entire form is in `FormContainerState`
- **Global notification**: `NotifyStateChanged()` notifies ALL components
- **Massive re-render**: Even unrelated fields re-render

### BlazorSignalStore

- **Individual signals**: Each field is an independent signal
- **Granular notifications**: Only components using the changed signal re-render
- **Automatic dependencies**: Computed signals update automatically

## Performance Analysis

### Common test scenarios:

| Action | Container State | BlazorSignalStore | Improvement |
|--------|----------------|-------------------|------------|
| Type in First Name | 6 re-renders | 3 re-renders | **50% less** |
| Type in Email | 6 re-renders | 3 re-renders | **50% less** |
| Type in Last Name | 6 re-renders | 3 re-renders | **50% less** |
| Reset form | 6 re-renders | 1 re-render | **83% less** |
| Complete form (20 fields) | ~120 re-renders | ~40 re-renders | **67% less** |

### In real applications:

**Container State** problems:

- ❌ Unnecessary re-renders of unrelated components
- ❌ Performance degrades with large forms
- ❌ Requires manual `ShouldRender()` optimization
- ❌ More CPU work on each change

**BlazorSignalStore** benefits:

- ✅ Only re-renders what actually changed
- ✅ Scalable performance with large forms
- ✅ Automatic optimization, no extra code needed
- ✅ Lower CPU usage

## Code examples

### Container State

```csharp
// A change triggers global NotifyStateChanged()
public void UpdatePersonalInfo(string firstName, string lastName, string email)
{
    _formState = _formState with { /* ... */ };
    NotifyStateChanged(); // ⚠️ Re-renders EVERYTHING
}
```

### BlazorSignalStore

```csharp
// Only the specific signal notifies
public void UpdateFirstName(string firstName)
{
    FirstName.Value = firstName; // ✅ Only re-renders components using FirstName
}
```

## Conclusion

**BlazorSignalStore offers better performance** because:

1. **Granular reactivity**: Only updates what's necessary
2. **Automatic optimization**: No extra code required
3. **Scalability**: Performance doesn't degrade with large forms
4. **Better user experience**: Smoother and more responsive UI

The difference becomes more noticeable in:

- Large forms (10+ fields)
- Applications with many components
- Interfaces requiring frequent updates
- Resource-limited devices
