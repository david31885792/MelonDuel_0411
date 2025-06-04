using UnityEngine;

public class Player1Skill_SlideBooster : MonoBehaviour, ICharacterSkill
{
    public void ActivateSkill()
    {
        Debug.Log("포포리: 슬라이드 부스터 발동!");
        BoardManager.Instance?.SetMoveAnimation(false);
    }
}
