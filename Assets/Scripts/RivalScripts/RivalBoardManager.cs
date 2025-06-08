using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class RivalBoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public GameObject tilePrefab;
    public RectTransform boardParent;
    public float spacing = 4f;
    public Color[] colorValues;
    public float tileSize = 160f;

    [Header("Tile Movement")]
    public float moveDuration = 0.2f;
    public Ease moveEase = Ease.InOutSine;
    public bool useTileAnimation = true;

    public int GetWidth() => tiles.GetLength(1);
    public int GetHeight() => tiles.GetLength(0);


    private Tile[,] tiles = new Tile[5, 5];
    private Vector2 emptyTilePos = new Vector2(4, 4);
    private int[,] clearPattern = new int[3, 3];

    private RivalPatternBoard patternBoard;

    public void SetMoveAnimation(bool use)
    {
        useTileAnimation = use;
    }

    public void SetPatternBoard(RivalPatternBoard pattern)
    {
        this.patternBoard = pattern;
        UpdateClearPattern();
    }

    public void GenerateBoard()
    {
        foreach (Transform child in boardParent)
            Destroy(child.gameObject);

        List<int> numbers = new List<int>();
        for (int i = 1; i <= 6; i++)
            for (int j = 0; j < 4; j++)
                numbers.Add(i);

        numbers.Shuffle();

        Vector2 start = new Vector2(-2 * (tileSize + spacing), 2 * (tileSize + spacing));
        int index = 0;

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (x == 4 && y == 4)
                {
                    tiles[y, x] = null;
                    continue;
                }

                GameObject go = Instantiate(tilePrefab, boardParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileSize, tileSize);
                rt.anchoredPosition = start + new Vector2(x * (tileSize + spacing), -y * (tileSize + spacing));

                Tile tile = go.GetComponent<Tile>();
                int value = numbers[index++];
                tile.SetTMPNumber(value);
                tile.SetColor(colorValues[value - 1]);

                tiles[y, x] = tile;
            }
        }

        emptyTilePos = new Vector2(4, 4);
    }

    public void UpdateClearPattern()
    {
        if (patternBoard == null) return;

        int[] flat = patternBoard.GetPattern();
        for (int i = 0; i < 9; i++)
        {
            clearPattern[i / 3, i % 3] = flat[i];
        }
    }

    public bool TryMoveTile(Tile tile)
    {
        Vector2Int from = GetTilePosition(tile);
        Vector2Int to = Vector2Int.RoundToInt(emptyTilePos);

        // 이동 불가능한 위치면 false 반환
        if (Vector2Int.Distance(from, to) != 1)
            return false;

        RectTransform rt = tile.GetComponent<RectTransform>();

        if (useTileAnimation)
        {
            rt.DOAnchorPos(GetAnchoredPosition(to), moveDuration)
              .SetEase(moveEase)
              .OnComplete(() =>
              {
                  tiles[to.y, to.x] = tile;
                  tiles[from.y, from.x] = null;
                  emptyTilePos = from;

                  CheckPatternMatch(); // 내부 패턴 검사
              });
        }
        else
        {
            rt.anchoredPosition = GetAnchoredPosition(to);
            tiles[to.y, to.x] = tile;
            tiles[from.y, from.x] = null;
            emptyTilePos = from;

            CheckPatternMatch();
        }

        return true; // 이동 성공
    }


    public Vector2Int GetTilePosition(Tile tile)
    {
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                if (tiles[y, x] == tile)
                    return new Vector2Int(x, y);
        return Vector2Int.zero;
    }

    private Vector2 GetAnchoredPosition(Vector2Int pos)
    {
        Vector2 start = new Vector2(-2 * (tileSize + spacing), 2 * (tileSize + spacing));
        return start + new Vector2(pos.x * (tileSize + spacing), -pos.y * (tileSize + spacing));
    }

    private void CheckPatternMatch()
    {
        int matchCount = 0;

        for (int row = 1; row <= 3; row++)
        {
            for (int col = 1; col <= 3; col++)
            {
                Tile tile = tiles[row, col];
                if (tile == null || tile.tmpText == null)
                    continue;

                if (tile.isWildTile)
                {
                    matchCount++;
                    continue;
                }

                if (!int.TryParse(tile.tmpText.text, out int gameNum)) continue;
                int expected = clearPattern[row - 1, col - 1];

                if (gameNum == expected)
                    matchCount++;
            }
        }

        //Debug.Log($"[AI] 패턴 매치 수: {matchCount}");

        // TODO: AI가 이 값을 바탕으로 스스로 판단 (ex. 패턴 완성 시 처리)
        // currentAI.OnPatternMatched(matchCount); 같은 방식
    }

    public void ConvertRandomTilesToWild(int count)
    {
        List<Tile> candidates = new List<Tile>();
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                if (tiles[y, x] != null && !tiles[y, x].isWildTile)
                    candidates.Add(tiles[y, x]);

        if (candidates.Count == 0) return;

        candidates.Shuffle();

        int convertCount = Mathf.Min(count, candidates.Count);
        for (int i = 0; i < convertCount; i++)
        {
            candidates[i].SetAsWildTile();
        }

        Debug.Log($"[AI] {convertCount}개의 타일이 와일드로 변환됨");
    }

    public Tile[,] GetTiles() => tiles;

    public Vector2Int GetEmptyTilePosition()
    {
        return Vector2Int.RoundToInt(emptyTilePos);
    }


    // ... 기존 코드 유지 ...

    /// <summary>
    /// 외부에서 호출 가능한 보드 생성 함수 (AIController 전용)
    /// </summary>
    public void GenerateBoardTiles()
    {
        GenerateBoard();
    }

    public Tile GetTileAtPatternRegion(int x, int y)
    {
        // 패턴은 중앙 정렬이므로 보드 기준 (1,1) ~ (3,3)
        return tiles[y + 1, x + 1]; // 보드 인덱스 변환
    }

    public List<Tile> GetAdjacentMovableTiles()
    {
        List<Tile> movableTiles = new List<Tile>();

        Vector2Int emptyPos = GetEmptyTilePosition();
        Vector2Int[] directions = new Vector2Int[]
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = emptyPos + dir;
            if (IsInBounds(neighborPos))
            {
                Tile tile = tiles[neighborPos.y, neighborPos.x];
                if (tile != null)
                    movableTiles.Add(tile);
            }
        }

        return movableTiles;
    }


    public Tile GetRandomMovableTile()
    {
        List<Tile> movableTiles = GetAdjacentMovableTiles();
        if (movableTiles != null && movableTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, movableTiles.Count);
            return movableTiles[randomIndex];
        }
        return null;
    }


    public Tile[,] SimulateSwap(Tile tile)
    {
        // Create a copy of the current tiles array
        Tile[,] simulatedTiles = (Tile[,])tiles.Clone();

        // Get the position of the tile to be swapped
        Vector2Int tilePos = GetTilePosition(tile);

        // Get the position of the empty tile
        Vector2Int emptyPos = new Vector2Int((int)emptyTilePos.x, (int)emptyTilePos.y);

        // Perform the swap in the simulated tiles array
        Tile temp = simulatedTiles[tilePos.y, tilePos.x];
        simulatedTiles[tilePos.y, tilePos.x] = simulatedTiles[emptyPos.y, emptyPos.x];
        simulatedTiles[emptyPos.y, emptyPos.x] = temp;

        return simulatedTiles;
    }

    public bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < GetWidth() && position.y >= 0 && position.y < GetHeight();
    }

    public Tile GetTileAt(Vector2Int pos)
    {
        if (!IsInBounds(pos)) return null;
        return tiles[pos.y, pos.x];
    }

    public List<Tile> GetMovableTiles()
    {
        List<Tile> movableTiles = new List<Tile>();
        Vector2Int emptyPos = GetEmptyTilePosition();

        Vector2Int[] directions = new Vector2Int[]
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int check = emptyPos + dir;
            if (IsInBounds(check))
            {
                Tile tile = GetTileAt(check);
                if (tile != null)
                    movableTiles.Add(tile);
            }
        }

        return movableTiles;
    }

    public string GetBoardHash()
    {
        string hash = "";
        for (int y = 0; y < GetHeight(); y++)
        {
            for (int x = 0; x < GetWidth(); x++)
            {
                Tile tile = GetTileAt(new Vector2Int(x, y));
                if (tile != null)
                    hash += tile.GetTileNumber().ToString();
                else
                    hash += "0";
            }
        }
        return hash;
    }

    public TileData[,] SimulateSwapClone(Tile tile)
    {
        TileData[,] backup = SaveBoardData();
        TryMoveTile(tile); // 실제 이동
        return backup;
    }

    public void RestoreBoard(TileData[,] backup)
    {
        if (backup == null)
        {
            Debug.LogWarning("❗ RestoreBoard 실패: 백업 데이터가 null");
            return;
        }

        LoadBoardData(backup);
    }


    public TileData[,] SaveBoardData()
    {
        int height = tiles.GetLength(0);
        int width = tiles.GetLength(1);
        TileData[,] snapshot = new TileData[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                snapshot[y, x] = tiles[y, x] != null ? tiles[y, x].ToData() : new TileData(-1, "NULL");
            }
        }

        return snapshot;
    }

    public void LoadBoardData(TileData[,] snapshot)
    {
        int height = tiles.GetLength(0);
        int width = tiles.GetLength(1);

        // 🔥 기존 타일 오브젝트 전부 파괴
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (tiles[y, x] != null)
                {
                    Destroy(tiles[y, x].gameObject);
                    tiles[y, x] = null;
                }
            }
        }

        // 📦 새 타일 데이터로 보드 구성
        for (int y = 0; y < snapshot.GetLength(0); y++)
        {
            for (int x = 0; x < snapshot.GetLength(1); x++)
            {
                TileData data = snapshot[y, x];
                if (data.TileNumber > 0)
                {
                    CreateTileFromData(x, y, data); // 타일 GameObject 생성
                }
                else
                {
                    tiles[y, x] = null;

                    // 빈 칸이면 emptyTilePos 갱신
                    emptyTilePos = new Vector2Int(x, y);
                }
            }
        }
    }


    public Tile CreateTileFromData(TileData data)
    {
        GameObject obj = Instantiate(tilePrefab); // tilePrefab 연결 필요
        Tile tile = obj.GetComponent<Tile>();
        tile.SetTileNumber(data.number);
        tile.name = data.name;
        return tile;
    }
    private void CreateTileFromData(int x, int y, TileData data)
    {
        GameObject obj = Instantiate(tilePrefab, boardParent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(tileSize, tileSize);
        rt.anchoredPosition = GetAnchoredPosition(new Vector2Int(x, y));

        Tile tile = obj.GetComponent<Tile>();
        tile.SetTMPNumber(data.TileNumber);

        if (data.TileNumber > 0 && data.TileNumber <= colorValues.Length)
            tile.SetColor(colorValues[data.TileNumber - 1]);

        tiles[y, x] = tile;
    }

    public TileData[,] GetTileDataClone()
    {
        int w = GetWidth();
        int h = GetHeight();
        TileData[,] clone = new TileData[h, w];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Tile t = GetTileAt(new Vector2Int(x, y));
                if (t != null)
                    clone[y, x] = new TileData(t.GetTileNumber(), t.name);

            }
        }

        return clone;
    }

}
