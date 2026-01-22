using System.Collections;
using System.Collections.Immutable;

namespace QaaS.Common.Generators.Tests.ConfigurationObjects;

/// <summary>
/// A lightweight implementation of IImmutableList that delays enumeration 
/// until elements are actually accessed.
/// </summary>
/// <remarks>
/// This class is designed to optimize performance when working with potentially large 
/// collections where not all elements will be accessed. Instead of immediately enumerating 
/// the entire source collection, it defers enumeration until:
/// <list type="bullet">
///   <item><description>Count or indexer access</description></item>
///   <item><description>Any operation requiring full enumeration (e.g., Contains, IndexOf)</description></item>
/// </list>
/// 
/// Once enumeration occurs (either explicitly via Count/indexer access or implicitly through 
/// operations requiring the full list), the results are cached in memory for subsequent accesses.
/// This provides a balance between lazy evaluation and avoiding repeated expensive enumerations.
/// 
/// Note: This implementation only supports enumeration-based operations and throws 
/// NotImplementedException for any mutation operations.
/// </remarks>
public class LazyList<T>(IEnumerable<T> inner) : IImmutableList<T>
{
    private IList<T>? _cache;

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// This operation does NOT trigger enumeration of the underlying data.
    /// Enumeration only occurs when IEnumerator.MoveNext is called.
    /// </summary>
    public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

    /// <summary>
    /// Gets the number of elements in the list.
    /// This triggers enumeration of the underlying collection and caches the result.
    /// Subsequent calls return the cached count without re-enumerating.
    /// </summary>
    public int Count => Cache.Count;

    /// <summary>
    /// Gets the element at the specified index.
    /// This triggers enumeration of the underlying collection and caches the result.
    /// Subsequent accesses to the same or other indices return cached values.
    /// </summary>
    public T this[int index] => Cache[index];

    /// <summary>
    /// Gets the cached version of the underlying collection.
    /// If not already cached, it enumerates the source collection and stores the result.
    /// </summary>
    private IList<T> Cache => _cache ??= inner.ToList();

    // All mutation methods are intentionally unimplemented as this is meant to be immutable.
    // These would typically create new instances rather than modifying the existing one.
    public IImmutableList<T> Add(T item) => throw new NotImplementedException();
    public IImmutableList<T> AddRange(IEnumerable<T> items) => throw new NotImplementedException();

    public IImmutableList<T> Clear() => throw new NotImplementedException();

    public int IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) =>
        throw new NotImplementedException();

    public IImmutableList<T> RemoveAll(Predicate<T> match) => throw new NotImplementedException();

    public IImmutableList<T> RemoveAt(int index) => throw new NotImplementedException();

    public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer) =>
        throw new NotImplementedException();

    public IImmutableList<T> RemoveRange(int index, int count) => throw new NotImplementedException();

    public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer) =>
        throw new NotImplementedException();

    public IImmutableList<T> SetItem(int index, T value) => throw new NotImplementedException();

    public IImmutableList<T> Insert(int index, T item) => throw new NotImplementedException();
    public IImmutableList<T> InsertRange(int index, IEnumerable<T> items) => throw new NotImplementedException();

    public int LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) =>
        throw new NotImplementedException();

    public IImmutableList<T> Remove(T value, IEqualityComparer<T>? equalityComparer) =>
        throw new NotImplementedException();
}