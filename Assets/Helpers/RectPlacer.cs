using UnityEngine;

enum AtScreenEdge
{
    nothing,
    flipPivot,
    clamp
}

public class RectPlacer : MonoBehaviour
{
    RectTransform rect;
    Canvas rootCanvas;

    [SerializeField] AtScreenEdge atScreenEdge;
    [SerializeField] bool keepAtMouse;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    void Update()
    {
        if (keepAtMouse) PlaceAtMouse();
    }


    public void PlaceAt(Vector2 screenPosition)
    {
        if (atScreenEdge == AtScreenEdge.flipPivot)
            SetPivot(screenPosition);
        if (atScreenEdge == AtScreenEdge.clamp)
        {
            screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width - Width());
            screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height - Height());
        }

        rect.position = screenPosition;
    }

    void SetPivot(Vector2 screenPosition)
    {
        Vector2 pivot = Vector2.zero;
        if (screenPosition.x > Screen.width - Width()) pivot.x = 1;
        if (screenPosition.y > Height()) pivot.y = 1;
        rect.pivot = pivot;
    }

    float Width() => rect.sizeDelta.x * rootCanvas.scaleFactor;
    float Height() => rect.sizeDelta.y * rootCanvas.scaleFactor;

    public void PlaceAtWorld(Camera camera, Vector3 worldPosition) => PlaceAt(camera.WorldToScreenPoint(worldPosition));
    public void PlaceAtMouse()
    {
        PlaceAt(Input.mousePosition);
    }
}
