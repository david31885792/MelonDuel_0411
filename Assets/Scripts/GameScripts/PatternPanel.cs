using UnityEngine;
using UnityEngine.UI;

public class PatternPanel : MonoBehaviour
{
    [Header("패턴 셀들 (3x3 총 9개)")]
    public Image[] patternCells; // 0~8

    [Header("색상 매핑")]
    public string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    public Color[] colorValues;

    private void Start()
    {
        LoadAndDisplayPattern();
    }

    private void LoadAndDisplayPattern()
    {
        string[,] pattern = PlayerPrefsPatternLoader.LoadPattern();

        if (pattern == null)
        {
            Debug.LogError("❌ 패턴 불러오기 실패");
            return;
        }

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int index = row * 3 + col;
                string colorName = pattern[row, col];

                Color visibleColor = GetColorByName(colorName);
                visibleColor.a = 1f; // 🔥 알파값 강제 설정
                patternCells[index].color = visibleColor;

                patternCells[index].gameObject.SetActive(true); // 혹시 비활성화된 경우도 방지
            }
        }

    }

    private Color GetColorByName(string name)
    {
        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return colorValues[i];
        }
        return Color.black; // 기본값
    }

    public void SetPattern(string[,] pattern)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int index = row * 3 + col;
                string colorName = pattern[row, col];

                Color visibleColor = GetColorByName(colorName);
                visibleColor.a = 1f; // 🔥 알파값 강제 설정
                patternCells[index].color = visibleColor;

                patternCells[index].gameObject.SetActive(true); // 혹시 비활성화된 경우도 방지
            }
        }

    }

}
