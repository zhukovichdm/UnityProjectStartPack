using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DropdownSelector : MonoBehaviour
{
    [Tooltip("Установить событие для каждого элемента Dropdown, назначить метод SelectValue на событие изменения значение Dropdown")] [SerializeField]
    private List<UnityEvent> events;

    [Header("Общие события")] [SerializeField]
    private UnityEvent commonEvents;

    private TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void InvokeAllEvents(int value)
    {
        commonEvents.Invoke();
        if (events.Count > value)
            events[value].Invoke();
    }

    public void InvokeCommonEvents()
    {
        commonEvents.Invoke();
    }

    public void DisableAndInvokeCommonEvents()
    {
        commonEvents.Invoke();
        gameObject.SetActive(false);
    }

    public void EnableAndSetFirstPosition()
    {
        gameObject.SetActive(true);
        dropdown.value = 0;
        if(events.Count>0) 
            events[0].Invoke();
    }
}