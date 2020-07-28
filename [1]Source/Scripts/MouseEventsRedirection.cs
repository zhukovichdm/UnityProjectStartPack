using System.Collections.Generic;
using UnityEngine;

public interface IMouseUpAsButton : IMouseEvent
{
    void OnMouseUpAsButton_Redirected(GameObject target);
}

public interface IMouseUp : IMouseEvent
{
    void OnMouseUp_Redirected(GameObject target);
}

public interface IMouseDown : IMouseEvent
{
    void OnMouseDown_Redirected(GameObject target);
}

public interface IMouseEnter : IMouseEvent
{
    void OnMouseEnter_Redirected(GameObject target);
}

public interface IMouseExit : IMouseEvent
{
    void OnMouseExit_Redirected(GameObject target);
}

public interface IMouseOver : IMouseEvent
{
    void OnMouseOver_Redirected(GameObject target);
}

public interface IMouseDrag : IMouseEvent
{
    void OnMouseDrag_Redirected(GameObject target);
}

public interface IMouseEvent
{
}

[RequireComponent(typeof(Collider))]
public class MouseEventsRedirection : MonoBehaviour
{
    private readonly List<IMouseEvent> _mouseEvents = new List<IMouseEvent>();

    public static void RedirectFromChildObjects(IMouseEvent target)
    {
        foreach (var child in ((MonoBehaviour) target).GetComponentsInChildren<Collider>())
        {
            if (child.TryGetComponent<MouseEventsRedirection>(out var redirection))
                redirection.Add(target);
            else
                child.gameObject.AddComponent<MouseEventsRedirection>().Add(target);
        }
    }

    public static void StopRedirectFromChildObjects(IMouseEvent target)
    {
        foreach (var child in ((MonoBehaviour) target).GetComponentsInChildren<Collider>())
        {
            if (child.TryGetComponent<MouseEventsRedirection>(out var redirection))
                redirection.Remove(target);
        }
    }

    public void Add(IMouseEvent mouseEvent)
    {
        if (_mouseEvents.Contains(mouseEvent) == false)
            _mouseEvents.Add(mouseEvent);
    }

    public void Remove(IMouseEvent mouseEvent)
    {
        if (_mouseEvents.Contains(mouseEvent))
            _mouseEvents.Remove(mouseEvent);
    }

    private void OnMouseUpAsButton()
    {
        foreach (var mouseEvent in _mouseEvents)
            if (mouseEvent is IMouseUpAsButton iEvent)
                iEvent.OnMouseUpAsButton_Redirected(gameObject);
    }

    public void OnMouseUp()
    {
        foreach (var mouseEvent in _mouseEvents)
            if (mouseEvent is IMouseUp iEvent)
                iEvent.OnMouseUp_Redirected(gameObject);
    }

    public void OnMouseDown()
    {
        foreach (var mouseEvent in _mouseEvents)
            if (mouseEvent is IMouseDown iEvent)
                iEvent.OnMouseDown_Redirected(gameObject);
    }

    private void OnMouseEnter()
    {
        foreach (var mouseEvent in _mouseEvents)
            if (mouseEvent is IMouseEnter iEvent)
                iEvent.OnMouseEnter_Redirected(gameObject);
    }

    private void OnMouseExit()
    {
        foreach (var mouseEvent in _mouseEvents)
            if (mouseEvent is IMouseExit iEvent)
                iEvent.OnMouseExit_Redirected(gameObject);
    }

    private void OnMouseOver()
    {
        foreach (var mouseEvent in _mouseEvents)
            if (mouseEvent is IMouseOver iEvent)
                iEvent.OnMouseOver_Redirected(gameObject);
    }

    private void OnMouseDrag()
    {
        foreach (var mouseEvent in _mouseEvents)
            if (mouseEvent is IMouseDrag iEvent)
                iEvent.OnMouseDrag_Redirected(gameObject);
    }
}