using UnityEngine;
using System.Collections;

public class Player2Skill_Blind : MonoBehaviour, ICharacterSkill
{
    [SerializeField] private bool isForAI = false;

    [SerializeField] private UIBlocker playerUIBlocker;
    [SerializeField] private InputBlocker playerInputBlocker;

    [SerializeField] private UIBlocker aiUIBlocker;
    [SerializeField] private InputBlocker aiInputBlocker;

    [SerializeField] private float blindDuration = 3f;

    [SerializeField] private int skillMaxGauge = 40;

    public int GetSkillMaxGauge() => skillMaxGauge;

    private bool isAI = false;

    public void SetIsAI(bool value) => isAI = value;


    public void ActivateSkill()
    {
        Debug.Log("루루노: 블라인드 스킬 발동!");
        StartCoroutine(ApplyBlind());

        FindFirstObjectByType<SkillEffectManager>()?.ShowEffect(SkillEffectManager.SkillType.Player2, isAI);
    }

    private IEnumerator ApplyBlind()
    {
        if (isForAI)
        {
            aiUIBlocker?.Show();
            aiInputBlocker?.Block();

            yield return new WaitForSeconds(blindDuration);

            aiUIBlocker?.Hide();
            aiInputBlocker?.Unblock();
        }
        else
        {
            playerUIBlocker?.Show();
            playerInputBlocker?.Block();

            yield return new WaitForSeconds(blindDuration);

            playerUIBlocker?.Hide();
            playerInputBlocker?.Unblock();
        }
    }

    public bool IsReady() => true;
}
