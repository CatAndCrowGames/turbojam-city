using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float disruptionRange;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MoveSpeed>(entity, new() { Speed = authoring.moveSpeed });
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new DisruptionRangeData() { Range = authoring.disruptionRange });
            AddComponent(entity, new PlayerResistanceData() { Resistance = 1f });
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, disruptionRange);    
    }
}

public struct PlayerTag : IComponentData
{

}

public struct ActiveDisruptorTag : IComponentData
{
}


public struct DisruptionRangeData : IComponentData
{
    public float Range;
}


public struct PlayerResistanceData : IComponentData
{
    public float Resistance;
}
