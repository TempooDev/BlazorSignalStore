using Microsoft.AspNetCore.Components;
using System;

namespace BlazorSignalStore.Core
{
    /// <summary>
    /// Provides the useSignal() hook, which automatically subscribes the component
    /// to changes in the signal and re-renders when they change.
    /// </summary>
    public static class SignalHooks
    {
        /// <summary>
        /// Hook to use a signal in a Blazor component.
        /// Automatically subscribes to changes and triggers re-render.
        /// </summary>
        /// <typeparam name="T">The type of the signal.</typeparam>
        /// <param name="component">The Blazor component.</param>
        /// <param name="signal">The signal to use.</param>
        /// <returns>A SignalValue wrapper that provides access to the signal's value.</returns>
        public static SignalValue<T> useSignal<T>(this ComponentBase component, Signal<T> signal)
        {
            return new SignalValue<T>(signal, component);
        }

        /// <summary>
        /// Hook to use a computed signal in a Blazor component.
        /// Automatically subscribes to changes and triggers re-render.
        /// </summary>
        /// <typeparam name="T">The type of the computed signal.</typeparam>
        /// <param name="component">The Blazor component.</param>
        /// <param name="computed">The computed signal to use.</param>
        /// <returns>A SignalValue wrapper that provides access to the computed signal's value.</returns>
        public static SignalValue<T> useSignal<T>(this ComponentBase component, Computed<T> computed)
        {
            return new SignalValue<T>(computed, component);
        }

        /// <summary>
        /// Hook to use a signal in a Blazor component, returning a function.
        /// This overload provides a more React-like API where you can call count() instead of count.Value.
        /// </summary>
        /// <typeparam name="T">The type of the signal.</typeparam>
        /// <param name="component">The Blazor component.</param>
        /// <param name="signal">The signal to use.</param>
        /// <param name="useAsFunction">Set to true to return a Func instead of SignalValue.</param>
        /// <returns>A function that returns the current value of the signal.</returns>
        public static Func<T> useSignal<T>(this ComponentBase component, Signal<T> signal, bool useAsFunction)
        {
            if (useAsFunction)
            {
                var signalValue = new SignalValue<T>(signal, component);
                return signalValue.Invoke;
            }
            return component.useSignal(signal);
        }

        /// <summary>
        /// Hook to use a computed signal in a Blazor component, returning a function.
        /// This overload provides a more React-like API where you can call label() instead of label.Value.
        /// </summary>
        /// <typeparam name="T">The type of the computed signal.</typeparam>
        /// <param name="component">The Blazor component.</param>
        /// <param name="computed">The computed signal to use.</param>
        /// <param name="useAsFunction">Set to true to return a Func instead of SignalValue.</param>
        /// <returns>A function that returns the current value of the computed signal.</returns>
        public static Func<T> useSignal<T>(this ComponentBase component, Computed<T> computed, bool useAsFunction)
        {
            if (useAsFunction)
            {
                var signalValue = new SignalValue<T>(computed, component);
                return signalValue.Invoke;
            }
            return component.useSignal(computed);
        }
    }

    /// <summary>
    /// Wrapper class that provides access to signal values and automatically handles component re-rendering.
    /// </summary>
    /// <typeparam name="T">The type of the signal value.</typeparam>
    public class SignalValue<T> : IDisposable
    {
        private readonly Signal<T> _signal; // Both Signal<T> and Computed<T> inherit from this
        private readonly ComponentBase _component;
        private readonly IDisposable _subscription;

        internal SignalValue(Signal<T> signal, ComponentBase component)
        {
            _signal = signal;
            _component = component;

            // Subscribe to signal changes and trigger re-render
            _subscription = signal.Subscribe(_ =>
            {
                TriggerRerender();
            });
        }

        internal SignalValue(Computed<T> computed, ComponentBase component) : this((Signal<T>)computed, component)
        {
            // Computed<T> inherits from Signal<T>, so we can cast and use the base constructor
        }

        private void TriggerRerender()
        {
            // Use reflection to call the protected InvokeAsync method safely
            try
            {
                // First try: InvokeAsync with Action parameter
                var invokeAsyncMethod = typeof(ComponentBase).GetMethod("InvokeAsync",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new[] { typeof(Action) },
                    null);

                if (invokeAsyncMethod != null)
                {
                    var stateHasChangedMethod = typeof(ComponentBase).GetMethod("StateHasChanged",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (stateHasChangedMethod != null)
                    {
                        Action stateHasChangedAction = () => stateHasChangedMethod.Invoke(_component, null);
                        invokeAsyncMethod.Invoke(_component, new object[] { stateHasChangedAction });
                        return; // Success, exit early
                    }
                }
            }
            catch
            {
                // Continue to fallback
            }

            // Fallback 1: Try direct StateHasChanged call with InvokeAsync
            try
            {
                var invokeAsyncMethod = typeof(ComponentBase).GetMethod("InvokeAsync",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new[] { typeof(Func<System.Threading.Tasks.Task>) },
                    null);

                if (invokeAsyncMethod != null)
                {
                    var stateHasChangedMethod = typeof(ComponentBase).GetMethod("StateHasChanged",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (stateHasChangedMethod != null)
                    {
                        Func<System.Threading.Tasks.Task> taskFunc = () =>
                        {
                            stateHasChangedMethod.Invoke(_component, null);
                            return System.Threading.Tasks.Task.CompletedTask;
                        };
                        invokeAsyncMethod.Invoke(_component, new object[] { taskFunc });
                        return; // Success, exit early
                    }
                }
            }
            catch
            {
                // Continue to final fallback
            }

            // Fallback 2: Direct StateHasChanged call (last resort)
            try
            {
                var stateHasChangedMethod = typeof(ComponentBase).GetMethod("StateHasChanged",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                stateHasChangedMethod?.Invoke(_component, null);
            }
            catch
            {
                // If all else fails, we can't trigger re-render
                // This should be extremely rare
            }
        }

        /// <summary>
        /// Gets the current value of the signal.
        /// </summary>
        public T Value => _signal.Value;

        /// <summary>
        /// Allows calling the signal value as a function to get the current value.
        /// </summary>
        /// <returns>The current value of the signal.</returns>
        public T Invoke() => _signal.Value;

        /// <summary>
        /// Implicit conversion to the signal's value type.
        /// </summary>
        /// <param name="signalValue">The signal value wrapper.</param>
        public static implicit operator T(SignalValue<T> signalValue) => signalValue.Value;

        /// <summary>
        /// Implicit conversion to a function that returns the signal's value.
        /// This allows using the signal like count() instead of count.Value.
        /// </summary>
        /// <param name="signalValue">The signal value wrapper.</param>
        public static implicit operator Func<T>(SignalValue<T> signalValue) => signalValue.Invoke;

        /// <summary>
        /// Disposes the subscription to the signal.
        /// </summary>
        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
