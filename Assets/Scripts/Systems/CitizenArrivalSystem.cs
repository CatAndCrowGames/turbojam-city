using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CitizenArrivalSystem : ISystem
{
    Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random(12321313);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (transform,arrival, citizenEntity)
            in SystemAPI.Query<RefRW<LocalTransform>, RefRO<JustArrived>>().WithAll<GoingHomeTag>().WithEntityAccess())
        {
            float3 randomOffset = new(random.NextFloat(-.3f, .3f), 0f, random.NextFloat(-.3f, .3f));
            transform.ValueRW.Position = arrival.ValueRO.InteriorPosition + randomOffset;
            if (!SystemAPI.HasComponent<RebelTag>(citizenEntity)) ecb.SetComponent<ShaderColor>(citizenEntity, new() { Value = new(0.1f, 0.1f, 0.1f, 1f) });
            else ecb.SetComponent<ShaderColor>(citizenEntity, new() { Value = new(.1f, 1f, 0.1f, 1f) });
            ecb.RemoveComponent<JustArrived>(citizenEntity);
            ecb.RemoveComponent<GoingHomeTag>(citizenEntity);
            if (SystemAPI.HasComponent<CitizenWentToWorkTag>(citizenEntity)) ecb.RemoveComponent<CitizenWentToWorkTag>(citizenEntity);
            ecb.AddComponent<WorkerAtHomeTag>(citizenEntity);
        }

        foreach (var (transform, arrival, citizenEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<JustArrived>>().WithAll<GoingToWorkTag>().WithEntityAccess())
        {
            float3 randomOffset = new(random.NextFloat(-.3f, .3f), 0f, random.NextFloat(-.3f, .3f));
            transform.ValueRW.Position = arrival.ValueRO.InteriorPosition + randomOffset;
            ecb.SetComponent<ShaderColor>(citizenEntity, new() { Value = new(0.1f, 0.1f, 0.1f, 1f) });
            ecb.RemoveComponent<JustArrived>(citizenEntity);
            ecb.RemoveComponent<GoingToWorkTag>(citizenEntity);
            ecb.AddComponent<WorkingTag>(citizenEntity);
            Entity workPlaceEntity = arrival.ValueRO.BuildingEntity;
            if (SystemAPI.Exists(workPlaceEntity) && SystemAPI.HasComponent<WorkPlaceData>(workPlaceEntity))
            {
                WorkPlaceData workPlaceData = SystemAPI.GetComponent<WorkPlaceData>(workPlaceEntity);
                if (SystemAPI.HasComponent<RebelTag>(citizenEntity)) workPlaceData.CurrentRebels++;
                else workPlaceData.CurrentWorkers++;
                SystemAPI.SetComponent(workPlaceEntity, workPlaceData);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public partial struct JustArrived : IComponentData
{
    public float3 InteriorPosition;
    public Entity BuildingEntity;
}