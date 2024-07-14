using UnityEngine;

public class RotateAroundAxis : MonoBehaviour
{
    [SerializeField] Vector3 eulers;

    new Transform transform;

    void Awake() => transform = GetComponent<Transform>();
    void Update() => transform.Rotate(eulers * Time.deltaTime);
}
