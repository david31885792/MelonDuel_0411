using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [Header("패턴 표시")]
    [SerializeField] private PatternPanel patternPanel;

    [Header("보드 생성")]
    [SerializeField] private BoardManager boardManager;

    private void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        // ClearPattern 불러오기
        string[,] pattern = PlayerPrefsPatternLoader.LoadPattern();

        if (pattern == null)
        {
            Debug.LogError("❌ ClearPattern 불러오기 실패");
            return;
        }

        // 패턴 패널에 표시
        patternPanel.SetPattern(pattern);

        Debug.Log("✅ GameScene 초기화 완료");
    }
}
