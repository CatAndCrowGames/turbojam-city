using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    Entity timeEntity;
    Entity boundaryEntity;
    EntityQuery timeQuery;
    EntityQuery boundaryQuery;
    EntityManager em;
    string powerFormat;
    string timeFormat;

    Label label;

    void Awake()
    {
        label = uiDocument.rootVisualElement.Query<Label>();
        timeFormat = "00";
    }

    IEnumerator Start()
    {
        while (!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        timeQuery = em.CreateEntityQuery(new ComponentType[] { typeof(TimeData) });
        while (timeQuery.IsEmpty) yield return null;
        timeEntity = timeQuery.GetSingletonEntity();
        boundaryQuery = em.CreateEntityQuery(new ComponentType[] { typeof(BoundaryData) });
        while (boundaryQuery.IsEmpty) yield return null;
        boundaryEntity = boundaryQuery.GetSingletonEntity();
    }

    void LateUpdate()
    {
        if (timeEntity == Entity.Null) return;
        if (boundaryEntity == Entity.Null) return;
        TimeData timeData = em.GetComponentData<TimeData>(timeEntity);
        BoundaryData boundaryData = em.GetComponentData<BoundaryData>(boundaryEntity);
        label.text = "Days: " + timeData.CurrentDay.ToString(timeFormat);
        label.text+= " // Hours: " + timeData.CurrentHour.ToString(timeFormat);
    }
}
