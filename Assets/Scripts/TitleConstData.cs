public static class TitleConstData
{
    private static PlayFabTitleConstDataManagerService Source => PlayFabTitleConstDataManagerService.Instance;

    public static string LeaderboardStatisticName => Source.GetString("LeaderboardStatisticName");
    public static int LeaderboardEntryCount => Source.GetInt("LeaderboardEntryCount");
    public static string UnityRoomGameId => Source.GetString("UnityRoomGameId");
    public static string GooglePlayStoreUrl => Source.GetString("GooglePlayStoreUrl");
}
