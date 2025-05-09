using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [Header("패턴 표시")]
    [SerializeField] private PatternPanel patternPanel;

    [Header("보드 생성")]
    [SerializeField] private BoardManager boardManager;

    public static GameSceneManager Instance { get; private set; }


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
        //patternPanel.SetPattern(pattern);

        Debug.Log("✅ GameScene 초기화 완료");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnGameClear()
    {
        Debug.Log("🎉 YOU WIN! (임시 OnGameClear 실행됨)");
        // 여기에 승리 연출이나 씬 전환 처리 예정
    }

}
