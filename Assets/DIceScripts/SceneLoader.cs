using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        Debug.Log($"📦 [SceneLoader] 씬 로드 요청: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadMainScene()
    {
        Debug.Log("🔁 [SceneLoader] MainScene 로드");
        SceneManager.LoadScene("MainScene");
    }

    public static void LoadDiceScene()
    {
        Debug.Log("🎲 [SceneLoader] DiceScene 로드");
        SceneManager.LoadScene("DiceScene");
    }

    public static void LoadGameScene()
    {
        Debug.Log("🎮 [SceneLoader] GameScene 로드");
        SceneManager.LoadScene("GameScene");
    }

    public static void QuitGame()
    {
        Debug.Log("❌ [SceneLoader] QuitGame 호출됨");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
