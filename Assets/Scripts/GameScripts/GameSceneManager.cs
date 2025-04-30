using System.Collections;
using UnityEngine;
using TMPro;

public class GameSceneManager : MonoBehaviour
{
    [Header("연결된 패턴 UI 및 보드")]
    public PatternPanelUI patternPanelUI;
    public BoardManager boardManager;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI moveCountText;
    public GameObject winPanel;

    [Header("안내 텍스트")]
    public TextMeshProUGUI noticeText;

    private float elapsedTime = 0f;
    private int moveCount = 0;
    private bool gameRunning = false;
    [SerializeField]
    private string noticeMessage = "Returning to the main menu in 5 seconds...";


    void Start()
    {
        Debug.Log("▶ [GameSceneManager] 게임 시작");

        // 패턴 불러오기
        string[,] pattern = PlayerPrefsPatternLoader.LoadPattern();
        patternPanelUI.SetPattern(pattern);

        // 보드 초기화
        boardManager.OnTileMoved += OnTileMoved;
        boardManager.OnClearMatched += OnPlayerWin;
        boardManager.InitBoard();

        // UI 초기화
        winPanel.SetActive(false);
        noticeText.text = noticeMessage;
        UpdateMoveUI();
        UpdateTimerUI();

        gameRunning = true;

        // 10초 후 메인 씬으로 전환
        Invoke("ReturnToMainScene", 5f);
    }

    void Update()
    {
        if (!gameRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"Time: {elapsedTime:F1}s";
    }

    private void UpdateMoveUI()
    {
        moveCountText.text = $"Moves: {moveCount}";
    }

    private void OnTileMoved()
    {
        moveCount++;
        UpdateMoveUI();
    }

    private void OnPlayerWin()
    {
        gameRunning = false;
        winPanel.SetActive(true);
        Debug.Log($" 클리어! 걸린 시간: {elapsedTime:F2}s, 이동 수: {moveCount}");
    }

    private void ReturnToMainScene()
    {
        Debug.Log("🔁 [GameSceneManager] 5초 경과 - 메인 씬으로 이동");
        SceneLoader.LoadMainScene();
    }
}
