using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.mainBGM);
            Debug.Log("🎵 MainScene BGM 재생 시작");
        }
        else
        {
            Debug.LogWarning("❌ AudioManager.Instance 가 존재하지 않음");
        }
    }
}
