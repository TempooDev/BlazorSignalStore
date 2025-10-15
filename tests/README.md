# BlazorSignalStore Tests

This project contains a comprehensive test suite for the BlazorSignalStore library.

## Test Structure

```
tests/
├── BlazorSignalStore.Tests/
│   ├── Core/
│   │   ├── SignalTests.cs           # Tests for Signal<T> class
│   │   ├── ComputedTests.cs         # Tests for Computed<T> class
│   │   └── SignalHooksTests.cs      # Tests for Blazor hooks
│   ├── Integration/
│   │   └── CounterStoreIntegrationTests.cs  # Integration tests
│   └── ServiceCollectionExtensionsTests.cs  # DI tests
```

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run specific tests
```bash
# Signal tests only
dotnet test --filter "FullyQualifiedName~SignalTests"

# ServiceCollection tests only
dotnet test --filter "FullyQualifiedName~ServiceCollectionExtensionsTests"

# Working tests only (excluding Blazor tests with issues)
dotnet test --filter "FullyQualifiedName~SignalTests|FullyQualifiedName~ServiceCollectionExtensionsTests"
```

### Run with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Implemented Tests

### ✅ SignalTests (11 tests - All passing)
- Initialization with value
- Change notifications
- Duplicate notification prevention
- Implicit conversion
- Invoke() method
- Subscription and unsubscription
- Nullable value handling
- Complex types

### ⚠️ ComputedTests (Work in progress)
- Initial value calculation
- Recalculation on dependency changes
- Multiple dependencies
- Nested signals
- Signal inheritance

### ⚠️ SignalHooksTests (Work in progress)
- Blazor hooks
- Automatic re-rendering
- Function conversion
- Subscription disposal

### ✅ ServiceCollectionExtensionsTests (5 tests - All passing)
- Scoped registration
- Multiple stores
- StoreBase inheritance
- Different scopes

### ⚠️ Integration Tests (Work in progress)
- End-to-end tests with CounterStore
- Real Blazor components
- Full interaction testing

## Testing Technologies

- **xUnit**: Main testing framework
- **FluentAssertions**: More readable assertions
- **bUnit**: Blazor component testing
- **Moq**: Mocking framework
- **Coverlet**: Code coverage

## Current Status

**Working Tests**: 16/38 (42%)
- ✅ Signal: 11/11 tests
- ✅ ServiceCollection: 5/5 tests
- ⚠️ Computed: 0/10 tests (work in progress)
- ⚠️ SignalHooks: 0/8 tests (work in progress)
- ⚠️ Integration: 0/5 tests (work in progress)

## Next Steps

1. **Fix Computed class**: Improve dependency subscription system
2. **Blazor threading**: Solve threading issues in component tests
3. **Blazor mocks**: Create appropriate mocks for ComponentBase
4. **Full coverage**: Achieve 100% coverage on core components

## Running Development Tests

To run only currently working tests:

```bash
dotnet test --filter "FullyQualifiedName~SignalTests|FullyQualifiedName~ServiceCollectionExtensionsTests"
```

To see specific tests in detail:

```bash
dotnet test --logger "console;verbosity=detailed"
```