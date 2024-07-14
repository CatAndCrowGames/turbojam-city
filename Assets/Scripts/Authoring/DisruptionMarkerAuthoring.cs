using Unity.Entities;
using UnityEngine;

public class DisruptionMarkerAuthoring : MonoBehaviour
{
    public class DisruptionMarkerBaker : Baker<DisruptionMarkerAuthoring>
    {
        public override void Bake(DisruptionMarkerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DisruptionMarkerData() { LifeTime = 1f });
        }
    }
}

public struct DisruptionMarkerData : IComponentData
{
    public float LifeTime;
}