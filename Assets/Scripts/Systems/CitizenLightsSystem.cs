using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct CitizenLightsSystem : ISystem
{
    float3 darkerOrange;
    float3 brighterYellow;
    float3 darkerGrey;
    float3 brighterBlue;
    float3 brighterRed;
    float3 darkerBlue;
    float3 brightWhite;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        darkerOrange = new float3(1f, 0.5f, 0f);
        brighterYellow = new float3(10f, 7f, 0f);
        brighterRed = new float3(10f, 0f, 0f);
        darkerGrey = new float3(1f, 1f, 1f);
        darkerBlue = new float3(1f, 1f, 5f);
        brighterBlue = new float3(1f, 1f, 7f);
        brightWhite = new float3(5f, 5f, 5f);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float elapsedTime = (float)SystemAPI.Time.ElapsedTime * 2f;
        float pulseFactor = (math.sin(elapsedTime * 2f) + 1f) * 0.5f;

        foreach (var shaderColor in SystemAPI.Query<RefRW<ShaderColor>>().WithAll<FirePersonTag, WorkingTag>())
        {
            float3 newColor = math.lerp(darkerOrange, brighterYellow, pulseFactor);
            shaderColor.ValueRW.Value = new (newColor, 1f);
        }

        foreach (var shaderColor in SystemAPI.Query<RefRW<ShaderColor>>().WithAll<PolicePersonData, WorkingTag>())
        {
            float3 newColor = math.lerp(darkerBlue, brighterRed, pulseFactor);
            shaderColor.ValueRW.Value = new(newColor, 1f);
        }

        foreach (var shaderColor in SystemAPI.Query<RefRW<ShaderColor>>().WithAll<OfficeWorkerTag, WorkingTag>())
        {
            float3 newColor = math.lerp(darkerGrey, brighterBlue, pulseFactor);
            shaderColor.ValueRW.Value = new (newColor, 1f);
        }

        foreach (var (shaderColor, boundaryData) in SystemAPI.Query<RefRW<ShaderColor>, RefRO<BoundaryData>>())
        {
            if (!boundaryData.ValueRO.IsPowered) shaderColor.ValueRW.Value = new(darkerOrange,1f);

            float3 newColor = math.lerp(darkerBlue, brighterBlue, pulseFactor);
            shaderColor.ValueRW.Value = new (newColor, 1f);
        }
    }
}