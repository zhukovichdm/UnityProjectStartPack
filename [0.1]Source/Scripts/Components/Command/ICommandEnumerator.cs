using System.Collections;

namespace Scripts.Components.Command
{
    public interface ICommandEnumerator
    {
        bool InProgress { get; set; }
        IEnumerator Execute();
    }
}