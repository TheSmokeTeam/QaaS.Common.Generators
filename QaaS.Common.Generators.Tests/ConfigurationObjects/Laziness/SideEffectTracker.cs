namespace QaaS.Common.Generators.Tests.ConfigurationObjects.Laziness;

/// <summary>
/// Tracks side effects during enumeration
/// </summary>
public class SideEffectTracker
{
    private int SideEffectCount { get; set; }

    public void IncrementSideEffect()
    {
        SideEffectCount++;
    }
}