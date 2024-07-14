using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class ResistanceDisplay : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    Entity playerEntity;
    
    VisualElement bar;
    VisualElement root;
    EntityQuery playerQuery;
    EntityManager em;

    float resistanceLastFrame;

    void Awake()
    {
        root = uiDocument.rootVisualElement;
        bar = root.Query<VisualElement>("Bar");
    }

    IEnumerator Start()
    {
        while(!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerQuery = em.CreateEntityQuery(new ComponentType[] { typeof(PlayerResistanceData) });
        yield return null;

    }

    void Update()
    {
        if (playerQuery.IsEmpty) return;
        
        float resistance = playerQuery.GetSingleton<PlayerResistanceData>().Resistance;
        if (bar == null) return;
        bar.style.width = Length.Percent(resistance * 100);
        if (resistance >= 1f) root.style.display = DisplayStyle.None;
        else root.style.display = DisplayStyle.Flex;
        if (resistance < resistanceLastFrame) SFXPlayer.PlayPoliceSound();
        resistanceLastFrame = resistance;
    }
}
