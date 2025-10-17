using BlazorSignalStore.Core;

namespace BlazorSignalStore.Demo.Store
{
    /// <summary>
    /// A simple counter store demonstrating the use of Signal and Computed.
    /// </summary>
    public class CounterStore : StoreBase
    {
        /// <summary>
        /// The current count signal.
        /// </summary>
        public Signal<int> Count { get; } = new(0);

        /// <summary>
        /// A computed signal that provides a label based on the current count.
        /// </summary>
        public Computed<string> Label { get; }

        /// <summary>
        /// Initializes a new instance of the CounterStore class.
        /// </summary>
        public CounterStore()
        {
            Label = new Computed<string>(() => $"Count: {Count.Value}", Count);
        }

        /// <summary>
        /// Increments the count.
        /// </summary>
        public void Increment() => Count.Value++;

        /// <summary>
        /// Decrements the count.
        /// </summary>
        public void Decrement() => Count.Value--;
    }
}