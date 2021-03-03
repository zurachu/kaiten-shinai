using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLeaderboardUtility
{
    public static UniTask<UpdatePlayerStatisticsResult> UpdatePlayerStatisticAsync(string statisticName, int value)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>() {
                new StatisticUpdate {
                    StatisticName = statisticName,
                    Value = value
                }
            }
        };

        var source = new UniTaskCompletionSource<UpdatePlayerStatisticsResult>();
        Action<UpdatePlayerStatisticsResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.UpdatePlayerStatistics(request, resultCallback, errorCallback);
        return source.Task;
    }

    public static async UniTask<UpdatePlayerStatisticsResult> UpdatePlayerStatisticWithRetry(string statisticName, int value, int retryMs)
    {
        while (true)
        {
            try
            {
                return await UpdatePlayerStatisticAsync(statisticName, value);
            }
            catch (Exception)
            {
                await UniTask.Delay(retryMs);
            }
        }
    }

    public static UniTask<List<PlayerLeaderboardEntry>> GetLeaderboardAsync(string statisticName, int maxResultsCount)
    {
        var request = new GetLeaderboardRequest
        {
            MaxResultsCount = maxResultsCount,
            StatisticName = statisticName,
        };

        var source = new UniTaskCompletionSource<List<PlayerLeaderboardEntry>>();
        Action<GetLeaderboardResult> resultCallback = (_result) =>
        {
            Log(_result);
            source.TrySetResult(_result.Leaderboard);
        };
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.GetLeaderboard(request, resultCallback, errorCallback);
        return source.Task;
    }

    public static async UniTask<List<PlayerLeaderboardEntry>> GetLeaderboardWithRetry(string statisticName, int maxResultsCount, int retryMs)
    {
        while (true)
        {
            try
            {
                return await GetLeaderboardAsync(statisticName, maxResultsCount);
            }
            catch (Exception)
            {
                await UniTask.Delay(retryMs);
            }
        }
    }

    public static UniTask<UpdateUserTitleDisplayNameResult> UpdateUserTitleDisplayNameAsync(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        var source = new UniTaskCompletionSource<UpdateUserTitleDisplayNameResult>();
        Action<UpdateUserTitleDisplayNameResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, resultCallback, errorCallback);
        return source.Task;
    }

    public static async UniTask<UpdateUserTitleDisplayNameResult> UpdateUserTitleDisplayNameWithRetry(string newName, int retryMs)
    {
        while (true)
        {
            try
            {
                return await UpdateUserTitleDisplayNameAsync(newName);
            }
            catch (Exception)
            {
                await UniTask.Delay(1000);
            }
        }
    }

    private static void Log(GetLeaderboardResult result)
    {
        var stringBuilder = new StringBuilder();
        foreach (var entry in result.Leaderboard)
        {
            stringBuilder.AppendFormat("{0}:{1}:{2}:{3}\n", entry.Position, entry.StatValue, entry.PlayFabId, entry.DisplayName);
        }

        Debug.Log(stringBuilder);
    }
}
