namespace FourierWatermark.Services;

/// <summary>
/// Simplified operations.
/// </summary>
internal static class Lazy
{
    /// <summary>
    /// Try to execute an action and return true if success, false if exception occurred.
    /// </summary>
    internal static bool Try(string message, Action action)
    {
        try { action(); return true; }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to {message}!");
            Console.WriteLine($"Exception: {ex}");
            return false;
        }
    }
}
