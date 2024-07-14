using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TileAuthoring : MonoBehaviour
{
    public class TileBaker : Baker<TileAuthoring>
    {
        public override void Bake(TileAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent<EntranceData>(entity);
        }
    }
}

public struct EntranceData : IComponentData
{
    public float3 EntrancePosition;
}
