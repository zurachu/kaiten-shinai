using UnityEngine;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class GooglePlayGameLoginManagerService
{
    private static GooglePlayGameLoginManagerService instance;

    public static GooglePlayGameLoginManagerService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GooglePlayGameLoginManagerService();
            }

            return instance;
        }
    }

    public bool LoggedIn => Social.localUser.authenticated;

    public void Initialize()
    {
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false)
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
#if UNITY_EDITOR
        PlayGamesPlatform.DebugLogEnabled = true;
#endif
        PlayGamesPlatform.Activate();
#endif
    }

    public UniTask<bool> TryLoginAsync()
    {
        var source = new UniTaskCompletionSource<bool>();
        Social.localUser.Authenticate((_result) => source.TrySetResult(_result));
        return source.Task;
    }
}
