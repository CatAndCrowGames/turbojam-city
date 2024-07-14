using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct MoveToTargetSystem : ISystem
{
    NativeArray<int3> possibleMoves;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.CreateSingletonBuffer<BlockedIntersectionBE>();
        possibleMoves = new NativeArray<int3>(4,Allocator.Persistent);
        possibleMoves[0] = new(1, 0, 0);
        possibleMoves[1] = new(0, 0, -1);
        possibleMoves[2] = new(-1, 0, 0);
        possibleMoves[3] = new(0, 0, 1);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if(possibleMoves.IsCreated) possibleMoves.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float3 right = new(1, 0, 0);
        float3 up = new(0, 0, 1);
        float tolerance = 0.1f;
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        GameStateData gameState = SystemAPI.GetSingleton<GameStateData>();
        if (gameState.GameWon || gameState.GameOver) return;

        foreach (var (transform, intersection, moveSpeed, citizenEntity)
            in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MovingToIntersection>, RefRO<MoveSpeed>>().WithEntityAccess().WithAll<PathTargetIntersection>())
        {
            float3 position = transform.ValueRO.Position;
            float3 target = intersection.ValueRO.IntersectionPosition;
            float distance = math.distance(position, target);
            if (distance < tolerance) ecb.RemoveComponent<MovingToIntersection>(citizenEntity);
            else
            {
                float3 direction = target - position;
                direction = math.normalize(direction);
                float3 move = direction * moveSpeed.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                if (distance < math.length(move)) transform.ValueRW.Position = target;
                else transform.ValueRW.Position += move;
            }
        }

        DynamicBuffer<BlockedIntersectionBE> blockedIntersections = SystemAPI.GetSingletonBuffer<BlockedIntersectionBE>();
        foreach (var (transform, pathTarget, citizenEntity)
            in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PathTargetIntersection>>().WithEntityAccess().WithNone<MovingToIntersection>())
        {
            float3 position = transform.ValueRO.Position;
            float3 target = pathTarget.ValueRO.IntersectionPosition;
            if (math.distance(position, target) < tolerance)
            {
                ecb.RemoveComponent<PathTargetIntersection>(citizenEntity);
                if (!SystemAPI.HasComponent<PoliceHuntingTag>(citizenEntity))
                {
                    ecb.AddComponent<JustArrived>(citizenEntity, new() { InteriorPosition = pathTarget.ValueRO.BuildingPosition, BuildingEntity = pathTarget.ValueRO.BuildingEntity });
                }
                continue;
            }

            float3 direction = target - position;
            int3 currentIntersection = new int3((int)math.round(position.x), (int)math.round(position.y), (int)math.round(position.z));
            int3 targetIntersection = new int3((int)math.round(target.x), (int)math.round(target.y), (int)math.round(target.z));
            NativeList<int3> bestMoves = new NativeList<int3>(Allocator.Temp);
            float bestDistance = float.MaxValue;

            foreach (int3 move in possibleMoves)
            {
                int3 newIntersection = currentIntersection + move;
                if (!IsIntersectionBlocked(newIntersection, blockedIntersections)
                    ||SystemAPI.HasComponent<PlayerTag>(citizenEntity)
                    || SystemAPI.HasComponent<PoliceHuntingTag>(citizenEntity))
                {
                    float distanceToTarget = math.distancesq(newIntersection, targetIntersection);
                    if (distanceToTarget < bestDistance)
                    {
                        bestDistance = distanceToTarget;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }

                    else if (math.abs(distanceToTarget - bestDistance) < float.Epsilon)
                    {
                        bestMoves.Add(move);
                    }
                }
            }

            if (bestMoves.Length == 0) continue;
            int randomIndex = Random.CreateFromIndex((uint)citizenEntity.Index).NextInt(0, bestMoves.Length);
            int3 chosenMove = bestMoves[randomIndex];
            int3 nextIntersection = currentIntersection + chosenMove;
            ecb.AddComponent(citizenEntity, new MovingToIntersection { IntersectionPosition = nextIntersection });

            bestMoves.Dispose();
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    bool IsIntersectionBlocked(int3 intersection, DynamicBuffer<BlockedIntersectionBE> blockedIntersections)
    {
        foreach (var blockedIntersection in blockedIntersections)
        {
            if (blockedIntersection.IntersectionPosition.Equals(intersection))
                return true;
        }
        return false;
    }
}

public struct MovingToIntersection : IComponentData
{
    public int3 IntersectionPosition;
}

public struct PathTargetIntersection : IComponentData
{
    public int3 IntersectionPosition;
    public float3 BuildingPosition;
    public Entity BuildingEntity;
}

public struct BlockedIntersectionBE : IBufferElementData
{
    public int3 IntersectionPosition;
}