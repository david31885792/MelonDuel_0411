using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchingSceneManager : MonoBehaviour
{
    [SerializeField] private float matchingDelay = 3f; // 대기 시간

    private void Start()
    {
        Debug.Log("⏳ 매칭 중...");
        Invoke(nameof(LoadGameScene), matchingDelay);
    }

    private void LoadGameScene()
    {
        Debug.Log("🎮 매칭 완료 → GameScene 이동");
        SceneManager.LoadScene("GameScene");
    }
}
