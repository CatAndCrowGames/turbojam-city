using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct DisruptionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        DynamicBuffer<IntersectionBE> intersectionBuffer;
        if (!SystemAPI.TryGetSingletonBuffer(out intersectionBuffer)) return;

        Entity blockedIntersectionBufferEntity = SystemAPI.GetSingletonEntity<BlockedIntersectionBE>();
        var blockedIntersectionBuffer = state.EntityManager.GetBuffer<BlockedIntersectionBE>(blockedIntersectionBufferEntity);
        blockedIntersectionBuffer.Clear();
        PrefabsData prefabsData;
        if (!SystemAPI.TryGetSingleton(out prefabsData)) return;
        
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        int blockedCount = 0;

        foreach (var intersectionBE in intersectionBuffer)
        {
            bool blocked = false;
            foreach (var (disruptorTransform, disruptorRange) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<DisruptionRangeData>>().WithAll<ActiveDisruptorTag>())
            {
                float3 disruptorPosition = disruptorTransform.ValueRO.Position;
                if (math.distance(intersectionBE.Position, disruptorPosition) < disruptorRange.ValueRO.Range)
                {
                    blocked = true;
                    Entity markerEntity = ecb.Instantiate(prefabsData.DisruptionMarkerPrefab);
                    LocalTransform markerTransform = LocalTransform.FromPosition(intersectionBE.Position);
                    ecb.SetComponent(markerEntity, markerTransform);
                    break;
                }
            }

            if (blocked)
            {
                blockedIntersectionBuffer.Add(new BlockedIntersectionBE { IntersectionPosition = new int3((int)math.round(intersectionBE.Position.x), (int)math.round(intersectionBE.Position.y), (int)math.round(intersectionBE.Position.z)) });
                blockedCount++;
            }
        }

        foreach (var (citizenTransform,citizenRebellionLevel,citizenEntity) in SystemAPI.Query<RefRO<LocalTransform>,RefRW<RebellionLevel>>().WithAll<CitizenTag>().WithAny<GoingToWorkTag,GoingHomeTag>().WithEntityAccess().WithNone<FirePersonTag,PolicePersonData>())
        {
            bool blocked = false;
            foreach (var (disruptorRange, disruptorTransform) in SystemAPI.Query<RefRO<DisruptionRangeData>, RefRO<LocalTransform>>().WithAll<ActiveDisruptorTag>())
            {
                float3 disruptorPosition = disruptorTransform.ValueRO.Position;
                float3 citizenPosition = citizenTransform.ValueRO.Position;
                if (math.distance(citizenPosition, disruptorPosition) < disruptorRange.ValueRO.Range)
                {
                    blocked = true;
                    break;
                }
            }

            if (blocked)
            {
                citizenRebellionLevel.ValueRW.Rebellion += deltaTime;
                if (citizenRebellionLevel.ValueRO.Rebellion >= .2f)
                {
                    ecb.AddComponent<RebelTag>(citizenEntity);
                    ecb.SetComponent(citizenEntity, new ShaderColor() { Value = new(0, 5, 0, 1) });
                    GameStateData stateData = SystemAPI.GetSingleton<GameStateData>();
                    stateData.RebelCount++;
                    SystemAPI.SetSingleton(stateData);
                }
            }

            else
            {
                citizenRebellionLevel.ValueRW.Rebellion -= deltaTime/150f;
                if (citizenRebellionLevel.ValueRO.Rebellion <= .2f)
                {
                    ecb.RemoveComponent<RebelTag>(citizenEntity);
                    ecb.SetComponent(citizenEntity, new ShaderColor() { Value = new(5, 5, 5, 1) });
                }
            }
        }

        foreach (var (markerData, markerEntity) in SystemAPI.Query<RefRW<DisruptionMarkerData>>().WithEntityAccess())
        {
            markerData.ValueRW.LifeTime -= deltaTime;
            if (markerData.ValueRO.LifeTime <= 0) ecb.DestroyEntity(markerEntity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public struct RebelTag : IComponentData
{

}