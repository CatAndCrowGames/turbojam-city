using System;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    EcsInitialization init;
    HoverDisplay hoverDisplay;
    Camera cam;
    Ray mouseRay;
    float mouseRayIntersectionDistance;
    Vector3 mousePositionOnPlane;
    int2 mouseCoord;
    int3 mousePositionOnRoads;
    Plane plane;

    EntityManager em;
    EntityQuery playerQuery;
    Entity playerEntity;
    LocalTransform playerTransform;

    void Awake()
    {
        cam = Camera.main;
        plane = new Plane(Vector3.up, Vector3.zero);
        init = FindFirstObjectByType<EcsInitialization>();
        hoverDisplay = FindFirstObjectByType<HoverDisplay>();
    }

    IEnumerator Start()
    {
        while (!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerQuery = em.CreateEntityQuery(new ComponentType[] { typeof(PlayerTag) });
        while (playerQuery.IsEmpty) yield return null;
        playerEntity = playerQuery.GetSingletonEntity();
    }

    void Update()
    {
        if (playerQuery.IsEmpty) return;
        if (playerEntity == Entity.Null) return;

        GetMousePositions();
        HandlePlayerMovement();
        HandlePlayerDisruption();
        hoverDisplay.Show(init.HoverTypeAt(mouseCoord));        
    }

    void GetMousePositions()
    {
        mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(mouseRay, out mouseRayIntersectionDistance);
        mousePositionOnPlane = mouseRay.GetPoint(mouseRayIntersectionDistance);
        mousePositionOnPlane = Vector3.ClampMagnitude(mousePositionOnPlane, init.Size/2f - 1);

        mousePositionOnRoads = new int3(
            Mathf.RoundToInt(mousePositionOnPlane.x),
            0,
            Mathf.RoundToInt(mousePositionOnPlane.z)
        );
        mouseCoord = new(Mathf.RoundToInt(mousePositionOnPlane.x-0.5f), Mathf.RoundToInt(mousePositionOnPlane.z - 0.5f));
    }

    void HandlePlayerMovement()
    {
        playerTransform = em.GetComponentData<LocalTransform>(playerEntity);
        float3 currentPosition = playerTransform.Position;
        
        if (Input.GetMouseButton(0))
        {
            if (em.HasComponent<PathTargetIntersection>(playerEntity))
                em.SetComponentData(playerEntity, new PathTargetIntersection { IntersectionPosition = mousePositionOnRoads });
            else
                em.AddComponentData(playerEntity, new PathTargetIntersection { IntersectionPosition = mousePositionOnRoads });
        }

        else
        {
            if (em.HasComponent<PathTargetIntersection>(playerEntity)) em.RemoveComponent<PathTargetIntersection>(playerEntity);
            if (em.HasComponent<MovingToIntersection>(playerEntity)) em.RemoveComponent<PathTargetIntersection>(playerEntity);
        }
    }

    void HandlePlayerDisruption()
    {
        if (Input.GetMouseButton(1) && !em.HasComponent<ActiveDisruptorTag>(playerEntity))
        {
            em.AddComponentData(playerEntity, new ActiveDisruptorTag());
        }

        else if (!Input.GetMouseButton(1) && em.HasComponent<ActiveDisruptorTag>(playerEntity))
        {
            em.RemoveComponent<ActiveDisruptorTag>(playerEntity);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green * 0.5f;
        Gizmos.DrawRay(mouseRay.origin, mouseRay.direction * 1000f);
        Gizmos.DrawSphere(mousePositionOnPlane, .1f);
        Gizmos.DrawWireSphere(new Vector3(mousePositionOnRoads.x, 0, mousePositionOnRoads.z), .2f);
    }
}