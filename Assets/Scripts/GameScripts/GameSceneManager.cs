using UnityEngine;
using TMPro;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Header("패턴 및 UI")]
    [SerializeField] private PatternPanel patternPanel;
    [SerializeField] private BoardManager boardManager;

    [SerializeField] private GameObject youWinPanel;
    [SerializeField] private GameObject glowingStroke;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private TextMeshProUGUI completeText;

    [Header("게임 규칙")]
    [SerializeField] private float totalGameTime = 180f;

    private float timeLeft;
    private int clearCount = 0;
    private bool isGameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);

        timeLeft = totalGameTime;

        patternPanel.CreateTiles();
        patternPanel.GenerateRandomPattern();
        boardManager.ShuffleBoard(); // 내부에서 UpdateClearPattern 포함돼야 함
        SkillGaugeManager.Instance?.ResetGaugeState(); // 게임 시작 시 초기화


        UpdateAllUI();
    }

    private void Update()
    {
        if (isGameEnded) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame("🕒 시간 종료 - 점수로 승자 결정");
        }

        UpdateTimerUI();
    }

    public void UpdateMatchCount(int matchCount)
    {
        completeText.text = matchCount.ToString();
        SkillGaugeManager.Instance?.AddGaugeForMatchCount(matchCount); // 🔥 완성도 반영
    }


    public void OnPatternMatched(int matchedTiles)
    {
        if (isGameEnded) return;

        clearCount++;
        UpdateMatchCount(matchedTiles);

        SkillGaugeManager.Instance?.ResetGaugeState(); // 🎯 클리어 후 상태 초기화

        if (clearCount >= 3)
        {
            isGameEnded = true;
            OnGameClear();
            return;
        }

        patternPanel.GenerateRandomPattern();
        boardManager.ShuffleBoard();
        UpdateAllUI();
    }


    private void EndGame(string reason)
    {
        isGameEnded = true;
        Debug.Log(reason);

        // 점수 비교 로직은 이후 구현 예정
        // 임시로 youWinPanel 비활성화 처리
        glowingStroke?.SetActive(false);
        AudioManager.Instance?.StopBGM();

        StartCoroutine(LoadMainSceneAfterDelay(3f));
    }

    private IEnumerator LoadMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneLoader.LoadMainScene();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        int milliseconds = Mathf.FloorToInt((timeLeft % 1f) * 100);

        timeText.text = $"{minutes}:{seconds:00}:{milliseconds:00}";
    }


    private void UpdateAllUI()
    {
        UpdateTimerUI();
        clearText.text = clearCount.ToString();
    }

    // 기존 기능: 외부에서 패턴 타일 배열 가져오기
    public Tile[] GetPatternTiles()
    {
        return patternPanel.GetPatternTiles();
    }

    // 기존 승리 처리 함수 (클리어 3회 시 사용)
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

        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.winClip);

        glowingStroke?.SetActive(false);

        StartCoroutine(LoadMainSceneAfterDelay(3f));
    }
}
