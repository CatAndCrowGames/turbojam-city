using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;

[BurstCompile]
public partial struct PoliceResistance : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        GameStateData gameState = SystemAPI.GetSingleton<GameStateData>();
        if (gameState.GameWon || gameState.GameOver) return;

        float deltaTime = SystemAPI.Time.DeltaTime;
        float deltaCopTime = deltaTime* 0.02f;
        int policeCount = 0;
        float policeFightingRange = 1.9f;
        foreach (var (playerTransform, playerResistance, playerEntity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PlayerResistanceData>>().WithEntityAccess())
        {
            float3 playerPosition = playerTransform.ValueRO.Position;
            float resistance = playerResistance.ValueRO.Resistance;
            if (resistance <= 0f)
            {
                gameState.GameOver = true;
                SystemAPI.SetSingleton(gameState);
                return;
            }

            resistance += deltaTime/10f;

            foreach (var policeTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PoliceHuntingTag>())
            {
                float3 policePosition = policeTransform.ValueRO.Position;
                if (math.distance(policePosition, playerPosition) < policeFightingRange)
                {
                    resistance -= deltaCopTime;
                    policeCount++;
                    if (policeCount > 20) break;
                }
            }

            resistance = math.clamp(resistance, 0f, 1f);
            playerResistance.ValueRW.Resistance = resistance;
        }
    }
}
