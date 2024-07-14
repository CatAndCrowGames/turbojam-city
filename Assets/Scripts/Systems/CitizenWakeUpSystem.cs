using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CitizenWakeUpSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        TimeData timeData = SystemAPI.GetSingleton<TimeData>();
        int currentHour = timeData.CurrentHour;
        float hourProgress = timeData.HourProgress;

        foreach (var (employmentData, transform, citizenEntity) in
            SystemAPI.Query<RefRO<EmploymentData>, RefRO<LocalTransform>>()
                .WithAll<WorkerAtHomeTag>()
                .WithNone<UnemployedTag,CitizenWentToWorkTag>()
                .WithEntityAccess())
        {
            Entity workPlaceEntity = employmentData.ValueRO.WorkPlaceEntity;
            WorkPlaceData workPlaceData = SystemAPI.GetComponent<WorkPlaceData>(workPlaceEntity);

            int startHour = workPlaceData.StartHour;
            float shiftLength = workPlaceData.ShiftLength;

            if (currentHour > startHour || (currentHour == startHour && hourProgress >= employmentData.ValueRO.WakeUpDelay))
            {
                float3 currentPosition = transform.ValueRO.Position;
                int3 workEntrancePosition = (int3)math.round(SystemAPI.GetComponent<EntranceData>(workPlaceEntity).EntrancePosition);
                ecb.RemoveComponent<WorkerAtHomeTag>(citizenEntity);
                ecb.AddComponent<GoingToWorkTag>(citizenEntity);
                if(SystemAPI.HasComponent<RebelTag>(citizenEntity)) ecb.SetComponent<ShaderColor>(citizenEntity, new() { Value = new(0, 5, 0, 1f) });
                else ecb.SetComponent<ShaderColor>(citizenEntity, new() { Value = new(5, 5, 5, 1f) });
                float3 buildingPosition = SystemAPI.GetComponent<LocalTransform>(workPlaceEntity).Position;
                ecb.AddComponent(citizenEntity, new PathTargetIntersection()
                { IntersectionPosition = workEntrancePosition, BuildingPosition = buildingPosition, BuildingEntity = workPlaceEntity });
                ecb.AddComponent<CitizenWentToWorkTag>(citizenEntity);
                ecb.SetComponent(citizenEntity, new WorkTimeLeft() { TimeLeft = workPlaceData.ShiftLength });
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}