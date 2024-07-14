using UnityEngine;

public enum GizmoDrawSetting
{
    never,
    selected,
    always
}

public class PositionGizmo : MonoBehaviour
{
    [SerializeField] Color color = Color.magenta;
    [SerializeField] GizmoDrawSetting draw = GizmoDrawSetting.selected;
    [SerializeField] float sphereSize = .5f;
    [SerializeField] float lineLength = 1f;

    void OnDrawGizmos()
    {
        if (draw == GizmoDrawSetting.always) Draw();
    }

    void OnDrawGizmosSelected()
    {
        if (draw == GizmoDrawSetting.selected) Draw();
    }

    void Draw()
    {
        Gizmos.color = color;
        Transform thisTransform = transform;
        Vector3 position = thisTransform.position;
        Vector3 forward = thisTransform.forward;

        Gizmos.DrawSphere(position, sphereSize);
        Gizmos.DrawLine(position, position + forward * lineLength);
    }
}
