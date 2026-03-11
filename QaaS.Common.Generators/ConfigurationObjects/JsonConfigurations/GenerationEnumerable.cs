using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// Represents a reusable sequence of generated records that can be iterated with an out-of-range policy.
/// </summary>
public record GenerationEnumerable
{
    /// <summary>
    /// The generated records available for iteration.
    /// </summary>
    private IEnumerable<Data<object>> Results { get; }
    
    /// <summary>
    /// The current index in the collection.
    /// </summary>
    private int Index { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationEnumerable"/> class.
    /// </summary>
    /// <param name="results">The collection of data records.</param>
    public GenerationEnumerable(IEnumerable<Data<object>> results)
    {
        Results = results;
        Index = 0;
    }

    /// <summary>
    /// Retrieves the next record in the sequence and applies the configured out-of-range behavior when the end is reached.
    /// </summary>
    /// <param name="generationOutOfRangeLoopPolicy">Determines whether iteration loops back to the beginning or returns a record with a null body.</param>
    /// <returns>The next available <see cref="Data{T}"/> entry.</returns>
    public Data<object> GetNextRecord(OutOfRangePolicy generationOutOfRangeLoopPolicy)
    {
        if (Index == Results.Count())
        {
            if (generationOutOfRangeLoopPolicy == OutOfRangePolicy.Null) return new Data<object> { Body = null };
            Index = 0;
        }
        
        var record = Results.ElementAt(Index);
        Index++;
        return record;
    }
}
