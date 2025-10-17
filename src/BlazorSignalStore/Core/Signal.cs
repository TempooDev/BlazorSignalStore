using System;
using System.Collections.Generic;

namespace BlazorSignalStore.Core
{
    /// <summary>
    /// Represents an observable signal that notifies subscribers when its value changes.
    /// </summary>
    /// <typeparam name="T">The type of the value held by the signal.</typeparam>
    public class Signal<T>
    {
        private T _value;

        /// <summary>
        /// Event that is raised when the signal's value changes.
        /// </summary>
        public event Action<T>? Changed;

        /// <summary>
        /// Gets or sets the current value of the signal.
        /// Setting a new value will notify all subscribers if the value has changed.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    Changed?.Invoke(_value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the Signal class with the specified initial value.
        /// </summary>
        /// <param name="initialValue">The initial value of the signal.</param>
        public Signal(T initialValue)
        {
            _value = initialValue;
        }

        /// <summary>
        /// Invoke the signal to get its current value.
        /// </summary>
        public T Invoke() => Value;

        /// <summary>
        /// Implicit cast to use the signal as a value directly.
        /// </summary>
        public static implicit operator T(Signal<T> signal) => signal.Value;

        /// <summary>
        /// Allows subscribing and automatically executing an action when it changes.
        /// </summary>
        public IDisposable Subscribe(Action<T> listener)
        {
            Changed += listener;
            listener(_value); // emit the initial value
            return new Unsubscriber(() => Changed -= listener);
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly Action _unsubscribe;
            public Unsubscriber(Action unsubscribe) => _unsubscribe = unsubscribe;
            public void Dispose() => _unsubscribe();
        }
    }
}
