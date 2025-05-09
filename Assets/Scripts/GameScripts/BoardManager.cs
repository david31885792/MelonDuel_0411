using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    private Vector2 emptyTilePos;  // ✅ 빈칸 위치만 값으로 관리

    private bool hasCheckedClear = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        hasCheckedClear = false;
        GenerateBoard();

        string[,] pattern = PlayerPrefsPatternLoader.LoadPattern();
        BoardManager.Instance.clearPattern = pattern;

        Debug.Log($"패턴 값 확인: {clearPattern[0, 0]} ~ {clearPattern[2, 2]}");

    }

    public void GenerateBoard()
    {
        float boardWidth = boardSize * tileSize + (boardSize - 1) * spacing;
        startPos = new Vector2(-boardWidth / 2 + tileSize / 2, boardWidth / 2 - tileSize / 2);

        List<string> tileNames = GenerateTileNames();

        // 빈칸 위치 중앙으로 지정
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
        Vector2 tilePos = tile.GetComponent<RectTransform>().anchoredPosition;
        float distance = Vector2.Distance(tilePos, emptyTilePos);
        float expectedDistance = tileSize + spacing;

        if (Mathf.Abs(distance - expectedDistance) < 1f)
        {
            RectTransform rt = tile.GetComponent<RectTransform>();
            Vector2 tileOriginalPos = rt.anchoredPosition;

            rt.DOAnchorPos(emptyTilePos, moveDuration)
              .SetEase(moveEase)
              .OnComplete(() =>
              {
                  emptyTilePos = tileOriginalPos;

                             // ✅ 움직일 때마다 검사
                  IsPatternMatched();
              });
        }
    }




    private void MoveTileToEmpty(Tile tile)
    {
        RectTransform rt = tile.GetComponent<RectTransform>();
        Vector2 tileOriginalPos = rt.anchoredPosition;

        // 타일 이동
        rt.DOAnchorPos(emptyTilePos, moveDuration).SetEase(moveEase);

        // 빈칸 위치 갱신
        emptyTilePos = tileOriginalPos;
    }

    // Add this to BoardManager.cs
    public string[,] clearPattern; // 외부에서 패턴 주입 필요

    private bool IsPatternMatched()
    {
        bool isMatched = true;

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int boardRow = row + 1;
                int boardCol = col + 1;

                Tile tile = tiles[boardRow, boardCol];
                if (tile == null)
                {
                    Debug.Log($"❌ tile[{boardRow},{boardCol}] = null");
                    isMatched = false;
                    continue;
                }

                Color actualColor = tile.tileImage.color;
                string expectedName = clearPattern[row, col];
                Color expectedColor = GetColorByName(expectedName);

                if (!ColorsApproximatelyEqual(actualColor, expectedColor))
                {
                    string actualColorName = GetColorName(actualColor);
                    Debug.Log($"❌ 불일치: tile[{boardRow},{boardCol}] = {actualColorName} ({actualColor}), 기대: {expectedName} ({expectedColor})");
                    isMatched = false;
                }
            }
        }

        if (isMatched)
        {
            Debug.Log("🎉 패턴 일치! YOU WIN!");
            GameSceneManager.Instance.OnGameClear();
        }

        return isMatched;
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
        new Color(1f, 0.5f, 0f), // orange
        Color.white
    };

        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return colorValues[i];
        }

        return Color.black; // 기본값
    }
    private string GetColorName(Color color)
    {
        string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
        Color[] colorValues = {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green,
        new Color(1f, 0.5f, 0f), // orange
        Color.white
    };

        for (int i = 0; i < colorValues.Length; i++)
        {
            if (ColorsApproximatelyEqual(color, colorValues[i]))
                return colorNames[i];
        }
        return "Unknown";
    }

}