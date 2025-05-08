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
            BoardManager.Instance.TryMoveTile(tile); // ��ĭ �����ϸ� �̵� �õ�
        }
    }
}
