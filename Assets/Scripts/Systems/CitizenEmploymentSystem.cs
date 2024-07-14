using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct CitizenEmploymentSystem : ISystem
{
    Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random(921731296);
        state.RequireForUpdate<UnemployedTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        DynamicBuffer<WorkPlaceBE> workPlaceBuffer;
        if (!SystemAPI.TryGetSingletonBuffer(out workPlaceBuffer) || !workPlaceBuffer.IsCreated)
        {
            ecb.Dispose();
            return;
        }

        // Check if there are any unemployed citizens
        if (!SystemAPI.QueryBuilder().WithAll<UnemployedTag>().Build().IsEmpty)
        {
            var availableWorkplaces = new NativeList<WorkPlaceBE>(Allocator.Temp);
            foreach (var workplace in workPlaceBuffer)
            {
                WorkPlaceData workPlaceData = SystemAPI.GetComponent<WorkPlaceData>(workplace.workplaceEntity);
                if (workPlaceData.NumberOfJobs > workPlaceData.FilledJobs) availableWorkplaces.Add(workplace);
            }

            if (availableWorkplaces.Length > 0)
            {
                foreach (var (employmentData, citizenEntity) in SystemAPI.Query<RefRW<EmploymentData>>().WithAll<UnemployedTag>().WithEntityAccess())
                {
                    if (availableWorkplaces.Length == 0) break;

                    int randomIndex = random.NextInt(0, availableWorkplaces.Length);
                    WorkPlaceBE chosenWorkplace = availableWorkplaces[randomIndex];

                    WorkPlaceData workPlaceData = SystemAPI.GetComponent<WorkPlaceData>(chosenWorkplace.workplaceEntity);

                    ecb.RemoveComponent<UnemployedTag>(citizenEntity);
                    Entity workplaceEntity = chosenWorkplace.workplaceEntity;
                    if (SystemAPI.HasComponent<FirewatchTag>(workplaceEntity)) ecb.AddComponent<FirePersonTag>(citizenEntity);
                    else if (SystemAPI.HasComponent<PoliceStationTag>(workplaceEntity)) ecb.AddComponent<PolicePersonData>(citizenEntity);
                    else if (SystemAPI.HasComponent<OfficesTag>(workplaceEntity)) ecb.AddComponent<OfficeWorkerTag>(citizenEntity);
                    employmentData.ValueRW.WorkPlaceEntity = workplaceEntity;
                    employmentData.ValueRW.WakeUpDelay = random.NextFloat(0, 0.8f);
                    workPlaceData.FilledJobs++;
                    SystemAPI.SetComponent(chosenWorkplace.workplaceEntity, workPlaceData);
                    if (workPlaceData.NumberOfJobs <= workPlaceData.FilledJobs) availableWorkplaces.RemoveAtSwapBack(randomIndex);

                }
            }

            availableWorkplaces.Dispose();
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public struct FirePersonTag : IComponentData
{

}

public struct PolicePersonData : IComponentData
{
    public Entity huntedEntity;
}

public struct OfficeWorkerTag : IComponentData
{

}