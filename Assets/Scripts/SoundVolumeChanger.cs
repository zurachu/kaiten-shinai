using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;

public class SoundVolumeChanger : MonoBehaviour
{
    [SerializeField] Slider slider;

    private static readonly string PrefKey = "SoundVolume";

    private float previousPlayTestVolume;

    private void Start()
    {
        var volume = PlayerPrefs.GetFloat(PrefKey, 1f);
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = volume;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        ChangeBaseVolume(volume);
        previousPlayTestVolume = volume;
    }

    private void ChangeBaseVolume(float volume)
    {
        BGMManager.Instance.ChangeBaseVolume(volume);
        SEManager.Instance.ChangeBaseVolume(volume);
    }

    private void OnSliderValueChanged(float value)
    {
        PlayerPrefs.SetFloat(PrefKey, value);
        ChangeBaseVolume(value);

        if (Mathf.Abs(value - previousPlayTestVolume) > 0.1f)
        {
            CommonAudioPlayer.PlayButtonClick();
            previousPlayTestVolume = value;
        }
    }
}
