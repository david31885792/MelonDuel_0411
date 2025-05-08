using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [Header("보드 설정")]
    public Transform boardParent;            // 타일이 들어갈 부모
    public GameObject tilePrefab;            // 타일 프리팹

    [Header("크기 및 간격")]
    public float tileSize = 160f;            // 타일 한 변 크기
    public float spacing = 10f;              // 타일 간격

    [Header("애니메이션")]
    public float moveDuration = 0.2f; // 이동 연출 시간


    private int boardSize = 5;
    private Tile[,] tiles = new Tile[5, 5];
    private Vector2 startPos;

    private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    private int maxPerColor = 4;

    private int blankX;
    private int blankY;

    // ✅ 싱글톤 초기화
    private void Awake()
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
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        float boardWidth = boardSize * tileSize + (boardSize - 1) * spacing;
        startPos = new Vector2(-boardWidth / 2 + tileSize / 2, boardWidth / 2 - tileSize / 2);

        List<string> tileNames = GenerateTileNames();

        blankX = Random.Range(0, boardSize);
        blankY = Random.Range(0, boardSize);

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (row == blankY && col == blankX)
                    continue;

                string name = tileNames[0];
                tileNames.RemoveAt(0);

                GameObject go = Instantiate(tilePrefab, boardParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = GetTilePosition(row, col);
                rt.sizeDelta = new Vector2(tileSize, tileSize);

                Tile tile = go.GetComponent<Tile>();
                tile.Initialize(row, col, name);

                tiles[row, col] = tile;

                Debug.Log($"🔷 타일 생성: {name} at ({row}, {col})");

            }
        }

        Debug.Log($"✅ Board generated with empty at ({blankX}, {blankY})");
        Debug.LogWarning("⚠️ GenerateBoard 호출됨!");
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
        return new Vector2(x, y);
    }

    public void TryMoveTile(Tile tile)
    {
        if (!IsAdjacentToBlank(tile.x, tile.y)) return;

        SwapTileWithBlank(tile);
        UpdateTilePositions();
    }

    private bool IsAdjacentToBlank(int x, int y)
    {
        int dx = Mathf.Abs(blankX - x);
        int dy = Mathf.Abs(blankY - y);
        return (dx + dy) == 1;
    }

    private void SwapTileWithBlank(Tile tile)
    {
        int tileX = tile.x;
        int tileY = tile.y;

        tiles[blankY, blankX] = tile;
        tiles[tileY, tileX] = null;

        tile.x = blankX;
        tile.y = blankY;

        blankX = tileX;
        blankY = tileY;
    }

    private void UpdateTilePositions()
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                Tile tile = tiles[row, col];
                if (tile != null)
                {
                    RectTransform rt = tile.GetComponent<RectTransform>();
                    Vector2 targetPos = GetTilePosition(row, col);
                    rt.DOAnchorPos(targetPos, moveDuration).SetEase(Ease.OutQuad); // 💫 부드러운 이동
                }
            }
        }
    }

}
