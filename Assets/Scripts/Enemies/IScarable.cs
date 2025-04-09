using Building.Concrete.Scarecrow;

namespace Enemies
{
    /// <summary>
    /// Interface for entities that can be frightened by scary objects
    /// Implementations should manage fear levels from multiple sources
    /// </summary>
    public interface IScarable
    {
        /// <summary>
        /// Updates the fear level from a specific source
        /// </summary>
        /// <param name="source">The object causing fear</param>
        /// <param name="amount">The intensity of fear (higher values = more fear)</param>
        void UpdateScareFromSource(IScary source, int amount);
        
        /// <summary>
        /// Removes a fear source when it's no longer affecting this entity
        /// </summary>
        /// <param name="source">The fear source to remove</param>
        void RemoveScareFromSource(IScary source);
    }
}