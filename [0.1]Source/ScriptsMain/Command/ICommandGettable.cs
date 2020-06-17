using System.Collections;

namespace Scripts.Components.Command
{
    public interface ICommandGettable<in T> : ISettable
    {
        IEnumerator Execute(T value);
    }
}