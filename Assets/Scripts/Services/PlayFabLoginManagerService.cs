using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using GooglePlayGames;

public class PlayFabLoginManagerService
{
    private static PlayFabLoginManagerService instance;

    public static PlayFabLoginManagerService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayFabLoginManagerService();
            }

            return instance;
        }
    }

    public string PlayFabId => result.PlayFabId;
    public bool LoggedIn => result != null;

    private static readonly string GuidKey = "Guid";

    private LoginResult result;

    public async UniTask<LoginResult> TryLoginAsync()
    {
        // Inspector で設定
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            throw new Exception("PlayFabSettings.TitleId is not set");
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        if (!GooglePlayGameLoginManagerService.Instance.LoggedIn)
        {
            GooglePlayGameLoginManagerService.Instance.Initialize();
            await GooglePlayGameLoginManagerService.Instance.TryLoginAsync();
        }

        if (GooglePlayGameLoginManagerService.Instance.LoggedIn)
        {
            result = await TryLoginWithGoogleAccountAsync();
        }
        else
        {
            result = await TryLoginWithAndroidDeviceIdAsync();
        }
#else
        result = await TryLoginDefaultAsync();
#endif
        return result;
    }

    public async UniTask<LoginResult> LoginAsyncWithRetry(int retryMs)
    {
        while (true)
        {
            try
            {
                return await TryLoginAsync();
            }
            catch (Exception)
            {
                await UniTask.Delay(retryMs);
            }
        }
    }

    private UniTask<LoginResult> TryLoginWithGoogleAccountAsync()
    {
        var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();

        Debug.Log("Server Auth Code: " + serverAuthCode);

        var request = new LoginWithGoogleAccountRequest
        {
            ServerAuthCode = serverAuthCode,
            CreateAccount = true
        };

        var source = new UniTaskCompletionSource<LoginResult>();
        Action<LoginResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.LoginWithGoogleAccount(request, resultCallback, errorCallback);
        return source.Task;
    }

    private UniTask<LoginResult> TryLoginWithAndroidDeviceIdAsync()
    {
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        var secure = new AndroidJavaClass("android.provider.Settings$Secure");
        var androidId = secure.CallStatic<string>("getString", contentResolver, "android_id");

        Debug.Log(androidId);

        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = androidId,
            CreateAccount = true
        };

        var source = new UniTaskCompletionSource<LoginResult>();
        Action<LoginResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, resultCallback, errorCallback);
        return source.Task;
    }

    private UniTask<LoginResult> TryLoginDefaultAsync()
    {
        // WebGL では端末 ID 的なものがなく、スコアランキング程度で Facebook 等連携してもらうのもユーザに手間をかけるので、
        // 簡易な端末 ID もどきとして。
        var guid = PlayerPrefs.GetString(GuidKey);
        if (string.IsNullOrEmpty(guid))
        {
            guid = Guid.NewGuid().ToString("D");
            PlayerPrefs.SetString(GuidKey, guid);
            PlayerPrefs.Save();
        }

        Debug.Log(guid);

        var request = new LoginWithCustomIDRequest
        {
            CustomId = guid,
            CreateAccount = true
        };

        var source = new UniTaskCompletionSource<LoginResult>();
        Action<LoginResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.LoginWithCustomID(request, resultCallback, errorCallback);
        return source.Task;
    }
}
