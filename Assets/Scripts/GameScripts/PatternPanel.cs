using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PatternPanel : MonoBehaviour
{
    [Header("타일 프리팹과 부모")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform boardParent;

    [Header("패턴 타일 저장")]
    private Tile[] patternTiles = new Tile[9];

    [Header("색상 매핑")]
    public string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    public Color[] colorValues;

    public Tile[] GetPatternTiles()
    {
        return GetComponentsInChildren<Tile>();
    }

    // 🔧 오류 해결을 위한 메서드 추가
    public int[] GetPattern()
    {
        int[] patternNumbers = new int[patternTiles.Length];
        for (int i = 0; i < patternTiles.Length; i++)
        {
            patternNumbers[i] = patternTiles[i].TMPNumber;
        }
        return patternNumbers;
    }

    private void Start()
    {
        CreateTiles();
        LoadAndDisplayPattern();
    }

    private void CreateTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            Tile tile = Instantiate(tilePrefab, boardParent);
            tile.SetTMPNumber(0); // 초기화
            tile.GetComponentInChildren<TextMeshProUGUI>().enabled = false; // TMP 안보이게

            // 알파값 강제 설정
            Image img = tile.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }

            patternTiles[i] = tile;
        }
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

                Color color = GetColorByName(colorName);
                patternTiles[index].SetColor(color);
                patternTiles[index].SetTMPNumber(GetColorIndex(colorName) + 1);
            }
        }
    }

    private Color GetColorByName(string name)
    {
        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                Color c = colorValues[i];
                c.a = 1f;
                return c;
            }
        }
        return Color.black;
    }

    private int GetColorIndex(string name)
    {
        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }
}
