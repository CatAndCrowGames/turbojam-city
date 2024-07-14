using UnityEngine;
using UnityEngine.UIElements;

public class ConnectorLine : VisualElement
{
    VisualElement startElement;
    VisualElement endElement;

    public ConnectorLine(VisualElement start, VisualElement end)
    {
        startElement = start;
        endElement = end;

        // Set the style of the line
        style.position = Position.Absolute;
        style.backgroundColor = Color.black;
        style.height = 1;
        style.width = 1;
        style.borderRightWidth = 1;
        style.borderRightColor = Color.black;
    }

    public void UpdateLine()
    {
        // Get the start and end positions of the line
        Vector2 startPos = startElement.worldBound.center;
        Vector2 endPos = endElement.worldBound.center;

        // Calculate the length and angle of the line
        float length = Vector2.Distance(startPos, endPos);
        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;

        // Set the position, length, and angle of the line
        style.left = startPos.x;
        style.top = startPos.y;
        style.width = length;
        //  style.rotation = angle;
    }
}

