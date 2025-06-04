using UnityEngine;

public class Player3Skill_WildTiles : MonoBehaviour
{
    [SerializeField] private int wildTileCount = 3;

    public void ActivateSkill()
    {
        Debug.Log("체리나: 만능 타일 생성!");
        BoardManager.Instance?.ConvertRandomTilesToWild(wildTileCount);
    }
}
