using UnityEngine;
using TMPro;
using System.Collections;

public class ChallengeSceneManager : MonoBehaviour
{
    public static ChallengeSceneManager Instance { get; private set; }

    [Header("패턴 관련")]
    [SerializeField] private ChallengePatternPanel patternPanel;

    [Header("UI 연결")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject goText;
    [SerializeField] private GameObject blockInputPanel;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text clearCountText;
    [SerializeField] private TMP_Text matchCountText;

    [Header("게임 설정")]
    [SerializeField] private float totalTime = 30f;

    private float timeLeft;
    private int clearCount;
    private int currentMatchCount;
    private bool isGameRunning = false;

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
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        blockInputPanel.SetActive(true);
        countdownPanel.SetActive(true);

        // 여기에 3, 2, 1, GO 텍스트 순차 표시
        yield return new WaitForSeconds(4f); // 간단 처리

        countdownPanel.SetActive(false);
        blockInputPanel.SetActive(false);

        StartChallenge();
    }

    private void StartChallenge()
    {
        timeLeft = totalTime;
        clearCount = 0;
        currentMatchCount = 0;
        isGameRunning = true;

        patternPanel.GenerateNewPattern();
    }

    private void Update()
    {
        if (!isGameRunning) return;

        timeLeft -= Time.deltaTime;
        UpdateTimerUI(timeLeft);

        if (timeLeft <= 0f)
        {
            isGameRunning = false;
            EndChallenge();
        }
    }

    public void OnPatternMatched()
    {
        clearCount++;
        patternPanel.GenerateNewPattern();
        UpdateClearCountUI(clearCount);
    }

    public void UpdateMatchCount(int count)
    {
        currentMatchCount = count;
        matchCountText.text = count.ToString();
    }

    private void EndChallenge()
    {
        AudioManager.Instance.StopBGM();
        string scoreString = $"{clearCount}-{currentMatchCount}";
        Debug.Log($"🏁 종료: 최종 점수 {scoreString}");

        // TODO: 최고기록 저장
        SceneLoader.LoadMainScene(); // 또는 결과 창 띄우기
    }

    private void UpdateTimerUI(float time)
    {
        timerText.text = Mathf.CeilToInt(time).ToString();
    }

    private void UpdateClearCountUI(int count)
    {
        clearCountText.text = count.ToString();
    }
}
