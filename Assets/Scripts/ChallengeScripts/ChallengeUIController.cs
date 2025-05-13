using UnityEngine;
using TMPro;

public class ChallengeUIController : MonoBehaviour
{
    public static ChallengeUIController Instance { get; private set; }

    [Header("UI 요소 연결")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text clearCountText;
    [SerializeField] private TMP_Text matchCountText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateTimer(float secondsLeft)
    {
        int displayTime = Mathf.CeilToInt(secondsLeft);
        timerText.text = displayTime.ToString();
    }

    public void UpdateClearCount(int count)
    {
        clearCountText.text = count.ToString();
    }

    public void UpdateMatchCount(int match)
    {
        matchCountText.text = match.ToString();
    }
}
