﻿using UnityEngine;
using Cysharp.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

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
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false)
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public UniTask<bool> TryLoginAsync()
    {
        var source = new UniTaskCompletionSource<bool>();
        Social.localUser.Authenticate((_result) => source.TrySetResult(_result));
        return source.Task;
    }
}