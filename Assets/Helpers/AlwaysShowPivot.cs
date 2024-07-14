using UnityEngine;

public class AlwaysShowPivot : MonoBehaviour
{
    [SerializeField] Color pivotColor = Color.magenta;
    [SerializeField] float pivotRadius =.5f;
    [SerializeField] bool showOnlyWhenSelected;

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        if (showOnlyWhenSelected) return;
        Gizmos.color = pivotColor;
        Gizmos.DrawWireSphere(transform.position, pivotRadius);
    }

    void OnDrawGizmosSelected()
    {
        if (!showOnlyWhenSelected) return;
        Gizmos.color = pivotColor;
        Gizmos.DrawWireSphere(transform.position, pivotRadius);
    }
#endif
}
