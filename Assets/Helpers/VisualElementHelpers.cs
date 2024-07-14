using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementHelpers
{
    public static void SetBorder(VisualElement visualElement, float borderWidth, Color borderColor)
    {
        visualElement.style.borderRightWidth = borderWidth;
        visualElement.style.borderLeftWidth = borderWidth;
        visualElement.style.borderTopWidth = borderWidth;
        visualElement.style.borderBottomWidth = borderWidth;
        visualElement.style.borderBottomColor = borderColor;
        visualElement.style.borderTopColor = borderColor;
        visualElement.style.borderLeftColor = borderColor;
        visualElement.style.borderRightColor = borderColor;
    }

    public static void SetPadding(VisualElement visualElement, float paddingWidth)
    {
        visualElement.style.paddingRight = paddingWidth;
        visualElement.style.paddingLeft = paddingWidth;
        visualElement.style.paddingTop = paddingWidth;
        visualElement.style.paddingBottom = paddingWidth;
    }

    public static void SetMargin(VisualElement visualElement, float marginWidth)
    {
        visualElement.style.marginRight = marginWidth;
        visualElement.style.marginLeft = marginWidth;
        visualElement.style.marginTop = marginWidth;
        visualElement.style.marginBottom = marginWidth;
    }
}
