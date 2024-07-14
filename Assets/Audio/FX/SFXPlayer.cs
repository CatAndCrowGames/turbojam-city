using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    static SFXPlayer instance;

    [SerializeField] AudioSource policeAudioSource;
    [SerializeField] AudioSource rebelAudioSource;
    [SerializeField] float rebelCooldown;
    [SerializeField] float policeCooldown;

    float rebelTimer;
    float policeTimer;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        rebelTimer -= Time.deltaTime;
        policeTimer -= Time.deltaTime;
    }

    public static void PlayRebelSound()
    {
        if (instance.rebelTimer > 0) return;
        instance.rebelAudioSource.pitch = UnityEngine.Random.Range(.9f, 1.1f);
        instance.rebelAudioSource.Play();
        instance.rebelTimer = instance.rebelCooldown;
    }

    public static void PlayPoliceSound()
    {
        if (instance.policeTimer > 0) return;
        instance.policeAudioSource.Play();
        instance.policeTimer = instance.policeCooldown;
    }
}
