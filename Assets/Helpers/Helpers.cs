using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    public static T RandomFromEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }

    public static T RandomFromList<T>(List<T> list)
    {
        T V = list[UnityEngine.Random.Range(0, list.Count)];
        return V;
    }

    public static T RandomFromHashSet<T>(HashSet<T> hashSet)
    {
        List<T> list = hashSet.ToList();
        T V = list[UnityEngine.Random.Range(0, hashSet.Count)];
        return V;
    }

    public static T RandomFromArray<T>(T[] array)
    {
        T V = array[UnityEngine.Random.Range(0, array.Length)];
        return V;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, digits);
        return Mathf.Round(value * mult) / mult;
    }


    public static Vector3 RoundVector3(Vector3 vector, int digits)
    {
        vector.x = Round(vector.x, digits);
        vector.y = Round(vector.y, digits);
        vector.z = Round(vector.z, digits);

        return vector;
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Color RandomColor()
    {
        float r = UnityEngine.Random.Range(0.2f, .8f);
        float g = UnityEngine.Random.Range(0.2f, .8f);
        float b = UnityEngine.Random.Range(0.2f, .8f);

        return new Color(r, g, b, 1);
    }

    public static Vector2 ScreenToRectPos(Canvas canvas, RectTransform rectTransform, Vector2 screen_pos)
    {
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null)
        {
            //Canvas is in Camera mode
            Vector2 anchorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screen_pos, canvas.worldCamera, out anchorPos);
            return anchorPos;
        }
        else
        {
            //Canvas is in Overlay mode
            Vector2 anchorPos = screen_pos - new Vector2(rectTransform.position.x, rectTransform.position.y);
            anchorPos = new Vector2(anchorPos.x / rectTransform.lossyScale.x, anchorPos.y / rectTransform.lossyScale.y);
            return anchorPos;
        }
    }

    public static Rect RectTransformToWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 topLeft = corners[0];
        Vector2 scaledSize = new Vector2(rectTransform.rect.size.x, rectTransform.rect.size.y);
        return new Rect(topLeft, scaledSize);
    }

    public static Vector2 RectTransformWorldCenter(RectTransform rectTransform)
    {
        return RectTransformToWorldRect(rectTransform).center;
    }

    public static Vector2Int[] SortPointsByDistanceFrom(Vector2Int origin, Vector2Int[] points)
    {
        Array.Sort(points, CompareByDistanceToOrigin);
        return points;

        int CompareByDistanceToOrigin(Vector2Int a, Vector2Int b)
        {
            float da = (a - origin).sqrMagnitude;
            float db = (b - origin).sqrMagnitude;

            if (da < db)
                return -1;
            else if (db < da)
                return 1;
            return 0;
        }
    }

    public static bool CoinFlip() => UnityEngine.Random.Range(0, 2) == 0;
    public static bool PercentChance(int chance) => UnityEngine.Random.Range(0, 100) < chance;

    public static void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
            if (child != parent)
                DestroyObject(child.gameObject);
    }

    static void DestroyObject(GameObject gameObject)
    {
        if (Application.isPlaying) GameObject.Destroy(gameObject);
        else GameObject.DestroyImmediate(gameObject);
    }
}

