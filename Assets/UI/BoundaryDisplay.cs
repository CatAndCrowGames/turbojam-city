using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class BoundaryDisplay : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    Entity boundaryEntity;
    Entity gameStateEntity;
    
    VisualElement bar;
    VisualElement workerBar;
    VisualElement root;
    Label label;
    EntityQuery boundaryQuery;
    EntityQuery gameStateQuery;
    EntityManager em;

    int currentRebels;

    void Awake()
    {
        root = uiDocument.rootVisualElement;
        bar = root.Query<VisualElement>("Bar");
        label = root.Query<Label>();
        workerBar = root.Query<VisualElement>("Bar");
    }

    IEnumerator Start()
    {
        while(!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        boundaryQuery = em.CreateEntityQuery(new ComponentType[] { typeof(BoundaryData) });
        gameStateQuery = em.CreateEntityQuery(new ComponentType[] { typeof(GameStateData) });
        yield return null;
    }

    void Update()
    {
        if (boundaryQuery.IsEmpty) return;
        BoundaryData boundaryData = boundaryQuery.GetSingleton<BoundaryData>();
        float power =boundaryData.Power;
        float maxPower = boundaryData.MaxPower;
        if (bar == null || label == null) return;
        bar.style.width = Length.Percent(power * 100 / maxPower);
        label.text = "Boundary Strength: " + power.ToString("F0") + " / " + maxPower.ToString("F0");
        GameStateData stateData = gameStateQuery.GetSingleton<GameStateData>();
        if (stateData.RebelCount > currentRebels) SFXPlayer.PlayRebelSound();
        currentRebels = stateData.RebelCount;
    }
}
