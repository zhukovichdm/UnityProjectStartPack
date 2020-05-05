using System.Collections;

namespace Scripts.Components.Command
{
    public interface ICommand : ISettable
    {
        IEnumerator Execute();
    }
}