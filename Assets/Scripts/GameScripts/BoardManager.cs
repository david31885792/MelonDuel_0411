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
    public Color[] colorValues;

    [Header("Tile Layout")]
    public float tileSize = 160f; // 🔹 인스펙터 조절 가능

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

        // 자동 보드 생성 제거 → GameSceneManager에서 호출함
    }

    public void ShuffleBoard()
    {
        foreach (Transform child in boardParent)
            Destroy(child.gameObject);

        UpdateClearPattern();
        GenerateBoard();

        // 🎯 클리어 직후 완성도 점수 초기화/계산
        IsPatternMatched();
    }

    public void UpdateClearPattern()
    {
        if (patternPanel == null) return;

        int[] flatPattern = patternPanel.GetPattern();
        for (int i = 0; i < 9; i++)
        {
            clearPattern[i / 3, i % 3] = flatPattern[i];
        }
    }

    private void GenerateBoard()
    {
        List<int> numbers = new List<int>();
        for (int i = 1; i <= 6; i++)
        {
            for (int j = 0; j < 4; j++)
                numbers.Add(i);
        }

        System.Random rng = new System.Random();
        int n = numbers.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (numbers[n], numbers[k]) = (numbers[k], numbers[n]);
        }

        int numberIndex = 0;
        Vector2 start = new Vector2(-2 * (this.tileSize + spacing), 2 * (this.tileSize + spacing));

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
                rt.sizeDelta = new Vector2(this.tileSize, this.tileSize);
                rt.anchoredPosition = start + new Vector2(x * (this.tileSize + spacing), -y * (this.tileSize + spacing));

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

            AudioManager.Instance?.PlaySFX(AudioManager.Instance.moveTileClip);

            SkillGaugeManager.Instance?.AddGaugeForTileMove();

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
        Vector2 start = new Vector2(-2 * (this.tileSize + spacing), 2 * (this.tileSize + spacing));
        return start + new Vector2(boardPos.x * (this.tileSize + spacing), -boardPos.y * (this.tileSize + spacing));
    }

    private bool IsPatternMatched()
    {
        int matchCount = 0;

        for (int row = 1; row <= 3; row++)
        {
            for (int col = 1; col <= 3; col++)
            {
                Tile tile = tiles[row, col];
                if (tile == null || tile.tmpText == null)
                    continue;

                int gameNum = int.Parse(tile.tmpText.text);
                int expected = clearPattern[row - 1, col - 1];

                if (gameNum == expected)
                {
                    matchCount++;
                }
            }
        }

        GameSceneManager.Instance?.UpdateMatchCount(matchCount);

        if (matchCount == 9)
        {
            Debug.Log("✅ 클리어 패턴과 완벽히 일치!");
            GameSceneManager.Instance?.OnPatternMatched(matchCount);
            return true;
        }

        return false;
    }
}
