using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BoundaryAuthoring : MonoBehaviour
{
    [SerializeField] float startPower;
    [SerializeField] float maxPower;
    [SerializeField] float workerPower;
    [SerializeField] float rebelPower;
    [SerializeField] Color offColor;
    [SerializeField] Color onColor;

    public class BoundaryBaker : Baker<BoundaryAuthoring>
    {
        public override void Bake(BoundaryAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            float4 offColorFloat = new(authoring.offColor.r, authoring.offColor.g, authoring.offColor.b, authoring.offColor.a);
            float4 onColorFloat = new(authoring.onColor.r, authoring.onColor.g, authoring.onColor.b, authoring.onColor.a);
            
            BoundaryData boundaryData = new();
            boundaryData.Power = authoring.startPower;
            boundaryData.OffColor = offColorFloat;
            boundaryData.OnColor = onColorFloat;
            boundaryData.WorkerPower = authoring.workerPower;
            boundaryData.RebelPower = authoring.rebelPower;
            boundaryData.MaxPower = authoring.maxPower;
            AddComponent(entity, boundaryData);

            AddComponent(entity, new ShaderColor() { Value = onColorFloat });
        }
    }
}

public struct BoundaryData : IComponentData
{
    public float Power;
    public float WorkerPower;
    public float RebelPower;
    public float MaxPower;
    public float4 OffColor;
    public float4 OnColor;
    public bool IsPowered;
}
