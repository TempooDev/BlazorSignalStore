# Container State Pattern Demo

This example demonstrates the **Container State** pattern recommended by Microsoft for efficient state management in Blazor applications.

## What is the Container State Pattern?

The Container State Pattern is a state management technique that groups multiple related state properties into a single container object, instead of managing them as individual properties.

## Problem it Solves

### Traditional Approach (Problematic):

```csharp
public class FormService
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    // ... more individual properties
    
    public event Action? OnChange;
    
    public void UpdateFirstName(string firstName)
    {
        FirstName = firstName;
        OnChange?.Invoke(); // Individual notification
    }
    
    public void UpdateLastName(string lastName)
    {
        LastName = lastName;
        OnChange?.Invoke(); // Individual notification
    }
    // ... more methods with individual notifications
}
```

**Problems:**

- Multiple change notifications
- Scattered state
- Fragmented validation
- Complex coordination logic
- Performance overhead

### Container State Approach (Solution):

```csharp
public class ContainerStateService
{
    private FormContainerState _formState = new();
    
    public event Action? OnChange;
    public FormContainerState FormState => _formState;
    
    public void UpdatePersonalInfo(string firstName, string lastName, string email)
    {
        _formState = _formState with
        {
            PersonalInfo = _formState.PersonalInfo with
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            }
        };
        OnChange?.Invoke(); // Single notification for entire update
    }
}

public record FormContainerState
{
    public PersonalInfo PersonalInfo { get; init; } = new();
    public Address Address { get; init; } = new();
    public UserPreferencesData Preferences { get; init; } = new();
    // ... logically grouped state
}
```

## Container State Benefits

1. **Fewer Notifications**: Single notification per state update
2. **Grouped State**: Related properties stay together
3. **Immutability**: Using records for immutable state
4. **Centralized Validation**: Validation logic within the container
5. **Better Performance**: Fewer UI updates
6. **Maintainability**: Cleaner and more organized code

## Example Features

### State Structure

- **PersonalInfo**: User's personal information
- **Address**: Address data
- **Preferences**: User configuration settings
- **UI States**: Loading indicators and results

### Demonstrated Functionality

- Multi-section form
- Real-time validation
- Progress bar
- Loading state management
- Submission results
- Form reset

### Update Pattern

```csharp
// Immutable update using records
_formState = _formState with
{
    PersonalInfo = _formState.PersonalInfo with
    {
        FirstName = newFirstName
    }
};
```

## Comparison with Signals

This example shows the standard Blazor approach **without** using Signals, following Microsoft's official recommendations. State management is handled through:

- Scoped services registered in DI
- `OnChange` events for notifications
- Immutable records for state
- With-expressions pattern for updates

## When to Use Container State

✅ **Use when:**

- You have multiple related state properties
- You need coordinated validation
- You want to reduce change notifications
- The state has a clear logical structure

❌ **Don't use when:**

- You only have 1-2 simple properties
- The state is not logically related
- You need very granular updates

## References

- [Microsoft Docs: Blazor State Management](https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management)
- [Container State Pattern](https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management?view=aspnetcore-9.0#container-state)
