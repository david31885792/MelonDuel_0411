using UnityEngine;

public class SkillController : MonoBehaviour
{
    private ICharacterSkill currentSkill;

    // 외부에서 스킬 지정
    public void SetSkill(ICharacterSkill skill)
    {
        currentSkill = skill;
    }

    // 스킬 발동
    public void ActivateSkill()
    {
        if (currentSkill != null)
        {
            currentSkill.ActivateSkill();
            Debug.Log("스킬 사용됨");
        }
        else
        {
            Debug.LogWarning("스킬이 설정되지 않았습니다.");
        }
    }
}
