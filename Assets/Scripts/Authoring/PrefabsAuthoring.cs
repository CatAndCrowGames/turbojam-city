using Unity.Entities;
using UnityEngine;

public class PrefabsAuthoring : MonoBehaviour
{
    [SerializeField] GameObject residenciesPrefab;
    [SerializeField] GameObject firewatchPrefab;
    [SerializeField] GameObject policeStationPrefab;
    [SerializeField] GameObject officesPrefab;
    [SerializeField] GameObject nightOfficesPrefab;
    [SerializeField] GameObject citizenPrefab;
    [SerializeField] GameObject entrancePrefab;
    [SerializeField] GameObject disruptionMarkerPrefab;

    public class PrefabsBaker : Baker<PrefabsAuthoring>
    {
        public override void Bake(PrefabsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            PrefabsData data = new PrefabsData();
            data.ResidenciesPrefab = GetEntity(authoring.residenciesPrefab, TransformUsageFlags.Dynamic);
            data.FirewatchPrefab = GetEntity(authoring.firewatchPrefab, TransformUsageFlags.Dynamic);
            data.OfficesPrefab = GetEntity(authoring.officesPrefab, TransformUsageFlags.Dynamic);
            data.NightOfficesPrefab = GetEntity(authoring.nightOfficesPrefab, TransformUsageFlags.Dynamic);
            data.PoliceStationPrefab = GetEntity(authoring.policeStationPrefab, TransformUsageFlags.Dynamic);
            data.CitizenPrefab = GetEntity(authoring.citizenPrefab, TransformUsageFlags.Dynamic);
            data.EntrancePrefab = GetEntity(authoring.entrancePrefab, TransformUsageFlags.Dynamic);
            data.DisruptionMarkerPrefab = GetEntity(authoring.disruptionMarkerPrefab, TransformUsageFlags.Dynamic);
            AddComponent(entity, data);
        }
    }
}

public struct PrefabsData : IComponentData
{
    public Entity ResidenciesPrefab;
    public Entity FirewatchPrefab;
    public Entity OfficesPrefab;
    public Entity PoliceStationPrefab;
    public Entity NightOfficesPrefab;
    public Entity CitizenPrefab;
    public Entity EntrancePrefab;
    public Entity DisruptionMarkerPrefab;
}
