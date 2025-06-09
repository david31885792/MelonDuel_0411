using UnityEngine;

public class Player3Skill_WildTiles : MonoBehaviour, ICharacterSkill
{
    [SerializeField] private bool isForAI = false;

    [SerializeField] private BoardManager playerBoardManager;
    [SerializeField] private RivalBoardManager aiBoardManager;

    [SerializeField] private int wildTileCount = 3;

    [SerializeField] private int skillMaxGauge = 50;

    public int GetSkillMaxGauge() => skillMaxGauge;

    private bool isAI = false;

    public void SetIsAI(bool value) => isAI = value;



    public void ActivateSkill()
    {
        Debug.Log("체리나: 와일드 타일 생성 스킬 발동!");

        if (isForAI && aiBoardManager != null)
        {
            aiBoardManager.ConvertRandomTilesToWild(wildTileCount);
        }
        else if (!isForAI && playerBoardManager != null)
        {
            playerBoardManager.ConvertRandomTilesToWild(wildTileCount);
        }

        FindFirstObjectByType<SkillEffectManager>()?.ShowEffect(SkillEffectManager.SkillType.Player3, isAI);
    }


    public bool IsReady() => true;
}
