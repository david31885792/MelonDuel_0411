using UnityEngine;
using UnityEngine.UI;

public class PatternPanel : MonoBehaviour
{
    [Header("타일 프리팹 및 부모")]
    public GameObject tilePrefab;
    public Transform patternParent;

    [Header("위치 및 크기")]
    public float tileSize = 160f;
    public float spacing = 10f;

    [Header("색상 매핑")]
    public string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    public Color[] colorValues;

    private string[,] clearPattern;

    private void Start()
    {
        clearPattern = PlayerPrefsPatternLoader.LoadPattern();

        if (clearPattern == null)
        {
            Debug.LogError("❌ 패턴 불러오기 실패");
            return;
        }

        GeneratePatternTiles();
    }

    private void GeneratePatternTiles()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                string colorName = clearPattern[row, col];

                GameObject go = Instantiate(tilePrefab, patternParent);
                Tile tile = go.GetComponent<Tile>();
                tile.Initialize(row, col, colorName);
                TileInputHandler handler = tile.GetComponent<TileInputHandler>();
                if (handler != null)
                    handler.enabled = false; // ✅ 클릭 막기


                // 크기 및 위치 설정
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileSize, tileSize);
                rt.anchoredPosition = GetTilePosition(row, col);

                // 알파값 강제 설정
                Image img = tile.tileImage;
                if (img != null)
                {
                    Color fixedColor = GetColorByName(colorName);
                    fixedColor.a = 1f;
                    img.color = fixedColor;
                }
            }
        }
    }

    private Vector2 GetTilePosition(int row, int col)
    {
        float boardWidth = 3 * tileSize + 2 * spacing;
        float startX = -boardWidth / 2 + tileSize / 2;
        float startY = boardWidth / 2 - tileSize / 2;

        float x = startX + col * (tileSize + spacing);
        float y = startY - row * (tileSize + spacing);
        return new Vector2(x, y);
    }

    private Color GetColorByName(string name)
    {
        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return colorValues[i];
        }
        return Color.black;
    }
}
