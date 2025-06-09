using UnityEngine;

public class Player1Skill_SlideBooster : MonoBehaviour, ICharacterSkill
{
    [SerializeField] private bool isForAI = false;
    [SerializeField] private BoardManager playerBoardManager;
    [SerializeField] private RivalBoardManager aiBoardManager;

    [SerializeField] private int skillMaxGauge = 30;

    [SerializeField] private float effectDuration = 10f; // Inspector에서 조절 가능
    private float effectTimer = 0f;
    private bool effectActive = false;


    public int GetSkillMaxGauge() => skillMaxGauge;


    public void ActivateSkill()
    {
        Debug.Log("포포리: 슬라이드 부스터 발동!");

        if (isForAI)
        {
            aiBoardManager?.SetMoveAnimation(false);
        }
        else
        {
            playerBoardManager?.SetMoveAnimation(false);
        }

        effectTimer = effectDuration;
        effectActive = true;

        FindFirstObjectByType<SkillEffectManager>()?.ShowEffect(SkillEffectManager.SkillType.Player1, isForAI);
    }

    private void Update()
    {
        if (!effectActive) return;

        effectTimer -= Time.deltaTime;
        if (effectTimer <= 0f)
        {
            effectActive = false;

            if (isForAI)
            {
                aiBoardManager?.SetMoveAnimation(true);
            }
            else
            {
                playerBoardManager?.SetMoveAnimation(true);
            }

            Debug.Log("슬라이드 부스터 효과 종료");
        }
    }

    public bool IsReady()
    {
        return true;
    }
}
