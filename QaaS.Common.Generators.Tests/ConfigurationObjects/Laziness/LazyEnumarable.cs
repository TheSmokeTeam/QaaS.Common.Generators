using System.Collections;

namespace QaaS.Common.Generators.Tests.ConfigurationObjects.Laziness;

/// <summary>
/// Wrapper around enumerable that adds side effect tracking
/// </summary>
public class LazyEnumerable<T>(
    Func<IEnumerable<T>> generatorFunc,
    SideEffectTracker tracker)
    : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        // Track when we start enumeration
        tracker.IncrementSideEffect();

        var enumerable = generatorFunc();
        return new LazyEnumerator<T>(enumerable.GetEnumerator(), tracker);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}