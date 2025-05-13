using UnityEngine;

public class AudioManagerBootstrap : MonoBehaviour
{
    [Header("AudioManager 프리팹")]
    public GameObject audioManagerPrefab;

    private void Awake()
    {
        if (AudioManager.Instance == null && audioManagerPrefab != null)
        {
            Instantiate(audioManagerPrefab);
            Debug.Log("✅ AudioManager 자동 생성됨 (Bootstrap)");
        }
    }
}
