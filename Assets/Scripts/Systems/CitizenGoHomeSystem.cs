using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(PoliceDispatchSystem))]
public partial struct CitizenGoHomeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        TimeData timeData = SystemAPI.GetSingleton<TimeData>();
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (employmentData, homeData, transform, workTimer,citizenEntity) in
            SystemAPI.Query<RefRO<EmploymentData>, RefRO<HomeData>, RefRO<LocalTransform>,RefRW<WorkTimeLeft>>()
                .WithAll<WorkingTag>()
                .WithEntityAccess())
        {
            Entity workPlaceEntity = employmentData.ValueRO.WorkPlaceEntity;
            WorkPlaceData workPlaceData = SystemAPI.GetComponent<WorkPlaceData>(workPlaceEntity);
            workTimer.ValueRW.TimeLeft -= deltaTime*timeData.TimeMultiplier;
            if (workTimer.ValueRO.TimeLeft<=0)
            {
                if (SystemAPI.HasComponent<PoliceHuntingTag>(citizenEntity)) ecb.RemoveComponent<PoliceHuntingTag>(citizenEntity);
                Entity homeEntity = homeData.ValueRO.HomeEntity;
                float3 buildingPosition = SystemAPI.GetComponent<LocalTransform>(homeEntity).Position;
                float3 currentPosition = transform.ValueRO.Position;
                int3 homePosition = (int3) math.round(SystemAPI.GetComponent<EntranceData>(homeEntity).EntrancePosition);

                ecb.RemoveComponent<WorkingTag>(citizenEntity);
                ecb.AddComponent<GoingHomeTag>(citizenEntity);
                if (!SystemAPI.HasComponent<RebelTag>(citizenEntity)) ecb.SetComponent<ShaderColor>(citizenEntity, new() { Value = new(5, 5, 5, 1f) });
                ecb.AddComponent(citizenEntity, new PathTargetIntersection() { IntersectionPosition = homePosition , BuildingPosition = buildingPosition, BuildingEntity = homeEntity});
                if(SystemAPI.HasComponent<RebelTag>(citizenEntity)) workPlaceData.CurrentRebels--;
                else workPlaceData.CurrentWorkers--;
                SystemAPI.SetComponent(workPlaceEntity, workPlaceData);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

