using System.Collections.Generic;
using PlayFab.Json;

public static class TitleConstData
{
    private static Dictionary<string, string> Source => PlayFabLoginManagerService.Instance.TitleData;

    public static string PrivacyPolicyUrl => StringDictionaryUtility.GetString(Source, "PrivacyPolicyUrl");
    public static string LeaderboardStatisticName => StringDictionaryUtility.GetString(Source, "LeaderboardStatisticName");
    public static int LeaderboardEntryCount => StringDictionaryUtility.GetInt(Source, "LeaderboardEntryCount");
    public static string UnityRoomGameId => StringDictionaryUtility.GetString(Source, "UnityRoomGameId");
    public static string GooglePlayStoreUrl => StringDictionaryUtility.GetString(Source, "GooglePlayStoreUrl");

    public static List<string> PlayCountAchievementIds =>
        PlayFabSimpleJson.DeserializeObject<List<string>>(Source["PlayCountAchievementIds"]);
    public static Dictionary<string, int> ScoreAchievementsMap =>
        PlayFabSimpleJson.DeserializeObject<Dictionary<string, int>>(Source["ScoreAchievementsMap"]);
}
