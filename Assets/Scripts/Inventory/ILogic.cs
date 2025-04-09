namespace Inventory
{
    /// <summary>
    /// Interface for components that can be activated/deactivated
    /// </summary>
    public interface ILogic
    {
        /// <summary>
        /// Sets the active state of the component
        /// </summary>
        /// <param name="active">True to activate, false to deactivate</param>
        public void SetActive(bool active);
    }
}
