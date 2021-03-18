using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KanKikuchi.AudioManager;

public class SampleScene : MonoBehaviour
{
    [SerializeField] Transform rotator;
    [SerializeField] UnityChanController unityChanController;
    [SerializeField] Text scoreText;
    [SerializeField] Text speedText;
    [SerializeField] CanvasGroup titleCanvasGroup;
    [SerializeField] CanvasGroup gameOverCanvasGroup;
    [SerializeField] CanvasGroup gameOverButtonCanvasGroup;
    [SerializeField] PlayFabLeaderboardScrollView leaderboardScrollView;
    [SerializeField] LicenseView licenseViewPrefab;
    [SerializeField] GameObject loadingViewPrefab;
    [SerializeField] Button achivementButton;

    private bool isStarted;
    private int score;
    private float speed;
    private DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> speedTweener;

    private async void Start()
    {
        isStarted = false;

        SetScore(0);
        UIUtility.TrySetActive(scoreText, false);
        UIUtility.TrySetActive(speedText, false);
        UIUtility.TrySetActive(titleCanvasGroup.gameObject, true);
        UIUtility.TrySetActive(gameOverCanvasGroup.gameObject, false);
        UIUtility.TrySetActive(achivementButton, false);

        await Login();
        UIUtility.TrySetActive(achivementButton, Social.localUser.authenticated);

        await UniTask.WaitUntil(() => isStarted);
        UIUtility.TrySetActive(scoreText, true);
        UIUtility.TrySetActive(titleCanvasGroup.gameObject, false);
        unityChanController.IsStarted = true;

        SEManager.Instance.Play(SEPath.WHISTLE);
        speedTweener = DOTween.To(() => speed, (_value) => speed = _value, 1440, 120).From(180).SetEase(Ease.Linear);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            UIUtility.TrySetActive(speedText, !speedText.IsActive());
        }

        UIUtility.TrySetText(speedText, $"{(int)speed} deg/s");

        if (!isStarted)
        {
            return;
        }

        var eulerAngles = rotator.eulerAngles;
        var normalized = Mathf.Repeat(eulerAngles.y + 180, 360) - 180;
        eulerAngles.y = normalized + Time.deltaTime * speed;
        rotator.eulerAngles = eulerAngles;

        if (normalized < 0 && 0 <= eulerAngles.y)
        {
            if (unityChanController.IsJumping)
            {
                SetScore(score + 1);
                SEManager.Instance.Play(SEPath.KIN);
            }
            else
            {
                if (!unityChanController.IsDown)
                {
                    GameOver();
                }

                if (!gameOverCanvasGroup.isActiveAndEnabled)
                {
                    unityChanController.Down();
                    SEManager.Instance.Play(SEPath.BASI);
                }
            }
        }
    }

    private void SetScore(int value)
    {
        score = value;
        UIUtility.TrySetText(scoreText, $"{score}");
    }

    private async void GameOver()
    {
        speedTweener.Kill();
        var statisticName = TitleConstData.LeaderboardStatisticName;
        await PlayFabLeaderboardUtility.UpdatePlayerStatisticAsync(statisticName, score);
        await UniTask.Delay(2000);
        Achievement.Report(score);
        UIUtility.TrySetActive(gameOverCanvasGroup.gameObject, true);
        gameOverButtonCanvasGroup.interactable = false;
        await leaderboardScrollView.Initialize(statisticName, TitleConstData.LeaderboardEntryCount, score);
        gameOverButtonCanvasGroup.interactable = true;
    }

    public void OnClickStart()
    {
        isStarted = true;
    }

    public void OnClickLicense()
    {
        CommonAudioPlayer.PlayButtonClick();

        Instantiate(licenseViewPrefab, titleCanvasGroup.transform);
    }

    public void OnClickPrivacyPolicy()
    {
        CommonAudioPlayer.PlayButtonClick();

        WebUtility.OpenURL(TitleConstData.PrivacyPolicyUrl);
    }

    public void OnClickAchievement()
    {
        CommonAudioPlayer.PlayButtonClick();

        Social.ShowAchievementsUI();
    }

    public void OnClickTweet()
    {
        CommonAudioPlayer.PlayButtonClick();

        var message = $"ユニティちゃん体当たりチャレンジ☆回転竹刀を{score}回避けたよ！";
#if UNITY_WEBGL
        naichilab.UnityRoomTweet.Tweet(TitleConstData.UnityRoomGameId, message, "unityroom", "unity1week");
#else
        TweetWithoutUnityRoom(message);
#endif
    }

    public void OnClickRetry()
    {
        CommonAudioPlayer.PlayButtonClick();

        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private async UniTask Login()
    {
        if (PlayFabLoginManagerService.Instance.LoggedIn)
        {
            return;
        }

        var loadingView = Instantiate(loadingViewPrefab, titleCanvasGroup.transform);
#if !UNITY_EDITOR && UNITY_ANDROID
        GooglePlayGameLoginManagerService.Instance.Initialize();
        await GooglePlayGameLoginManagerService.Instance.TryLoginAsync();
#endif
        await PlayFabLoginManagerService.Instance.LoginAsyncWithRetry(1000);

        Destroy(loadingView.gameObject);
    }

    private void TweetWithoutUnityRoom(string message)
    {
        var messageWithGooglePlayStoreUrl = $"{message}\n{TitleConstData.GooglePlayStoreUrl}";
        Application.OpenURL("http://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(messageWithGooglePlayStoreUrl));
    }
}
