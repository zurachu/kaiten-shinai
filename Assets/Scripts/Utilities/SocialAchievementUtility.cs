using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public static class SocialAchievementUtility
{
    public static void ReportProgress(string achievementId, double progress = 100.0)
    {
        if (!Social.localUser.authenticated)
        {
            return;
        }

        Social.ReportProgress(achievementId, progress, (_succeeded) =>
        {
            if (_succeeded)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.SetGravityForPopups(Gravity.TOP);
#endif
            }
        });
    }

    public static void IncrementProgress(string achievementId, int steps = 1)
    {
        if (!Social.localUser.authenticated)
        {
            return;
        }

#if UNITY_ANDROID
        PlayGamesPlatform.Instance.IncrementAchievement(achievementId, steps, (_succeeded) =>
        {
            if (_succeeded)
            {
                PlayGamesPlatform.Instance.SetGravityForPopups(Gravity.TOP);
            }
        });
#endif
    }
}
