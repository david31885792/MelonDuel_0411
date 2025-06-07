using UnityEngine;

public class RivalGaugeManager : MonoBehaviour
{
    [Header("게이지 설정")]
    [Tooltip("게이지 최대 용량 (캐릭터마다 다름)")]
    public int maxGauge = 100;

    [SerializeField, Tooltip("현재 게이지 (읽기 전용)")]
    private int currentGauge = 0;

    [Header("획득량 설정")]
    public int tileMoveGain = 1;
    public int match3Gain = 5;
    public int match6Gain = 10;
    public int match9Gain = 20;

    [Header("스킬 컨트롤러")]
    [SerializeField] private RivalSkillController skillController;

    private int lastMatchedStep = 0;
    [HideInInspector] public bool isBoostTime = false;

    public void ResetGaugeState()
    {
        lastMatchedStep = 0;
    }

    public void AddGaugeForTileMove()
    {
        int gain = isBoostTime ? tileMoveGain * 2 : tileMoveGain;
        //Debug.Log($"[Gauge] AddGaugeForTileMove() 호출됨: gain={gain} (base={tileMoveGain}, boost={isBoostTime})");
        AddGauge(gain);
    }

    public void AddGaugeForMatchCount(int matchCount)
    {
        if (matchCount >= 3 && lastMatchedStep < 3)
        {
            AddGauge(match3Gain);
            lastMatchedStep = 3;
        }

        if (matchCount >= 6 && lastMatchedStep < 6)
        {
            AddGauge(match6Gain);
            lastMatchedStep = 6;
        }

        if (matchCount >= 9 && lastMatchedStep < 9)
        {
            AddGauge(match9Gain);
            lastMatchedStep = 9;
        }
    }

    private void AddGauge(int amount)
    {
        currentGauge += amount;
        //Debug.Log($"[Gauge] AddGauge({amount}) → 누적 gauge: {currentGauge}/{maxGauge}");

        if (currentGauge >= maxGauge)
        {
            currentGauge = maxGauge;
            Debug.Log("🤖 [AI] 스킬 게이지 최대 도달");

            OnSkillActivated(); // 스킬 발동 후 게이지 초기화
        }
    }

    private void OnSkillActivated()
    {
        if (skillController != null && skillController.HasSkill())
        {
            skillController.ActivateSkill();
        }
        else
        {
            Debug.LogWarning("⚠️ [AI] 스킬 컨트롤러 또는 스킬이 없음");
        }

        currentGauge = 0;
        ResetGaugeState();
        Debug.Log("🧹 [AI] 스킬 사용 후 게이지 리셋");
    }

    public float GetGaugeNormalized()
    {
        return (float)currentGauge / maxGauge;
    }

    public int GetCurrentGauge()
    {
        return currentGauge;
    }

    public void EnterBoostTime()
    {
        isBoostTime = true;
        Debug.Log("🔥 [AI] Boost Time 진입 - 게이지 획득 2배");
    }

    public void SetSkillController(RivalSkillController controller)
    {
        skillController = controller;
    }

}
