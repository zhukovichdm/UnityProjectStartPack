using UnityEngine;
using UnityEngine.Events;

public class EventForAnimator : MonoBehaviour
{
    [SerializeField] private UnityEvent startAction;
    [SerializeField] private UnityEvent endAction;

    public void InvokeStart()
    {
        startAction.Invoke();
    }

    public void InvokeEnd()
    {
        endAction.Invoke();
    }
}
