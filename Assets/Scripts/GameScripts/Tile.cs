using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    [Header("타일 위치")]
    public int x;
    public int y;

    [Header("타일 속성")]
    public string tileName;
    public Image tileImage;
    public TextMeshProUGUI tmpText;

    [Header("색상 매핑")]
    [SerializeField] private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    [SerializeField] private Color[] colorValues;

    private int tmpNumber; // 🔧 TMP 숫자 저장용 필드 추가

    public int TMPNumber => tmpNumber; // 🔧 외부에서 읽을 수 있도록 프로퍼티 추가

    public void Initialize(int x, int y, string name)
    {
        this.x = x;
        this.y = y;
        UpdateColor(name);
    }

    public void UpdateColor(string name)
    {
        tileName = name;
        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                SetColor(colorValues[i]);
                SetTMPNumber(i + 1); // 유튜브 방식의 넘버링
                return;
            }
        }
        SetColor(Color.black);
        SetTMPNumber(0);
    }

    public void SetColor(Color color)
    {
        if (tileImage != null)
        {
            color.a = 1f;
            tileImage.color = color;
        }
    }

    public void SetTMPNumber(int number)
    {
        tmpNumber = number; // 🔧 숫자 저장
        if (tmpText != null)
        {
            tmpText.text = number.ToString();
            tmpText.enabled = false; // 보이지 않게
        }
    }
}
