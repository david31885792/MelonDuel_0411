using UnityEngine;

public class Player3Skill_WildTiles : MonoBehaviour, ICharacterSkill
{
    [SerializeField] private int wildTileCount = 3;

    public void ActivateSkill()
    {
        Debug.Log("체리나: 만능 타일 생성!");
        BoardManager.Instance?.ConvertRandomTilesToWild(wildTileCount);
    }
    public bool IsReady()
    {
        // 기본 구현: 스킬이 항상 준비된 상태로 가정
        return true;
    }
}
