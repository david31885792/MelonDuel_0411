using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class Tile : MonoBehaviour, IPointerClickHandler
{
    [Header("타일 좌표")]
    public int x;
    public int y;

    [Header("타일 이름 (색상 이름)")]
    public string tileName;

    [Header("타일 이미지")]
    public Image tileImage;

    [Header("색상 매핑")]
    public string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    public Color[] colorValues;

    /// <summary>
    /// 타일 초기화
    /// </summary>
    public void Initialize(int x, int y, string name)
    {
        this.x = x;
        this.y = y;
        this.tileName = name;

        if (tileImage == null)
            tileImage = GetComponent<Image>();

        UpdateColor(name);
    }

    /// <summary>
    /// 클릭 시 호출됨 (IPointerClickHandler)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        BoardManager.Instance.TryMoveTile(this);
    }

    /// <summary>
    /// 색상 이름에 따라 색상 지정
    /// </summary>
    public void UpdateColor(string name)
    {
        if (tileImage == null) return;

        for (int i = 0; i < colorNames.Length && i < colorValues.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                tileImage.color = colorValues[i];
                return;
            }
        }

        tileImage.color = Color.black;
    }

    private void Start()
    {
        // 알파값 강제 설정
        if (tileImage != null)
        {
            Color c = tileImage.color;
            c.a = 1f;
            tileImage.color = c;
        }
    }
}
