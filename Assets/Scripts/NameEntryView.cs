using System;
using UnityEngine;
using UnityEngine.UI;

public class NameEntryView : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button cancelButton;

    private static readonly int minimumLength = 3;

    private Action<string> onSubmit;
    private Action onCancel;

    public void Initialize(string originalName, Action<string> onSubmit, Action onCancel)
    {
        this.onSubmit = onSubmit;
        this.onCancel = onCancel;

        inputField.text = originalName;
        UIUtility.TrySetActive(cancelButton, !string.IsNullOrEmpty(originalName));
    }

    public void OnValueChanged(string text)
    {
        var lineFeedRemoved = text.Replace("\r", "").Replace("\n", "");
        inputField.text = lineFeedRemoved;
        submitButton.interactable = IsValidName(lineFeedRemoved);
    }

    public void OnClickSubmit()
    {
        var newName = inputField.text;
        if (!IsValidName(newName))
        {
            return;
        }

        CommonAudioPlayer.PlayButtonClick();

        onSubmit?.Invoke(newName);
    }

    public void OnClickCancel()
    {
        CommonAudioPlayer.PlayCancel();

        onCancel?.Invoke();
    }

    private bool IsValidName(string name)
    {
        return !string.IsNullOrEmpty(name) &&
            minimumLength <= name.Length && name.Length <= inputField.characterLimit &&
            !name.Contains("\r") && !name.Contains("\n");
    }
}
