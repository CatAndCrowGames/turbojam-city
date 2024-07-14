using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class HoverDisplay : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] Texture2D residencyImage;
    [SerializeField] Texture2D officesImage;
    [SerializeField] Texture2D nightOfficesImage;
    [SerializeField] Texture2D policeImage;
    [SerializeField] Texture2D firewatchImage;

    Entity boundaryEntity;
    
    VisualElement image;
    VisualElement root;
    Label label;
    EntityQuery boundaryQuery;
    EntityManager em;

    MouseHoverType shownType;

    void Awake()
    {
        root = uiDocument.rootVisualElement;
        image = root.Query<VisualElement>("Image");
        label = root.Query<Label>();
    }

    public void Hide()
    {
        if (!gameObject.activeInHierarchy) return;

        root.style.display = DisplayStyle.None;
        shownType = MouseHoverType.none;
    }

    public void Show(MouseHoverType type)
    {
        if (!gameObject.activeInHierarchy) return;

        if (shownType == type) return;
        if (type == MouseHoverType.none)
        root.style.display = DisplayStyle.Flex;
        switch (type)
        {
            case MouseHoverType.none:
                Hide();
                return;

            case MouseHoverType.residency:
                image.style.backgroundImage = residencyImage;
                label.text = "Residencies. Everyone living here is a worker.";
                break;
            case MouseHoverType.firewatch:
                image.style.backgroundImage = firewatchImage;
                label.text = "A firewatch. I respect them.";
                break;
            case MouseHoverType.offices:
                image.style.backgroundImage = officesImage;
                label.text = "Offices. The bureaucracy keeps everyone blind and busy.";
                break;
            case MouseHoverType.nightoffices:
                image.style.backgroundImage = nightOfficesImage;
                label.text = "Night offices. Someone needs to keep the barrier functional, right?";
                break;
            case MouseHoverType.policeStation:
                image.style.backgroundImage = policeImage;
                label.text = "A police station. Better not get too bold around here.";
                break;
        }
    }
}