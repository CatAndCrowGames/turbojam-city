using Unity.Entities;
using UnityEngine;

public class WorkPlaceAuthoring : MonoBehaviour
{
    [SerializeField] int numberOfJobs;
    [SerializeField] int startHour;
    [SerializeField] int shiftLength;

    public class WorkPlaceBaker : Baker<WorkPlaceAuthoring>
    {
        public override void Bake(WorkPlaceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Renderable);
            WorkPlaceData workPlaceData = new WorkPlaceData();
            workPlaceData.NumberOfJobs = authoring.numberOfJobs;
            workPlaceData.StartHour = authoring.startHour;
            workPlaceData.ShiftLength = authoring.shiftLength;
            workPlaceData.FilledJobs = 0;
            AddComponent(entity, workPlaceData);
            AddBuffer<WorkerBE>(entity);
        }
    }
}

public struct WorkPlaceData : IComponentData
{
    public int NumberOfJobs;
    public int FilledJobs;
    public int CurrentWorkers;
    public int CurrentRebels;
    public int StartHour;
    public float ShiftLength;
}

public struct WorkerBE : IBufferElementData
{
    public Entity CitizenEntity;
}

public struct WorkPlaceBE : IBufferElementData
{
    public Entity workplaceEntity;
}
