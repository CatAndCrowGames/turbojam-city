using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemDebugger : MonoBehaviour
{
    [Read][SerializeField] bool isCurrent;
    [Read][SerializeField] bool hoveringOverSomething;
    [Read][SerializeField] int eventSystemsCount;
    [Read][SerializeField] GameObject selectedObject;
    [Read][SerializeField] GameObject hoveredObject;

    EventSystem eSystem;

    void Awake()
    {
        eSystem = GetComponent<EventSystem>();
        eventSystemsCount = FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
    }

    void Update()
    {
        isCurrent = EventSystem.current == eSystem;
        selectedObject = eSystem.currentSelectedGameObject;
        hoveringOverSomething = eSystem.IsPointerOverGameObject();
    }
}
