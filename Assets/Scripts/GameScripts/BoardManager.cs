using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [Header("보드 설정")]
    public Transform boardParent;
    public GameObject tilePrefab;

    [Header("크기 및 간격")]
    public float tileSize = 160f;
    public float spacing = 10f;

    [Header("애니메이션")]
    public float moveDuration = 0.2f;
    public Ease moveEase = Ease.OutQuad;

    private int boardSize = 5;
    private Tile[,] tiles = new Tile[5, 5];
    private Vector2 startPos;

    private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    private int maxPerColor = 4;

    private Vector2 emptyTilePos;

    private PatternPanel patternPanel;
    private int[,] clearPattern;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        patternPanel = FindFirstObjectByType<PatternPanel>();

        // 🔧 한 프레임 대기 후 PatternPanel이 Start에서 초기화될 시간 확보
        yield return null;

        if (patternPanel != null)
        {
            int[] flatPattern = patternPanel.GetPattern();
            clearPattern = new int[3, 3];
            for (int i = 0; i < 9; i++)
            {
                clearPattern[i / 3, i % 3] = flatPattern[i];
            }
        }

        GenerateBoard();
    }

    public void GenerateBoard()
    {
        float boardWidth = boardSize * tileSize + (boardSize - 1) * spacing;
        startPos = new Vector2(-boardWidth / 2 + tileSize / 2, boardWidth / 2 - tileSize / 2);

        List<string> tileNames = GenerateTileNames();

        int emptyX = boardSize / 2;
        int emptyY = boardSize / 2;
        emptyTilePos = GetTilePosition(emptyY, emptyX);

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (row == emptyY && col == emptyX) continue;

                string color = tileNames[0];
                tileNames.RemoveAt(0);

                GameObject go = Instantiate(tilePrefab, boardParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = GetTilePosition(row, col);
                rt.sizeDelta = new Vector2(tileSize, tileSize);

                Tile tile = go.GetComponent<Tile>();
                tile.Initialize(row, col, color);

                tiles[row, col] = tile;
            }
        }

        Debug.Log($"✅ Board generated with empty tile at center ({emptyX},{emptyY})");
    }

    private List<string> GenerateTileNames()
    {
        List<string> names = new List<string>();
        foreach (string color in colorNames)
        {
            for (int i = 0; i < maxPerColor; i++)
                names.Add(color);
        }

        for (int i = 0; i < names.Count; i++)
        {
            int rand = Random.Range(i, names.Count);
            (names[i], names[rand]) = (names[rand], names[i]);
        }

        return names;
    }

    private Vector2 GetTilePosition(int row, int col)
    {
        float x = startPos.x + col * (tileSize + spacing);
        float y = startPos.y - row * (tileSize + spacing);
        return new Vector2(Mathf.Round(x), Mathf.Round(y));
    }

    public void TryMoveTile(Tile tile)
    {
        RectTransform rt = tile.GetComponent<RectTransform>();
        Vector2 anchoredStartPos = rt.anchoredPosition; // ✅ DOTween 실행 전에 위치 저장

        float distance = Vector2.Distance(anchoredStartPos, emptyTilePos);
        float expectedDistance = tileSize + spacing;

        if (Mathf.Abs(distance - expectedDistance) < 1f)
        {
            Vector2Int from = GetTileIndexByPosition(anchoredStartPos);     // ✅ 이동 전 위치 기준
            Vector2Int to = GetTileIndexByPosition(emptyTilePos);           // ✅ 이동 대상 위치 기준

            rt.DOAnchorPos(emptyTilePos, moveDuration)
              .SetEase(moveEase)
              .OnComplete(() =>
              {
                  // ✅ 논리 배열 내 타일 참조 위치 스왑
                  tiles[to.y, to.x] = tile;
                  tiles[from.y, from.x] = null;

                  // ✅ 빈칸 위치 갱신
                  emptyTilePos = anchoredStartPos;

                  // ✅ 패턴 매칭 검사
                  IsPatternMatched();
              });
        }
    }



    private void MoveTileToEmpty(Tile tile)
    {
        RectTransform rt = tile.GetComponent<RectTransform>();
        Vector2 tileOriginalPos = rt.anchoredPosition;

        rt.DOAnchorPos(emptyTilePos, moveDuration).SetEase(moveEase);
        emptyTilePos = tileOriginalPos;
    }

    private bool IsPatternMatched()
    {
        // GameSceneManager 안전하게 참조
        GameSceneManager gsm = GameSceneManager.Instance ?? FindFirstObjectByType<GameSceneManager>();
        if (gsm == null)
        {
            Debug.LogError("❌ GameSceneManager 인스턴스를 찾을 수 없습니다!");
            return false;
        }

        Tile[] patternTiles = gsm.GetPatternTiles();

        if (patternTiles == null || patternTiles.Length != 9)
        {
            Debug.LogError("❌ 클리어 패턴이 올바르게 로드되지 않았습니다.");
            return false;
        }

        int patternIndex = 0;

        for (int row = 1; row <= 3; row++)
        {
            for (int col = 1; col <= 3; col++)
            {
                Tile gameTile = tiles[row, col];
                Tile patternTile = patternTiles[patternIndex];

                // 🛡 null 방어
                if (gameTile == null)
                {
                    Debug.LogWarning($"⚠️ 중앙 3x3 중 빈칸 포함됨: tile[{row},{col}] is null → 비교 중단");
                    return false;
                }

                if (patternTile == null)
                {
                    Debug.LogError($"❌ PatternTile[{patternIndex}] is null.");
                    return false;
                }

                if (gameTile.tmpText == null || patternTile.tmpText == null)
                {
                    Debug.LogError($"❌ TMP 텍스트가 null입니다: gameTile[{row},{col}], patternTile[{patternIndex}]");
                    return false;
                }

                int gameNum = int.Parse(gameTile.tmpText.text);
                int patternNum = int.Parse(patternTile.tmpText.text);

                if (gameNum != patternNum)
                {
                    Debug.Log($"❌ 불일치: tile[{row},{col}] = {gameNum}, 기대: {patternNum}");
                    return false;
                }

                patternIndex++;
            }
        }

        Debug.Log("✅ 클리어 패턴과 완벽히 일치!");

        gsm.OnGameClear(); // ✅ 클리어 UI + 씬 전환 실행

        return true;
    }





    private bool ColorsApproximatelyEqual(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }

    private Color GetColorByName(string name)
    {
        string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
        Color[] colorValues = {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green,
        new Color(1f, 0.5f, 0f),
        Color.white
    };

        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return colorValues[i];
        }

        return Color.black;
    }

    private string GetColorName(Color color)
    {
        string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
        Color[] colorValues = {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green,
        new Color(1f, 0.5f, 0f),
        Color.white
    };

        for (int i = 0; i < colorValues.Length; i++)
        {
            if (ColorsApproximatelyEqual(color, colorValues[i]))
                return colorNames[i];
        }
        return "Unknown";
    }
    private Vector2Int GetTileIndexByPosition(Vector2 anchoredPos)
    {
        float tileStep = tileSize + spacing;

        int col = Mathf.RoundToInt((anchoredPos.x - startPos.x) / tileStep);
        int row = Mathf.RoundToInt((startPos.y - anchoredPos.y) / tileStep);

        return new Vector2Int(col, row);
    }

}
