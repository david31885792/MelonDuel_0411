using UnityEngine;

/// <summary>
/// AI 전용 스킬 컨트롤러 (기존 SkillController 구조 계승)
/// AI가 상황에 따라 ActivateSkill()을 직접 호출함
/// </summary>
public class RivalSkillController : MonoBehaviour
{
    private ICharacterSkill currentSkill;

    [SerializeField] private RivalGaugeManager gaugeManager;


    /// <summary>
    /// 외부에서 캐릭터 스킬 할당
    /// </summary>
    public void SetSkill(ICharacterSkill skill)
    {
        currentSkill = skill;
        Debug.Log($"[RivalSkillController] 스킬 세팅됨: {skill?.GetType().Name ?? "NULL"}");
    }

    /// <summary>
    /// 스킬 발동 (AI가 판단하여 직접 호출)
    /// </summary>
    public void ActivateSkill()
    {
        if (currentSkill != null)
        {
            currentSkill.ActivateSkill();
            Debug.Log("🤖 [AI] 스킬 사용됨");
        }
        else
        {
            Debug.LogWarning("⚠️ [AI] 스킬이 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 현재 스킬이 할당되어 있는지 확인
    /// </summary>
    public bool HasSkill()
    {
        return currentSkill != null;
    }

    public bool IsSkillReady() // Add this method to fix the error
    {
        return currentSkill != null && currentSkill.IsReady();
    }

    public RivalGaugeManager GetGaugeManager()
    {
        return gaugeManager;
    }

    public void UseSkill()
    {
        ActivateSkill(); // 기존 메서드 재사용
    }

}
