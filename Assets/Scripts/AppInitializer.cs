using UnityEngine;

public class AppInitializer : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        DontDestroyOnLoad(gameObject);
    }
}
