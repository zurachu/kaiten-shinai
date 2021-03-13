using System.Collections.Generic;

public static class TitleConstData
{
    private static Dictionary<string, string> Source => PlayFabLoginManagerService.Instance.TitleData;

    public static string LeaderboardStatisticName => StringDictionaryUtility.GetString(Source, "LeaderboardStatisticName");
    public static int LeaderboardEntryCount => StringDictionaryUtility.GetInt(Source, "LeaderboardEntryCount");
    public static string UnityRoomGameId => StringDictionaryUtility.GetString(Source, "UnityRoomGameId");
    public static string GooglePlayStoreUrl => StringDictionaryUtility.GetString(Source, "GooglePlayStoreUrl");
}
