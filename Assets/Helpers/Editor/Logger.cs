using UnityEngine;

public static class Logger
{
    public static void Log(string text)
    {
#if (UNITY_EDITOR)
        Debug.Log(text);
#endif
    }
}
