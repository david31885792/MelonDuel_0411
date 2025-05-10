using UnityEngine;
using TMPro;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [SerializeField] private PatternPanel patternPanel;
    [SerializeField] private GameObject youWinPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Tile[] GetPatternTiles()
    {
        return patternPanel.GetPatternTiles();
    }

    // ✅ 게임 클리어 시 호출되는 함수
    public void OnGameClear()
    {
        Debug.Log("🎉 YOU WIN! (게임 클리어 처리)");

        if (youWinPanel == null)
        {
            Debug.LogError("❌ youWinPanel 오브젝트가 에디터에 연결되지 않았습니다!");
            return;
        }

        youWinPanel.SetActive(true);
        Debug.Log("✅ youWinPanel.SetActive(true) 호출됨");

        StartCoroutine(LoadMainSceneAfterDelay(3f));
    }



    private IEnumerator LoadMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneLoader.LoadMainScene(); // ✅ SceneLoader.cs를 통한 씬 전환
    }
}
