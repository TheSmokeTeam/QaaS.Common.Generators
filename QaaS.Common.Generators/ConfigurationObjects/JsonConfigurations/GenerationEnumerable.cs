using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// This class represents an enumerable collection of data records.
/// It provides functionality to retrieve the next record in the collection.
/// </summary>
public record GenerationEnumerable
{
    /// <summary>
    /// The collection of data records.
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
    /// Retrieves the next record in the collection. If the current index is at the end of the collection and the provided policy is false,
    /// a new <c>Data&lt;object&gt;</c> with a null body is returned. Otherwise, the index is reset to 0 and the first record in the collection is returned.
    /// </summary>
    /// <param name="generationOutOfRangeLoopPolicy">The policy to handle the end of the collection.</param>
    /// <returns>The next record in the collection.</returns>
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
