using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct TimeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        EntityManager em = state.EntityManager;
        em.CreateSingleton(new TimeData() { CurrentHour = 4, HourProgress = 0, TimeMultiplier = 0.5f, CurrentDay = 0 });
    }

    public void OnUpdate(ref SystemState state)
    {
        TimeData currentTime = SystemAPI.GetSingleton<TimeData>();
        
        currentTime.HourProgress += SystemAPI.Time.DeltaTime * currentTime.TimeMultiplier;
        if (currentTime.HourProgress >= 1f)
        {
            currentTime.HourProgress = 0f;
            currentTime.CurrentHour++;
            if (currentTime.CurrentHour == 24)
            {
                currentTime.CurrentHour = 0;
                currentTime.CurrentDay++;
            }
        }

        SystemAPI.SetSingleton(currentTime);
    }
}

public struct TimeData : IComponentData
{
    public float TimeMultiplier;
    public int CurrentHour;
    public float HourProgress;
    public int CurrentDay;
}
