using System;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;

public class PlayFabLeaderboardEntryItem : MonoBehaviour
{
    [SerializeField] private Image baseImage;
    [SerializeField] private Text rankText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Button changeNameButton;
    [SerializeField] private string scoreFormat;
    [SerializeField] private Color highlightedBaseColor;
    [SerializeField] private Color minusScoreColor;

    public bool IsMyself => entry != null && entry.PlayFabId == PlayFabLoginManagerService.Instance.PlayFabId;

    private PlayerLeaderboardEntry entry;
    private Action<PlayerLeaderboardEntry> onClickChangeName;

    public void Initialize(PlayerLeaderboardEntry entry, bool isHighlighted, Action<PlayerLeaderboardEntry> onClickChangeName)
    {
        this.entry = entry;
        this.onClickChangeName = onClickChangeName;

        UIUtility.TrySetText(rankText, $"{entry.Position + 1}");
        UIUtility.TrySetText(nameText, entry.DisplayName);
        UIUtility.TrySetText(scoreText, string.Format(scoreFormat, entry.StatValue));
        if (entry.StatValue < 0)
        {
            UIUtility.TrySetColor(scoreText, minusScoreColor);
        }

        if (isHighlighted)
        {
            UIUtility.TrySetColor(baseImage, highlightedBaseColor);
        }

        UIUtility.TrySetActive(changeNameButton, IsMyself);
    }

    public void OnClickInputName()
    {
        if (IsMyself)
        {
            CommonAudioPlayer.PlayButtonClick();

            onClickChangeName?.Invoke(entry);
        }
    }

    public void ChangeMyName(string newName)
    {
        if (IsMyself)
        {
            UIUtility.TrySetText(nameText, newName);
        }
    }
}
