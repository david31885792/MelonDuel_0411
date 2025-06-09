using UnityEngine;

public class RivalScoreManager : MonoBehaviour
{
    private int clearScore = 0;
    private int completeScore = 0;

    [SerializeField] private TMPro.TextMeshProUGUI clearScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI completeScoreText;

    public void AddClearScore(int value)
    {
        clearScore += value;
        Debug.Log($"[AI] 클리어 점수 +{value} → 총: {clearScore}");
        UpdateClearScoreUI();
    }

    public void AddCompleteScore(int value)
    {
        completeScore += value;
        Debug.Log($"[AI] 완성도 점수 +{value} → 총: {completeScore}");
        UpdateCompleteScoreUI();
    }

    private void UpdateClearScoreUI()
    {
        if (clearScoreText != null)
            clearScoreText.text = clearScore.ToString();
    }

    private void UpdateCompleteScoreUI()
    {
        if (completeScoreText != null)
            completeScoreText.text = completeScore.ToString();
    }

    public int GetClearScore() => clearScore;
    public int GetCompleteScore() => completeScore;
}
