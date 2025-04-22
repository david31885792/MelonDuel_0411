using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public int x, y;
    public string tileName; // ex: "Top", "Bottom", ...
    public Image image;

    private BoardManager board;

    public void Init(BoardManager board, int x, int y, string name, Color color)
    {
        this.board = board;
        this.x = x;
        this.y = y;
        tileName = name;

        if (image == null) image = GetComponent<Image>();
        image.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        board?.OnTileClicked(this);
    }
}
