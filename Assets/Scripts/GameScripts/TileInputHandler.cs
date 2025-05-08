using UnityEngine;
using UnityEngine.EventSystems;

public class TileInputHandler : MonoBehaviour, IPointerClickHandler
{
    private Tile tile;

    private void Awake()
    {
        tile = GetComponent<Tile>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tile != null)
        {
            BoardManager.Instance.TryMoveTile(tile); // 빈칸 인접하면 이동 시도
        }
    }
}
