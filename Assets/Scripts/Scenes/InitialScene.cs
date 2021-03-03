using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{
    private async void Start()
    {
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
        await PlayFabLoginManagerService.Instance.LoginAsyncWithRetry(1000);
        SceneManager.LoadScene("SampleScene");
    }
}
