using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("타일 위치")]
    public int x;
    public int y;

    [Header("타일 이름")]
    public string tileName;

    [Header("타일 이미지")]
    public Image tileImage;

    [Header("색상 매핑 (이름과 값 순서 일치)")]
    [SerializeField]
    private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };

    [SerializeField]
    private Color[] colorValues;

    /// <summary>
    /// 타일 초기화 (좌표, 이름 지정)
    /// </summary>
    public void Initialize(int x, int y, string name)
    {
        this.x = x;
        this.y = y;
        UpdateColor(name);
    }

    /// <summary>
    /// 색상 이름에 따라 이미지 색상 변경
    /// </summary>
    public void UpdateColor(string name)
    {
        tileName = name;

        for (int i = 0; i < colorNames.Length && i < colorValues.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                if (tileImage != null)
                    tileImage.color = colorValues[i];
                return;
            }
        }

        // 매칭 실패 시 기본값
        if (tileImage != null)
            tileImage.color = Color.black;
    }
}
