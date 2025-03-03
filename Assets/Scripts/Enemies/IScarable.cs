using System.Collections.Generic;

namespace Enemies
{
    public interface IScarable
    {
        void UpdateScareFromSource(IScary source, int amount);
        void RemoveScareFromSource(IScary source);
    }
}