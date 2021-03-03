using KanKikuchi.AudioManager;

public static class CommonAudioPlayer
{
    public static void PlayButtonClick()
    {
        SEManager.Instance.Play(SEPath.ENTER15);
    }

    public static void PlayCancel()
    {
        SEManager.Instance.Play(SEPath.ENTER15);
    }
}
