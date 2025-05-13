using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public GameObject tilePrefab;
    public RectTransform boardParent;
    public float spacing = 4f;
    public Color[] colorValues; // 1~6 색상 매핑

    [Header("Tile Movement")]
    public float moveDuration = 0.2f;
    public Ease moveEase = Ease.InOutSine;

    public static BoardManager Instance { get; private set; }

    private Tile[,] tiles = new Tile[5, 5];
    private Vector2 emptyTilePos = new Vector2(4, 4);

    private PatternPanel patternPanel;
    private int[,] clearPattern = new int[3, 3];

    private void Awake()
    {
        Instance = this;
        patternPanel = FindFirstObjectByType<PatternPanel>();
    }

    private IEnumerator Start()
    {
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

    private void GenerateBoard()
    {
        List<int> numbers = new List<int>();
        for (int i = 1; i <= 6; i++)
        {
            for (int j = 0; j < 4; j++)
                numbers.Add(i);
        }

        // Fisher-Yates Shuffle
        System.Random rng = new System.Random();
        int n = numbers.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (numbers[n], numbers[k]) = (numbers[k], numbers[n]);
        }

        int numberIndex = 0;

        float tileSize = 160f; // ✅ 타일 크기 고정
        Vector2 start = new Vector2(-2 * (tileSize + spacing), 2 * (tileSize + spacing)); // 위치 기준 수정 없음

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (x == 4 && y == 4)
                {
                    tiles[y, x] = null;
                    continue;
                }

                GameObject tileGO = Instantiate(tilePrefab, boardParent);
                RectTransform rt = tileGO.GetComponent<RectTransform>();

                rt.sizeDelta = new Vector2(tileSize, tileSize); // ✅ 고정 크기 적용
                rt.anchoredPosition = start + new Vector2(x * (tileSize + spacing), -y * (tileSize + spacing));

                Tile tile = tileGO.GetComponent<Tile>();
                int tileNumber = numbers[numberIndex];
                tile.SetTMPNumber(tileNumber);

                if (tileNumber >= 1 && tileNumber <= colorValues.Length)
                    tile.SetColor(colorValues[tileNumber - 1]);
                else
                    tile.SetColor(Color.black);

                tiles[y, x] = tile;
                numberIndex++;
            }
        }

        emptyTilePos = new Vector2(4, 4);
    }


    public void TryMoveTile(Tile tile)
    {
        Vector2Int from = GetTilePosition(tile);
        Vector2Int to = Vector2Int.RoundToInt(emptyTilePos);

        if (Vector2Int.Distance(from, to) != 1)
            return;

        RectTransform rt = tile.GetComponent<RectTransform>();
        Vector2 anchoredStartPos = rt.anchoredPosition;

        rt.DOAnchorPos(GetAnchoredPosition(to), moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
                tiles[to.y, to.x] = tile;
                tiles[from.y, from.x] = null;
                emptyTilePos = from;
                IsPatternMatched();
            });
    }

    private Vector2Int GetTilePosition(Tile tile)
    {
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                if (tiles[y, x] == tile)
                    return new Vector2Int(x, y);
        return Vector2Int.zero;
    }

    private Vector2 GetAnchoredPosition(Vector2Int boardPos)
    {
        float tileSize = 160f; // ✅ 타일 고정 크기
        Vector2 start = new Vector2(-2 * (tileSize + spacing), 2 * (tileSize + spacing)); // 시작 위치 계산
        return start + new Vector2(boardPos.x * (tileSize + spacing), -boardPos.y * (tileSize + spacing));
    }


    private bool IsPatternMatched()
    {
        for (int row = 1; row <= 3; row++)
        {
            for (int col = 1; col <= 3; col++)
            {
                Tile tile = tiles[row, col];
                if (tile == null || tile.tmpText == null)
                    return false;

                int gameNum = int.Parse(tile.tmpText.text);
                int expected = clearPattern[row - 1, col - 1];

                if (gameNum != expected)
                {
                    Debug.Log($"❌ 불일치: tile[{row},{col}] = {gameNum}, 기대: {expected}");
                    return false;
                }
            }
        }

        Debug.Log("✅ 클리어 패턴과 완벽히 일치!");
        GameSceneManager.Instance?.OnGameClear();
        return true;
    }
}
