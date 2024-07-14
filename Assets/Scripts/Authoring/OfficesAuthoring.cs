using Unity.Entities;
using UnityEngine;

public class OfficesAuthoring : MonoBehaviour
{
    public class OfficesBaker : Baker<OfficesAuthoring>
    {
        public override void Bake(OfficesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new OfficesTag());
        }
    }
}

public struct OfficesTag : IComponentData
{

}
