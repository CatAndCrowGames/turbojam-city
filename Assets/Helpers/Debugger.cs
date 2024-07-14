using UnityEngine;

public static class Debugger
{
    public static void Log(string message) => Log(message, null);
    public static void Log(string message, Object context)
    {
#if (UNITY_EDITOR)
        Debug.Log(" --- " + message + " --- ", context);
#endif
    }
    public static void Warn(string message) => Warn(message, null);
    public static void Warn(string message, Object context)
    {
#if (UNITY_EDITOR)
        Debug.LogWarning(" --- " + message + " --- ", context);
#endif
    }
    public static void Error(string message) => Error(message, null);
    public static void Error(string message, Object context)
    {
#if (UNITY_EDITOR)
        Debug.LogError(" --- " + message + " --- ", context);
#endif
    }
}
