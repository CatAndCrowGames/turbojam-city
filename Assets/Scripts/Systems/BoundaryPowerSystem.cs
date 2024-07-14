using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct BoundaryPowerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        Entity boundaryEntity;
        if (!SystemAPI.TryGetSingletonEntity<BoundaryData>(out boundaryEntity)) return;
        BoundaryData boundaryData = SystemAPI.GetSingleton<BoundaryData>();
        GameStateData gameState = SystemAPI.GetSingleton<GameStateData>();
        if (gameState.GameWon) return;
        if (gameState.GameOver) return;

        foreach (var workPlace in SystemAPI.Query<RefRW<WorkPlaceData>>().WithAll<OfficesTag>())
        {
            boundaryData.Power+=workPlace.ValueRO.CurrentWorkers*deltaTime *boundaryData.WorkerPower;
            boundaryData.Power-=workPlace.ValueRO.CurrentRebels*deltaTime *boundaryData.RebelPower;
        }

        boundaryData.Power = math.clamp(boundaryData.Power, 0, boundaryData.MaxPower);
        boundaryData.IsPowered = boundaryData.Power > 0;
        if (boundaryData.Power <= 0)
        {
            gameState.GameWon = true;
            SystemAPI.SetSingleton(gameState);
        }
        SystemAPI.SetSingleton(boundaryData);
    }
}
