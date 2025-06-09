using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillController : MonoBehaviour
{
    private ICharacterSkill currentSkill;
    [SerializeField] private SkillGaugeManager gaugeManager;

    // ✅ 추가: 유저인지 AI인지 판별 (외부에서 설정 가능)
    [SerializeField] private bool isAI = false;

    // ✅ 추가: 이펙트 이미지들 (6개 할당)
    [Header("Skill Effect Images")]
    [SerializeField] private GameObject player1UserEffect;
    [SerializeField] private GameObject player1AIEffect;
    [SerializeField] private GameObject player2UserEffect;
    [SerializeField] private GameObject player2AIEffect;
    [SerializeField] private GameObject player3UserEffect;
    [SerializeField] private GameObject player3AIEffect;

    public void SetSkill(ICharacterSkill skill)
    {
        currentSkill = skill;

        if (gaugeManager != null && currentSkill != null)
        {
            int maxGauge = GetGaugeFromSkill(currentSkill);
            gaugeManager.maxGauge = maxGauge;
        }
    }

    private int GetGaugeFromSkill(ICharacterSkill skill)
    {
        switch (skill)
        {
            case Player1Skill_SlideBooster s: return s.GetSkillMaxGauge();
            case Player2Skill_Blind s: return s.GetSkillMaxGauge();
            case Player3Skill_WildTiles s: return s.GetSkillMaxGauge();
            default: return 100;
        }
    }

    // ✅ 스킬 발동 시 이펙트 실행 추가
    public void ActivateSkill()
    {
        if (currentSkill != null)
        {
            currentSkill.ActivateSkill();
            ShowEffectForSkill(currentSkill);
            Debug.Log("스킬 사용됨");
        }
        else
        {
            Debug.LogWarning("스킬이 설정되지 않았습니다.");
        }
    }

    // ✅ 현재 스킬에 따라 맞는 이펙트 보여주기
    private void ShowEffectForSkill(ICharacterSkill skill)
    {
        GameObject targetEffect = null;
        string clipName = "";

        if (skill is Player1Skill_SlideBooster)
        {
            targetEffect = isAI ? player1AIEffect : player1UserEffect;
            clipName = isAI ? "SlideInOut_Rival_P1" : "SlideInOut_User_P1";
        }
        else if (skill is Player2Skill_Blind)
        {
            targetEffect = isAI ? player2AIEffect : player2UserEffect;
            clipName = isAI ? "SlideInOut_Rival_P2" : "SlideInOut_User_P2";
        }
        else if (skill is Player3Skill_WildTiles)
        {
            targetEffect = isAI ? player3AIEffect : player3UserEffect;
            clipName = isAI ? "SlideInOut_Rival_P3" : "SlideInOut_User_P3";
        }

        if (targetEffect != null && !string.IsNullOrEmpty(clipName))
        {
            StartCoroutine(PlayEffect(targetEffect, clipName));
        }
    }


    // ✅ 이펙트 3초간 재생 후 자동 비활성화
    private IEnumerator PlayEffect(GameObject effectGO, string clipName)
    {
        effectGO.SetActive(true);

        Animator anim = effectGO.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play(clipName, 0, 0f);
        }

        yield return new WaitForSeconds(3f);
        effectGO.SetActive(false);
    }

}
