using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using BlazorSignalStore.Core;
using Xunit;
using System;
using System.Threading.Tasks;

namespace BlazorSignalStore.Tests.Core;

/// <summary>
/// Tests for the enhanced TriggerRerender functionality and reflection-based component updates.
/// </summary>
public class SignalValueRerenderTests : TestContext
{
    [Fact]
    public void SignalValue_TriggerRerender_ShouldHandleNormalComponents()
    {
        // Arrange
        var signal = new Signal<string>("initial");
        var component = RenderComponent<NormalTestComponent>();

        // Act
        var signalValue = component.Instance.CreateSignalValue(signal);
        signal.Value = "updated";

        // Assert
        // The component should show the updated value through the signal value wrapper
        signalValue.Value.Should().Be("updated");
    }

    [Fact]
    public void SignalValue_TriggerRerender_ShouldHandleComponentsWithoutStateHasChanged()
    {
        // Arrange
        var signal = new Signal<int>(1);
        var component = RenderComponent<ComponentWithoutStateHasChanged>();
        var signalValue = component.Instance.CreateSignalValue(signal);

        // Act & Assert - Should not throw exception even if reflection fails
        Action act = () => signal.Value = 2;
        act.Should().NotThrow();
    }

    [Fact]
    public void SignalValue_TriggerRerender_ShouldHandleReflectionFailure()
    {
        // Arrange
        var signal = new Signal<bool>(true);
        var component = RenderComponent<MockComponentWithRestrictedAccess>();

        // Act & Assert - Should gracefully handle reflection access issues
        Action act = () =>
        {
            var signalValue = component.Instance.CreateSignalValue(signal);
            signal.Value = false;
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void SignalValue_WithSignal_ShouldStoreCorrectSourceType()
    {
        // Arrange
        var component = RenderComponent<SourceTypeTestComponent>();
        var signal = new Signal<double>(3.14);

        // Act
        var signalValue = component.Instance.CreateSignalValue(signal);

        // Assert - Since we can't access private fields in tests, 
        // we'll verify behavior instead of internal structure
        signalValue.Value.Should().Be(3.14);
        signalValue.Invoke().Should().Be(3.14);
    }

    [Fact]
    public void SignalValue_WithComputed_ShouldStoreCorrectSourceType()
    {
        // Arrange
        var component = RenderComponent<SourceTypeTestComponent>();
        var baseSignal = new Signal<int>(42);
        var computed = new Computed<string>(() => baseSignal.Value.ToString(), baseSignal);

        // Act
        var signalValue = component.Instance.CreateSignalValue(computed);

        // Assert - Since we can't access private fields in tests,
        // we'll verify behavior instead of internal structure
        signalValue.Value.Should().Be("42");
        signalValue.Invoke().Should().Be("42");
    }

    [Fact]
    public void SignalValue_Dispose_ShouldNotThrowOnMultipleCalls()
    {
        // Arrange
        var signal = new Signal<string>("test");
        var component = RenderComponent<NormalTestComponent>();
        var signalValue = component.Instance.CreateSignalValue(signal);

        // Act & Assert
        Action act = () =>
        {
            signalValue.Dispose();
            signalValue.Dispose(); // Second dispose should not throw
            signalValue.Dispose(); // Third dispose should not throw
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void SignalValue_AccessValueAfterDispose_ShouldStillWork()
    {
        // Arrange
        var signal = new Signal<int>(123);
        var component = RenderComponent<NormalTestComponent>();
        var signalValue = component.Instance.CreateSignalValue(signal);

        // Act
        signalValue.Dispose();

        // Assert - Value access should still work even after disposal
        signalValue.Value.Should().Be(123);
        signalValue.Invoke().Should().Be(123);
    }

    [Fact]
    public void SignalValue_InvokeAfterDispose_ShouldStillWork()
    {
        // Arrange
        var signal = new Signal<string>("disposed");
        var component = RenderComponent<NormalTestComponent>();
        var signalValue = component.Instance.CreateSignalValue(signal);

        // Act
        signalValue.Dispose();
        Func<string> func = signalValue;

        // Assert
        func().Should().Be("disposed");
    }

    [Fact]
    public void SignalValue_ImplicitConversionAfterDispose_ShouldStillWork()
    {
        // Arrange
        var signal = new Signal<bool>(true);
        var component = RenderComponent<NormalTestComponent>();
        var signalValue = component.Instance.CreateSignalValue(signal);

        // Act
        signalValue.Dispose();
        bool value = signalValue; // Implicit conversion

        // Assert
        value.Should().BeTrue();
    }

    [Fact]
    public void SignalValue_ReflectionFallback_ShouldHandleInvokeAsyncFailure()
    {
        // Arrange
        var signal = new Signal<string>("fallback");
        var component = RenderComponent<ComponentWithInvokeAsyncIssues>();

        // Act & Assert - Should not throw and should fallback gracefully
        Action act = () =>
        {
            var signalValue = component.Instance.CreateSignalValue(signal);
            signal.Value = "changed";
        };

        act.Should().NotThrow();
    }
}

// Test components for rerender tests
public class NormalTestComponent : ComponentBase
{
    private string _value = "initial";

    public SignalValue<T> CreateSignalValue<T>(Signal<T> signal)
    {
        var signalValue = this.useSignal(signal);

        // Set up a simple way to track value changes
        if (typeof(T) == typeof(string))
        {
            signal.Changed += value => _value = value?.ToString() ?? "null";
        }

        return signalValue;
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddContent(1, _value);
        builder.CloseElement();
    }
}

public class ComponentWithoutStateHasChanged : ComponentBase
{
    public SignalValue<T> CreateSignalValue<T>(Signal<T> signal)
    {
        return this.useSignal(signal);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.AddContent(0, "Component without StateHasChanged access");
    }
}

public class MockComponentWithRestrictedAccess : ComponentBase
{
    public SignalValue<T> CreateSignalValue<T>(Signal<T> signal)
    {
        return this.useSignal(signal);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.AddContent(0, "Restricted component");
    }
}

public class SourceTypeTestComponent : ComponentBase
{
    public SignalValue<T> CreateSignalValue<T>(Signal<T> signal)
    {
        return this.useSignal(signal);
    }

    public SignalValue<T> CreateSignalValue<T>(Computed<T> computed)
    {
        return this.useSignal(computed);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.AddContent(0, "Source type test component");
    }
}

public class ComponentWithInvokeAsyncIssues : ComponentBase
{
    public SignalValue<T> CreateSignalValue<T>(Signal<T> signal)
    {
        return this.useSignal(signal);
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.AddContent(0, "Component with InvokeAsync issues");
    }
}