using UnityEngine;
using TMPro;
using System.Collections;

public class ChallengeSceneManager : MonoBehaviour
{
    public static ChallengeSceneManager Instance { get; private set; }

    [Header("패턴 관련")]
    [SerializeField] private ChallengePatternPanel patternPanel;

    [Header("UI 연결")]
    [SerializeField] private GameObject goText;
    [SerializeField] private GameObject blockInputPanel;

    [Header("카운트다운 숫자 오브젝트")] // ✅ 추가된 부분
    [SerializeField] private GameObject count3;
    [SerializeField] private GameObject count2;
    [SerializeField] private GameObject count1;

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

        // ✅ 카운트다운 순차 표시
        count1.SetActive(false);
        count2.SetActive(false);
        count3.SetActive(false);
        goText.SetActive(false);

        count3.SetActive(true);
        yield return new WaitForSeconds(1f);
        count3.SetActive(false);

        count2.SetActive(true);
        yield return new WaitForSeconds(1f);
        count2.SetActive(false);

        count1.SetActive(true);
        yield return new WaitForSeconds(1f);
        count1.SetActive(false);

        goText.SetActive(true);
        yield return new WaitForSeconds(1f);
        goText.SetActive(false);

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

        string score = $"{clearCount}-{currentMatchCount}";
        Debug.Log($"🏁 종료: 최종 점수 {score}");

        ChallengeScore.SaveScore(clearCount, currentMatchCount);

        SceneLoader.LoadMainScene();
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
