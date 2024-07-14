using Unity.Entities;
using UnityEngine;

public class FirewatchAuthoring : MonoBehaviour
{
    public class FirewatchBaker : Baker<FirewatchAuthoring>
    {
        public override void Bake(FirewatchAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new FirewatchTag());
        }
    }
}

public struct FirewatchTag : IComponentData
{

}
