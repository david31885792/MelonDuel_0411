using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ChallengeBoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public GameObject tilePrefab;
    public RectTransform boardParent;
    public float spacing = 4f;
    public Color[] colorValues;

    [Header("Tile Movement")]
    public float moveDuration = 0.2f;
    public Ease moveEase = Ease.InOutSine;

    public static ChallengeBoardManager Instance { get; private set; }

    private Tile[,] tiles = new Tile[5, 5];
    private Vector2 emptyTilePos = new Vector2(4, 4);

    private int[,] currentPattern = new int[3, 3];

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateBoard();
        UpdatePattern(); // 첫 패턴 로딩
    }

    public void UpdatePattern()
    {
        int[] flatPattern = ChallengePatternPanel.Instance.GetPattern();
        for (int i = 0; i < 9; i++)
            currentPattern[i / 3, i % 3] = flatPattern[i];
    }

    private void GenerateBoard()
    {
        List<int> numbers = new List<int>();
        for (int i = 1; i <= 6; i++)
            for (int j = 0; j < 4; j++)
                numbers.Add(i);

        System.Random rng = new System.Random();
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }

        int numberIndex = 0;
        float tileSize = 160f;
        Vector2 start = new Vector2(-2 * (tileSize + spacing), 2 * (tileSize + spacing));

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
                rt.sizeDelta = new Vector2(tileSize, tileSize);
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

        if (Vector2Int.Distance(from, to) != 1) return;

        RectTransform rt = tile.GetComponent<RectTransform>();
        rt.DOAnchorPos(GetAnchoredPosition(to), moveDuration)
          .SetEase(moveEase)
          .OnComplete(() =>
          {
              tiles[to.y, to.x] = tile;
              tiles[from.y, from.x] = null;
              emptyTilePos = from;

              AudioManager.Instance?.PlaySFX(AudioManager.Instance.moveTileClip);

              CheckPattern();
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
        float tileSize = 160f;
        Vector2 start = new Vector2(-2 * (tileSize + spacing), 2 * (tileSize + spacing));
        return start + new Vector2(boardPos.x * (tileSize + spacing), -boardPos.y * (tileSize + spacing));
    }

    private void CheckPattern()
    {
        int matchCount = 0;

        for (int row = 1; row <= 3; row++)
        {
            for (int col = 1; col <= 3; col++)
            {
                Tile tile = tiles[row, col];
                if (tile == null || tile.tmpText == null) continue;

                int gameNum = int.Parse(tile.tmpText.text);
                int expected = currentPattern[row - 1, col - 1];

                if (gameNum == expected)
                    matchCount++;
            }
        }

        ChallengeSceneManager.Instance?.UpdateMatchCount(matchCount);

        if (matchCount == 9)
        {
            ChallengeSceneManager.Instance?.OnPatternMatched();
            UpdatePattern(); // 새로운 패턴 로드
        }
    }
}
