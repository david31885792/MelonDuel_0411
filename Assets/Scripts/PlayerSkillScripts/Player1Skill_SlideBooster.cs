using UnityEngine;

public class Player1Skill_SlideBooster : MonoBehaviour, ICharacterSkill
{
    public void ActivateSkill()
    {
        Debug.Log("포포리: 슬라이드 부스터 발동!");
        BoardManager.Instance?.SetMoveAnimation(false);
    }
    public bool IsReady()
    {
        // 기본 구현: 항상 준비 상태로 반환
        return true;
    }
}
