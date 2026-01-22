using System.Collections;

namespace QaaS.Common.Generators.Tests.ConfigurationObjects.Laziness;

/// <summary>
/// Wrapper around enumerator that adds side effect tracking
/// </summary>
public class LazyEnumerator<T>(IEnumerator<T> innerEnumerator, SideEffectTracker tracker) : IEnumerator<T>
{
    public T Current => innerEnumerator.Current;

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        // Track when we move to next element
        tracker.IncrementSideEffect();
        return innerEnumerator.MoveNext();
    }

    public void Reset()
    {
        innerEnumerator.Reset();
    }

    public void Dispose()
    {
        innerEnumerator.Dispose();
    }
}