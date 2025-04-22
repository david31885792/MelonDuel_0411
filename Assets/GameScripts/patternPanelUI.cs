using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PatternPanelUI : MonoBehaviour
{
    [Header("9개 칸 (왼→오, 위→아래 순서)")]
    public List<Image> patternImages;

    [Header("색상 이름 → 컬러 매핑")]
    public Color defaultColor = Color.gray;
    public List<string> colorNames;
    public List<Color> colorValues;

    private Dictionary<string, Color> colorMap = new Dictionary<string, Color>();

    void Awake()
    {
        for (int i = 0; i < Mathf.Min(colorNames.Count, colorValues.Count); i++)
            colorMap[colorNames[i]] = colorValues[i];
    }

    public void SetPattern(string[,] pattern)
    {
        if (patternImages.Count < 9)
        {
            Debug.LogWarning("❌ [PatternUI] 이미지 부족");
            return;
        }

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                int idx = y * 3 + x;
                string name = pattern[y, x];
                patternImages[idx].color = colorMap.ContainsKey(name) ? colorMap[name] : defaultColor;
            }
        }

        Debug.Log("🧩 [PatternUI] 패턴 표시 완료");
    }
}
