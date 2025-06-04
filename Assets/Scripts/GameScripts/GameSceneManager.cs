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
    [SerializeField] private GameObject boostTimeText;


    [Header("게임 규칙")]
    [SerializeField] private float totalGameTime = 180f;

    private float timeLeft;
    private int clearCount = 0;
    private bool isGameEnded = false;
    private bool hasEnteredBoostTime = false;



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
        boardManager.ShuffleBoard();
        SkillGaugeManager.Instance?.ResetGaugeState();

        // ✅ Skill 연결 (문자열 기반으로 수정됨)
        string selectedCharacterId = PlayerPrefs.GetString("SelectedCharacterID", "player1");
        Debug.Log($"[GameSceneManager] 선택된 캐릭터 ID: {selectedCharacterId}");

        SkillController skillController = FindFirstObjectByType<SkillController>();

        switch (selectedCharacterId)
        {
            case "player1":
                skillController.SetSkill(skillController.GetComponent<Player1Skill_SlideBooster>());
                break;
            case "player2":
                skillController.SetSkill(skillController.GetComponent<Player2Skill_Blind>());
                break;
            case "player3":
                skillController.SetSkill(skillController.GetComponent<Player3Skill_WildTiles>());
                break;
            default:
                Debug.LogWarning("알 수 없는 캐릭터 ID, 기본값 사용됨 (포포리)");
                skillController.SetSkill(skillController.GetComponent<Player1Skill_SlideBooster>());
                break;
        }

        UpdateAllUI();
    }

    private void Update()
    {
        if (!hasEnteredBoostTime && timeLeft <= 60f)
        {
            hasEnteredBoostTime = true;

            SkillGaugeManager.Instance?.EnterBoostTime();
            Debug.Log("💥 Boost Time Activated!");

            ShowBoostTimeUI();
        }

        if (isGameEnded) return;

        timeLeft -= Time.deltaTime;

        // ⏱ Boost Time 진입 조건
        if (!hasEnteredBoostTime && timeLeft <= 60f)
        {
            hasEnteredBoostTime = true;
            SkillGaugeManager.Instance?.EnterBoostTime();
            Debug.Log("💥 Boost Time Activated!");
        }

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
        SkillGaugeManager.Instance?.AddGaugeForMatchCount(matchCount);
    }

    public void OnPatternMatched(int matchedTiles)
    {
        if (isGameEnded) return;

        clearCount++;
        UpdateMatchCount(matchedTiles);

        SkillGaugeManager.Instance?.ResetGaugeState();

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

    public Tile[] GetPatternTiles()
    {
        return patternPanel.GetPatternTiles();
    }

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

    private void ShowBoostTimeUI()
    {
        if (boostTimeText != null)
        {
            boostTimeText.SetActive(true);
            Invoke(nameof(HideBoostTimeUI), 1.5f); // 1.5초 후 자동 숨김
        }
    }

    private void HideBoostTimeUI()
    {
        boostTimeText?.SetActive(false);
    }
}
