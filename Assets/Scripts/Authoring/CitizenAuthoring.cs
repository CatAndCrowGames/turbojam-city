using Unity.Entities;
using UnityEngine;

public class CitizenAuthoring : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    public class CitizenBaker : Baker<CitizenAuthoring>
    {
        public override void Bake(CitizenAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MoveSpeed>(entity, new() { Speed = authoring.moveSpeed });
            AddComponent(entity, new CitizenTag());
            AddComponent(entity, new UnemployedTag());
            AddComponent(entity, new EmploymentData() { WorkPlaceEntity = Entity.Null });
            AddComponent(entity, new HomeData());
            AddComponent(entity, new ShaderColor() { Value = new(1, 1, 1, 1) });
            AddComponent<WorkTimeLeft>(entity);
            AddComponent<RebellionLevel>(entity);
        }
    }
}

public struct CitizenTag : IComponentData
{

}

public struct UnemployedTag : IComponentData
{

}

public struct WorkerAtHomeTag : IComponentData
{

}

public struct GoingToWorkTag : IComponentData
{

}


public struct GoingHomeTag : IComponentData
{

}

public struct WorkingTag : IComponentData
{

}

public struct CitizenWentToWorkTag : IComponentData
{

}

public struct EmploymentData : IComponentData
{
    public Entity WorkPlaceEntity;
    public float WakeUpDelay;
}

public struct HomeData : IComponentData
{
    public Entity HomeEntity;
}

public struct MoveSpeed : IComponentData
{
    public float Speed;
}

public struct WorkTimeLeft : IComponentData
{
    public float TimeLeft;
}

public struct RebellionLevel : IComponentData
{
    public float Rebellion;
}