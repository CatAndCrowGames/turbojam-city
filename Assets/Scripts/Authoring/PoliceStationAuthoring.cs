using Unity.Entities;
using UnityEngine;

public class PoliceStationAuthoring : MonoBehaviour
{
    public class PoliceStationBaker : Baker<PoliceStationAuthoring>
    {
        public override void Bake(PoliceStationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new PoliceStationTag());
        }
    }
}

public struct PoliceStationTag : IComponentData
{

}
