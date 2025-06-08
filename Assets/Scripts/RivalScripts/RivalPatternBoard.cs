using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RivalPatternBoard : MonoBehaviour
{
    [Header("타일 프리팹과 부모")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform boardParent;

    [Header("색상 매핑")]
    public string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    public Color[] colorValues;

    private Tile[] patternTiles = new Tile[9];

    private void Start()
    {
        // 자동 실행하지 않음. AIController에서 수동 호출
    }

    public void CreateTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            Tile tile = Instantiate(tilePrefab, boardParent);
            tile.SetTMPNumber(0); // 초기화

            TextMeshProUGUI tmp = tile.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.enabled = false;

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

    public void GenerateRandomPattern()
    {
        if (patternTiles == null || patternTiles.Length != 9)
        {
            Debug.LogError("❌ [AI] 패턴 타일이 생성되지 않았습니다. CreateTiles()가 먼저 호출되어야 합니다.");
            return;
        }

        for (int i = 0; i < patternTiles.Length; i++)
        {
            if (patternTiles[i] == null)
            {
                Debug.LogError($"❌ [AI] patternTiles[{i}]가 null입니다.");
                continue;
            }

            int index = Random.Range(0, colorValues.Length);
            Color color = colorValues[index];

            patternTiles[i].SetColor(color);
            patternTiles[i].SetTMPNumber(index + 1);

            TextMeshProUGUI tmp = patternTiles[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.enabled = false;
        }

        Debug.Log("🎯 [AI] 랜덤 패턴 생성 완료");
    }

    public int[] GetPattern()
    {
        int[] patternNumbers = new int[patternTiles.Length];
        for (int i = 0; i < patternTiles.Length; i++)
        {
            patternNumbers[i] = patternTiles[i].TMPNumber;
        }
        return patternNumbers;
    }

    public int[,] GetPatternAsMatrix()
    {
        int[,] matrix = new int[3, 3];
        for (int i = 0; i < 9; i++)
        {
            matrix[i / 3, i % 3] = patternTiles[i].TMPNumber;
        }
        return matrix;
    }

    public Tile[] GetPatternTiles()
    {
        return patternTiles;
    }

    // ... 기존 코드 유지 ...

    /// <summary>
    /// 외부에서 호출 가능한 패턴 생성 함수 (AIController 전용)
    /// </summary>
    public void GenerateNewPattern()
    {
        CreateTiles();             // 타일 먼저 생성
        GenerateRandomPattern();   // 그 위에 랜덤 패턴 적용
    }

    public Tile[,] GetCurrentPattern()
    {
        // Assuming the pattern is stored in a 3x3 matrix format
        Tile[,] currentPattern = new Tile[3, 3];
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                currentPattern[y, x] = patternTiles[y * 3 + x];
            }
        }
        return currentPattern;
    }

    public int GetWidth()
    {
        return 3;
    }

    public int GetHeight()
    {
        return 3;
    }

    public Tile GetPatternTile(int x, int y)
    {
        if (x >= 0 && x < 3 && y >= 0 && y < 3)
        {
            return patternTiles[y * 3 + x];
        }
        return null;
    }

    public Vector2Int GetBoardPositionFromPatternOffset(int x, int y)
    {
        // 보드 중앙 (4,4)를 기준으로 3x3 패턴 배치 시 적용
        return new Vector2Int(4 + x - 1, 4 + y - 1);
    }

    public Tile GetTileAtBoardPos(Vector2Int pos)
    {
        return FindFirstObjectByType<RivalBoardManager>().GetTileAt(pos);
    }


}
