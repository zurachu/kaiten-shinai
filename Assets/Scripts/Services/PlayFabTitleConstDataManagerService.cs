using System;
using System.ComponentModel;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayFabTitleConstDataManagerService
{
    private static PlayFabTitleConstDataManagerService instance;

    public static PlayFabTitleConstDataManagerService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayFabTitleConstDataManagerService();
            }

            return instance;
        }
    }

    private GetTitleDataResult result;

    public UniTask<GetTitleDataResult> TryGetDataAsync()
    {
        var source = new UniTaskCompletionSource<GetTitleDataResult>();
        Action<GetTitleDataResult> resultCallback = (_result) =>
        {
            result = _result;
            Debug.Log(Dump());
            source.TrySetResult(_result);
        };
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), resultCallback, errorCallback);
        return source.Task;
    }

    public async UniTask<GetTitleDataResult> GetDataAsyncWithRetry(int retryMs)
    {
        while (true)
        {
            try
            {
                return await TryGetDataAsync();
            }
            catch (Exception)
            {
                await UniTask.Delay(retryMs);
            }
        }
    }

    public int GetInt(string key)
    {
        return GetValue<int>(key);
    }

    public float GetFloat(string key)
    {
        return GetValue<float>(key);
    }

    public string GetString(string key)
    {
        return GetValue<string>(key);
    }

    private T GetValue<T>(string key)
    {
        if (result?.Data != null)
        {
            if (result.Data.TryGetValue(key, out var value))
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    return (T)converter.ConvertFromString(value);
                }
            }
        }

        return default;
    }

    private string Dump()
    {
        if (result?.Data == null)
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();
        foreach (var item in result.Data)
        {
            stringBuilder.Append($"{item.Key}:{item.Value}\n");
        }

        return stringBuilder.ToString();
    }
}
