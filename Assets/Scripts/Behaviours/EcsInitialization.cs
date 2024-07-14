using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum MouseHoverType
{
    none,
    residency,
    firewatch,
    offices,
    nightoffices,
    policeStation
}

public class EcsInitialization : MonoBehaviour
{
    public bool IsGenerating => generating;
    public int Size=> size;


    [SerializeField] int size;
    [SerializeField] int randomSeed = 12999;
    [SerializeField] int numberOfFirewatches;
    [SerializeField] int numberOfOffices;
    [SerializeField] int numberOfNightOffices;
    [SerializeField] int numberOfPoliceStations;

    List<int2> officesCoords;
    List<int2> nightOfficesCoords;
    List<int2> policeStationCoords;
    List<int2> fireWatchCoords;

    List<int2> validCoords;
    List<int2> coordsWithoutWorkPlaces;

    List<Entity> tiles;
    List<Entity> citizens;
    EntityManager em;

    Entity workPlaceBufferEntity;
    Entity intersectionBufferEntity;

    bool generating;

    IEnumerator Start()
    {
        while (!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (!generating)
        {
            generating = true;
            StartCoroutine(CreateTiles());
        }
        else Debug.Log("Generation already in progress!");
    }

    public IEnumerator CreateTiles()
    {
        yield return null;
        yield return null;
        ResetGame();
        yield return null;
        yield return null;


        officesCoords = new();
        nightOfficesCoords = new();
        policeStationCoords = new();
        fireWatchCoords = new();
        validCoords = new();
        coordsWithoutWorkPlaces = new();

        tiles = new();
        citizens = new();
        UnityEngine.Random.InitState(randomSeed);

        while (!World.DefaultGameObjectInjectionWorld.IsCreated) yield return null;
        EntityCommandBuffer ecb = new(Allocator.Persistent);

        workPlaceBufferEntity = em.CreateSingletonBuffer<WorkPlaceBE>();
        var workPlaceBuffer = em.GetBuffer<WorkPlaceBE>(workPlaceBufferEntity);
        intersectionBufferEntity = em.CreateSingletonBuffer<IntersectionBE>();
        var intersectionBuffer = em.GetBuffer<IntersectionBE>(intersectionBufferEntity);

        EntityQuery timeQuery = em.CreateEntityQuery(new ComponentType[] { typeof(TimeData) });
        while (timeQuery.IsEmpty) yield return null;
        TimeData timeData = timeQuery.GetSingleton<TimeData>();
        timeData.CurrentHour = 4;
        timeData.CurrentDay = 0;
        timeQuery.SetSingleton(timeData);

        EntityQuery stateQuery = em.CreateEntityQuery(new ComponentType[] { typeof(GameStateData) });
        while (stateQuery.IsEmpty) yield return null;
        GameStateData stateData = stateQuery.GetSingleton<GameStateData>();
        stateData.GameOver = false;
        stateData.GameWon = false;
        stateQuery.SetSingleton(stateData);

        EntityQuery playerQuery = em.CreateEntityQuery(new ComponentType[] { typeof(PlayerResistanceData) });
        while (playerQuery.IsEmpty) yield return null;
        PlayerResistanceData resistanceData = playerQuery.GetSingleton<PlayerResistanceData>();
        resistanceData.Resistance = 1f;
        playerQuery.SetSingleton(resistanceData);

        EntityQuery boundaryQuery = em.CreateEntityQuery(new ComponentType[] { typeof(BoundaryData) });
        while(boundaryQuery.IsEmpty) yield return null;
        Entity boundaryEntity = boundaryQuery.GetSingletonEntity();
        BoundaryData boundaryData = boundaryQuery.GetSingleton<BoundaryData>();
        boundaryData.Power = boundaryData.MaxPower / 2f;
        boundaryQuery.SetSingleton(boundaryData);

        LocalTransform boundaryTransform = new();
        boundaryTransform.Position = new(0,1,0);
        boundaryTransform.Rotation = quaternion.Euler(math.radians(90),0,0);
        boundaryTransform.Scale = size+1;
        em.SetComponentData(boundaryEntity, boundaryTransform);

        EntityQuery prefabsQuery = em.CreateEntityQuery(new ComponentType[] { typeof(PrefabsData) });
        while (prefabsQuery.IsEmpty) yield return null;
        PrefabsData prefabs = prefabsQuery.GetSingleton<PrefabsData>();
        
        int2 currentCoord;
        for (currentCoord.y = -size / 2; currentCoord.y < size / 2; currentCoord.y++)
        {
            for (currentCoord.x = -size / 2; currentCoord.x < size / 2; currentCoord.x++)
            {
                if (math.length(currentCoord) > size / 2) continue;
                validCoords.Add(currentCoord);
            }
        }

        coordsWithoutWorkPlaces = new(validCoords);


        int numberOfTiles = size*size;
        for(int i = 0; i< numberOfOffices;i++)
        {
            int2 coord = Helpers.RandomFromList(coordsWithoutWorkPlaces);
            coordsWithoutWorkPlaces.Remove(coord);
            officesCoords.Add(coord);
        }

        for (int i = 0; i < numberOfNightOffices; i++)
        {
            int2 coord = Helpers.RandomFromList(coordsWithoutWorkPlaces);
            coordsWithoutWorkPlaces.Remove(coord);
            nightOfficesCoords.Add(coord);
        }

        for (int i = 0; i < numberOfFirewatches; i++)
        {
            int2 coord = Helpers.RandomFromList(coordsWithoutWorkPlaces);
            coordsWithoutWorkPlaces.Remove(coord);
            fireWatchCoords.Add(coord);
        }

        for (int i = 0; i < numberOfPoliceStations; i++)
        {
            int2 coord = Helpers.RandomFromList(coordsWithoutWorkPlaces);
            coordsWithoutWorkPlaces.Remove(coord);
            policeStationCoords.Add(coord);
        }

        float3 offsetForTiles = new(.5f,0,.5f);
        foreach (int2 coord in validCoords)
        {
            Entity tilePrefab = SelectBuildingPrefab(prefabs, coord);
            float3 position = new(coord.x, 0, coord.y);
            ecb.AppendToBuffer(intersectionBufferEntity, new IntersectionBE() { Position = (int3)position });
            Entity tileEntity = em.Instantiate(tilePrefab);
            LocalTransform tileTransform = new() { Position = position + offsetForTiles, Rotation = quaternion.identity, Scale = 1f };
            em.SetComponentData(tileEntity, tileTransform);
            tiles.Add(tileEntity);
            if (em.HasComponent<WorkPlaceData>(tileEntity)) ecb.AppendToBuffer(workPlaceBufferEntity, new WorkPlaceBE() { workplaceEntity = tileEntity });
            SpawnCitizens(prefabs, position, tileEntity);
            Entity entranceEntity = em.Instantiate(prefabs.EntrancePrefab);
            int entranceDirection = UnityEngine.Random.Range(0, 4);
            float3 entranceOffset = Vector3.back;
            quaternion entranceRotation = quaternion.identity;
            float radians = math.radians(90);

            switch (entranceDirection)
            {
                case 0:
                    entranceOffset = Vector3.right;
                    entranceRotation = quaternion.Euler(0, radians, 0);
                    break;
                case 1:
                    entranceOffset = Vector3.left;
                    entranceRotation = quaternion.Euler(0, -radians, 0);
                    break;
                case 2:
                    entranceOffset = Vector3.forward;
                    break;
                case 3:
                    entranceOffset = Vector3.back;
                    entranceRotation = quaternion.Euler(0, radians * 2f, 0);
                    break;

                default:
                    break;

            }

            entranceOffset *= 0.5f;

            float3 entrancePosition = entranceOffset + position + offsetForTiles;

            LocalTransform entranceTransform = new() { Position = entrancePosition, Rotation = entranceRotation, Scale = 1 };
            em.SetComponentData(entranceEntity, entranceTransform);

            EntranceData entranceData = new();
            entranceData.EntrancePosition = entrancePosition;
            em.SetComponentData(tileEntity, entranceData);

        }

        ecb.Playback(em);
        ecb.Dispose();
        generating = false;
    }

    Entity SelectBuildingPrefab(PrefabsData prefabs, int2 currentCoord)
    {
        Entity tilePrefab;

        if (fireWatchCoords.Contains(new(currentCoord.x, currentCoord.y))) tilePrefab = prefabs.FirewatchPrefab;
        else if (policeStationCoords.Contains(new(currentCoord.x, currentCoord.y))) tilePrefab = prefabs.PoliceStationPrefab;
        else if (officesCoords.Contains(new(currentCoord.x, currentCoord.y))) tilePrefab = prefabs.OfficesPrefab;
        else if (nightOfficesCoords.Contains(new(currentCoord.x, currentCoord.y))) tilePrefab = prefabs.NightOfficesPrefab;

        else tilePrefab = prefabs.ResidenciesPrefab;
        return tilePrefab;
    }

    public MouseHoverType HoverTypeAt(int2 coord)
    {
        if ((fireWatchCoords == null)
        || policeStationCoords == null
        || officesCoords == null
        || nightOfficesCoords == null
        || validCoords == null) return MouseHoverType.none;

        if (fireWatchCoords.Contains(new(coord.x, coord.y))) return MouseHoverType.firewatch;
        else if (policeStationCoords.Contains(new(coord.x, coord.y))) return MouseHoverType.policeStation;
        else if (officesCoords.Contains(new(coord.x, coord.y))) return MouseHoverType.offices;
        else if (nightOfficesCoords.Contains(new(coord.x, coord.y))) return MouseHoverType.nightoffices;
        else if (validCoords.Contains(coord)) return MouseHoverType.residency;
        return MouseHoverType.none;
    }

    void SpawnCitizens(PrefabsData prefabs, float3 position, Entity homeEntity)
    {
        int citizenCount = UnityEngine.Random.Range(1, 4);
        for (int citizenIndex = 0; citizenIndex < citizenCount; citizenIndex++) SpawnCitizen(prefabs, position, homeEntity);
    }

    void SpawnCitizen(PrefabsData prefabs, float3 position, Entity homeEntity)
    {
        Entity citizenEntity = em.Instantiate(prefabs.CitizenPrefab);

        if (Helpers.CoinFlip()) position += new float3 (UnityEngine.Random.Range(-0.5f, 0.5f),0f,0f);
        else position += new float3 (0f,0f,UnityEngine.Random.Range(-0.5f, 0.5f));

        LocalTransform citizenTransform = new LocalTransform() { Position = position, Rotation = quaternion.identity, Scale = .1f };
        em.SetComponentData(citizenEntity, citizenTransform);
        em.SetComponentData<HomeData>(citizenEntity, new() { HomeEntity = homeEntity });
        em.AddComponent<GoingHomeTag>(citizenEntity);
        LocalTransform homeTransform = em.GetComponentData<LocalTransform>(homeEntity);
        em.AddComponentData(citizenEntity, new JustArrived() { BuildingEntity = homeEntity, InteriorPosition = homeTransform.Position } );
        citizens.Add(citizenEntity);
    }

    void ClearTiles()
    {
        if (tiles == null) return;
        for (int tileIndex = 0; tileIndex < tiles.Count; tileIndex++)
        {
            Entity tileEntity = tiles[tileIndex];
            if (em.Exists(tileEntity)) em.DestroyEntity(tileEntity);
        }
        tiles.Clear();
    }

    void ResetGame()
    {
        DestroyCitizens();
        ClearTiles();
        ClearBuffers();
    }

    void ClearBuffers()
    {
        if (em.Exists(workPlaceBufferEntity)) em.DestroyEntity(workPlaceBufferEntity);
        if (em.Exists(intersectionBufferEntity)) em.DestroyEntity(intersectionBufferEntity);
    }

    void DestroyCitizens()
    {
        if (citizens == null) return;
        for (int citizenIndex = 0; citizenIndex < citizens.Count; citizenIndex++)
        {
            Entity citizenEntity = citizens[citizenIndex];
            if (em.Exists(citizenEntity)) em.DestroyEntity(citizenEntity);
        }
        citizens.Clear();
    }
}

public struct IntersectionBE : IBufferElementData
{
    public int3 Position;
}