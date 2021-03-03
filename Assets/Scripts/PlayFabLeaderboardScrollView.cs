using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;

public class PlayFabLeaderboardScrollView : MonoBehaviour
{
    [SerializeField] private Text loadingText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private PlayFabLeaderboardEntryItem leaderboardEntryItemPrefab;
    [SerializeField] private NameEntryView nameEntryViewPrefab;

    private List<PlayFabLeaderboardEntryItem> leaderboardEntryItems;

    public async UniTask Initialize(string statisticName, int maxResultsCount, int yourScore)
    {
        UIUtility.TrySetActive(loadingText, true);
        var playerLeaderboardEntries = await PlayFabLeaderboardUtility.GetLeaderboardWithRetry(statisticName, maxResultsCount, 1000);
        UIUtility.TrySetActive(loadingText, false);
        SetupScrollView(playerLeaderboardEntries, yourScore);

        var myPlayerLeaderboardEntry = playerLeaderboardEntries.Find(_entry => _entry.PlayFabId == PlayFabLoginManagerService.Instance.PlayFabId);
        if (myPlayerLeaderboardEntry != null && string.IsNullOrEmpty(myPlayerLeaderboardEntry.DisplayName))
        {
            ShowNameEntryView(myPlayerLeaderboardEntry);
        }
    }

    private void SetupScrollView(List<PlayerLeaderboardEntry> playerLeaderboardEntries, int yourScore)
    {
        var content = scrollRect.content;
        leaderboardEntryItems = new List<PlayFabLeaderboardEntryItem>();

        foreach (var entry in playerLeaderboardEntries)
        {
            var isMySelf = entry.PlayFabId == PlayFabLoginManagerService.Instance.PlayFabId;
            var isHighlighted = isMySelf && entry.StatValue == yourScore;
            var entryItem = Instantiate(leaderboardEntryItemPrefab, content.transform);
            entryItem.Initialize(entry, isHighlighted, ShowNameEntryView);
            leaderboardEntryItems.Add(entryItem);
        }

        var myself = leaderboardEntryItems.Find(_item => _item.IsMyself);
        if (myself != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            var scrollRectHeight = scrollRect.GetComponent<RectTransform>().rect.height;
            var y = -myself.transform.localPosition.y - scrollRectHeight / 2;
            y = Mathf.Max(Mathf.Min(y, content.rect.height - scrollRectHeight), 0);
            scrollRect.content.localPosition = new Vector3(0, y, 0);
        }
    }

    private void ShowNameEntryView(PlayerLeaderboardEntry myPlayerLeaderboardEntry)
    {
        var nameEntryView = Instantiate(nameEntryViewPrefab, transform);
        nameEntryView.Initialize(myPlayerLeaderboardEntry.DisplayName,
            (_newName) =>
            {
                _ = PlayFabLeaderboardUtility.UpdateUserTitleDisplayNameWithRetry(_newName, 1000);
                myPlayerLeaderboardEntry.DisplayName = _newName;
                leaderboardEntryItems.ForEach(_item => _item.ChangeMyName(_newName));
                Destroy(nameEntryView.gameObject);
            },
            () =>
            {
                Destroy(nameEntryView.gameObject);
            });
    }
}
