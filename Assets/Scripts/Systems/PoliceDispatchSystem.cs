using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public partial struct PoliceDispatchSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entity playerEntity;
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out playerEntity)) return;
        float3 playerPosition = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

        BoundaryData boundaryData = SystemAPI.GetSingleton<BoundaryData>();
        bool boundaryPowerLow = boundaryData.Power < 300;

        foreach(var (policePersonData, policePersonEntity)
            in SystemAPI.Query<RefRW<PolicePersonData>>().WithAll<WorkingTag>().WithNone<RebelTag>().WithEntityAccess())
        {
            foreach(var (disruptorTransform,disruptorEntity)
                in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<ActiveDisruptorTag>().WithEntityAccess())
            {
                PathTargetIntersection pathTargetIntersection = new();
                pathTargetIntersection.IntersectionPosition = (int3)math.round(disruptorTransform.ValueRO.Position); 
                ecb.AddComponent(policePersonEntity, pathTargetIntersection);
                ecb.AddComponent<PoliceHuntingTag>(policePersonEntity);
                if (boundaryPowerLow) ecb.SetComponent(policePersonEntity, new MoveSpeed() { Speed = 3.3f });
                else ecb.SetComponent(policePersonEntity, new MoveSpeed() { Speed = 3f });
                policePersonData.ValueRW.huntedEntity = disruptorEntity;
                break;
            }
        }

        foreach(var (policeTransform,policePersonEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PoliceHuntingTag>().WithEntityAccess())
        {
            float3 policePosition=policeTransform.ValueRO.Position;
            if(math.distance(policePosition,playerPosition)<5f)
            {
                PathTargetIntersection pathTargetIntersection = new();
                pathTargetIntersection.IntersectionPosition = (int3)math.round(playerPosition);
                ecb.AddComponent(policePersonEntity, pathTargetIntersection);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    
}

public struct PoliceHuntingTag : IComponentData
{
    public int randomIntersectionsToCheck;
}
