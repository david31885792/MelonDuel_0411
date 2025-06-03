using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 하단 탭용 씬 이동 함수
    public void LoadScene(string sceneName)
    {
        Debug.Log($"📦 Loading Scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // "플레이" 버튼 클릭 시 호출 → MatchingScene으로 이동
    public void LoadMatchingScene()
    {
        Debug.Log("🔁 MatchingScene으로 이동");
        SceneManager.LoadScene("MatchingScene");
    }


    // MatchingScene 이후 GameScene으로 넘어갈 때 사용
    public static void LoadGameScene()
    {
        Debug.Log("🎲 Matching 완료 → GameScene 이동");
        SceneManager.LoadScene("GameScene");
    }

    // 게임 끝나고 MainScene으로 복귀할 때 사용
    public static void LoadMainScene()
    {
        Debug.Log("🏁 메인 화면으로 복귀");
        SceneManager.LoadScene("MainScene");
    }
}
