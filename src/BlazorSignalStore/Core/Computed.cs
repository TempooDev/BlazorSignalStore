using System;

namespace BlazorSignalStore.Core
{
    /// <summary>
    /// Derived signal: automatically recalculates when its dependencies change.
    /// </summary>
    public class Computed<T> : Signal<T>, IDisposable
    {
        private readonly Func<T> _computeFn;
        private readonly IDisposable[] _subscriptions;

        /// <summary>
        /// Creates a computed signal that recalculates its value when any of its dependencies change.
        /// </summary>
        /// <param name="computeFn">The function used to compute the value.</param>
        /// <param name="dependencies">The dependencies that trigger recalculation.</param>
        public Computed(Func<T> computeFn, params object[] dependencies) : base(default!)
        {
            _computeFn = computeFn;

            // Subscribe to all dependencies
            var subscriptions = new List<IDisposable>();
            foreach (var dep in dependencies)
            {
                if (dep is Signal<int> intSignal)
                {
                    subscriptions.Add(intSignal.Subscribe(_ => OnDependencyChanged()));
                }
                else if (dep is Signal<string> stringSignal)
                {
                    subscriptions.Add(stringSignal.Subscribe(_ => OnDependencyChanged()));
                }
                else if (dep is Signal<double> doubleSignal)
                {
                    subscriptions.Add(doubleSignal.Subscribe(_ => OnDependencyChanged()));
                }
                else if (dep is Signal<bool> boolSignal)
                {
                    subscriptions.Add(boolSignal.Subscribe(_ => OnDependencyChanged()));
                }
                else if (dep is Signal<DateTime> dateTimeSignal)
                {
                    subscriptions.Add(dateTimeSignal.Subscribe(_ => OnDependencyChanged()));
                }
                // Add more specific types as needed, or use reflection for generic handling
                else
                {
                    // Try generic approach using reflection
                    var subscribeMethod = dep.GetType().GetMethod("Subscribe");
                    if (subscribeMethod != null)
                    {
                        // Create a generic Action delegate
                        var delegateType = subscribeMethod.GetParameters()[0].ParameterType;
                        var wrapperMethod = typeof(Computed<T>).GetMethod(nameof(CreateWrapper),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        if (wrapperMethod != null)
                        {
                            var genericTypes = delegateType.GetGenericArguments();
                            if (genericTypes.Length == 1)
                            {
                                var specificWrapperMethod = wrapperMethod.MakeGenericMethod(genericTypes[0]);
                                var wrapper = specificWrapperMethod.Invoke(this, null);
                                var subscription = (IDisposable)subscribeMethod.Invoke(dep, new[] { wrapper })!;
                                subscriptions.Add(subscription);
                            }
                        }
                    }
                }
            }

            _subscriptions = subscriptions.ToArray();

            // Calculate initial value
            Value = _computeFn();
        }

        /// <summary>
        /// Creates a wrapper action for a specific type.
        /// </summary>
        private Action<TValue> CreateWrapper<TValue>()
        {
            return _ => OnDependencyChanged();
        }

        /// <summary>
        /// Handles dependency change events by recalculating the value.
        /// </summary>
        private void OnDependencyChanged()
        {
            Value = _computeFn();
        }

        /// <summary>
        /// Disposes all subscriptions when the computed signal is disposed.
        /// </summary>
        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription?.Dispose();
            }
        }
    }
}
