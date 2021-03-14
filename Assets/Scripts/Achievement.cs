public static class Achievement
{
    public static void Report(int score)
    {
        foreach (var id in TitleConstData.PlayCountAchievementIds)
        {
            SocialAchievementUtility.IncrementProgress(id);
        }

        foreach (var item in TitleConstData.ScoreAchievementsMap)
        {
            if (score >= item.Value)
            {
                SocialAchievementUtility.ReportProgress(item.Key);
            }
        }
    }
}
