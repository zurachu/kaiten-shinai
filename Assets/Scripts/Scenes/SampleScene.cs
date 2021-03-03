using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KanKikuchi.AudioManager;

public class SampleScene : MonoBehaviour
{
    [SerializeField] Transform rotator;
    [SerializeField] UnityChanController unityChanController;
    [SerializeField] Text scoreText;
    [SerializeField] CanvasGroup titleCanvasGroup;
    [SerializeField] CanvasGroup gameOverCanvasGroup;
    [SerializeField] CanvasGroup gameOverButtonCanvasGroup;
    [SerializeField] PlayFabLeaderboardScrollView leaderboardScrollView;

    private static readonly string statisticName = "max";

    private bool isStarted;
    private int score;
    private float speed;

    private async void Start()
    {
        if (!PlayFabLoginManagerService.Instance.LoggedIn)
        {
            SceneManager.LoadScene("InitialScene");
            return;
        }

        isStarted = false;

        SetScore(0);
        UIUtility.TrySetActive(scoreText, false);
        UIUtility.TrySetActive(titleCanvasGroup.gameObject, true);
        UIUtility.TrySetActive(gameOverCanvasGroup.gameObject, false);

        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
        UIUtility.TrySetActive(scoreText, true);
        UIUtility.TrySetActive(titleCanvasGroup.gameObject, false);
        isStarted = true;

        SEManager.Instance.Play(SEPath.WHISTLE);
        DOTween.To(() => speed, (_value) => speed = _value, 1440, 120).From(180).SetEase(Ease.Linear);
    }

    private void Update()
    {
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
            if (!unityChanController.IsDown)
            {
                if (unityChanController.IsJumping)
                {
                    SetScore(score + 1);
                    SEManager.Instance.Play(SEPath.KIN);
                }
                else
                {
                    GameOver();
                }
            }

            if (!unityChanController.IsJumping && !gameOverCanvasGroup.isActiveAndEnabled)
            {
                SEManager.Instance.Play(SEPath.BASI);
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
        unityChanController.Down();

        await PlayFabLeaderboardUtility.UpdatePlayerStatisticAsync(statisticName, score);
        await UniTask.Delay(2000);
        UIUtility.TrySetActive(gameOverCanvasGroup.gameObject, true);
        gameOverButtonCanvasGroup.interactable = false;
        await leaderboardScrollView.Initialize(statisticName, 30, score);
        gameOverButtonCanvasGroup.interactable = true;
    }

    public void OnClickTweet()
    {
        CommonAudioPlayer.PlayButtonClick();

        var message = $"ユニティちゃん体当たりチャレンジ☆回転竹刀を{score}回避けたよ！";
        naichilab.UnityRoomTweet.Tweet("kaiten-shinai", message, "unityroom", "unity1week");
    }

    public void OnClickRetry()
    {
        CommonAudioPlayer.PlayButtonClick();

        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
