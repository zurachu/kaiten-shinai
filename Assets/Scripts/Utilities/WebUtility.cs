using System.Runtime.InteropServices;
using UnityEngine;

public static class WebUtility
{
#if UNITY_WEBGL
    // naichilab.UnityRoomTweet
    [DllImport("__Internal")]
    private static extern void OpenWindow(string url);
#endif

    public static void OpenURL(string url)
    {
#if UNITY_WEBGL
        OpenWindow(url);
#else
        Application.OpenURL(url);
#endif
    }
}
