using UnityEngine;

public class SkillGaugeManager : MonoBehaviour
{
    [Header("게이지 설정")]
    [Tooltip("게이지 최대 용량 (캐릭터마다 다름)")]
    public int maxGauge = 100;

    [SerializeField, Tooltip("현재 게이지 (읽기 전용)")]
    private int currentGauge = 0;

    [Header("획득량 설정")]
    [Tooltip("타일 이동 시 증가량")]
    public int tileMoveGain = 1;

    [Tooltip("완성도 3 도달 시 증가량")]
    public int match3Gain = 5;

    [Tooltip("완성도 6 도달 시 증가량")]
    public int match6Gain = 10;

    [Tooltip("완성도 9 도달 시 증가량")]
    public int match9Gain = 20;

    // 현재 완성도 단계 추적: 0 → 3 → 6 → 9
    private int lastMatchedStep = 0;

    /// <summary>
    /// 클리어 시 리셋 (다시 3, 6, 9 체크 가능)
    /// </summary>
    public void ResetGaugeState()
    {
        lastMatchedStep = 0;
    }

    /// <summary>
    /// 타일 이동 시 호출
    /// </summary>
    public void AddGaugeForTileMove()
    {
        int gain = isBoostTime ? tileMoveGain * 2 : tileMoveGain;
        AddGauge(gain);
    }


    /// <summary>
    /// 완성도 점수에 따라 최초 3/6/9 도달 시 점수 추가
    /// </summary>
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

    /// <summary>
    /// 내부 게이지 증가 및 한계 검사
    /// </summary>
    private void AddGauge(int amount)
    {
        currentGauge += amount;

        if (currentGauge >= maxGauge)
        {
            currentGauge = maxGauge;
            Debug.Log("스킬 게이지 충전 완료");

            // 💥 스킬 발동 시점 → 게이지 리셋
            OnSkillActivated();
        }

        // TODO: UI 업데이트 등 필요시
    }


    /// <summary>
    /// 게이지 비율 (UI 슬라이더용)
    /// </summary>
    public float GetGaugeNormalized()
    {
        return (float)currentGauge / maxGauge;
    }

    /// <summary>
    /// 현재 게이지 값
    /// </summary>
    public int GetCurrentGauge()
    {
        return currentGauge;
    }

    public static SkillGaugeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnSkillActivated()
    {
        // TODO: 나중에 스킬 실제 효과 발동 로직 넣기

        currentGauge = 0; // 스킬 발동 후 게이지 초기화
        Debug.Log("스킬 사용 후 게이지 초기화됨");
        ResetGaugeState(); // 완성도 보상 단계도 초기화

        FindFirstObjectByType<SkillController>()?.ActivateSkill();

    }

    [HideInInspector]
    public bool isBoostTime = false;

    public void EnterBoostTime()
    {
        isBoostTime = true;
        Debug.Log("🔁 SkillGaugeManager: Boost Time Started (2x Gain)");
    }

}