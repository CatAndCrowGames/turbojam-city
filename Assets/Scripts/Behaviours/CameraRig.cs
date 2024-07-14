using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    EntityManager em;
    EntityQuery playerQuery;

    IEnumerator Start()
    {
        while (!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        playerQuery = em.CreateEntityQuery(new ComponentType[] { typeof(LocalTransform), typeof(PlayerTag) });
    }

    void LateUpdate()
    {
        if (playerQuery.IsEmpty) return;
        float3 followPosition = playerQuery.GetSingleton<LocalTransform>().Position;
        transform.position = Vector3.MoveTowards(transform.position, followPosition,speed);
    }
}
