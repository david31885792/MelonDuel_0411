using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform boardParent;
    public Vector2 tileSpacing = new Vector2(100, 100);

    public event Action OnTileMoved;
    public event Action OnClearMatched;

    private Tile[,] tiles = new Tile[5, 5];
    private Vector2Int emptyPos;

    private string[,] clearPattern;

    [Header("색상 이름 매핑")]
    public List<string> colorNames;
    public List<Color> colorValues;
    private Dictionary<string, Color> colorMap = new Dictionary<string, Color>();

    public void InitBoard()
    {
        InitColorMap();
        clearPattern = PlayerPrefsPatternLoader.LoadPattern();
        CreateBoard();
    }

    private void InitColorMap()
    {
        for (int i = 0; i < Mathf.Min(colorNames.Count, colorValues.Count); i++)
            colorMap[colorNames[i]] = colorValues[i];
    }

    private void CreateBoard()
    {
        List<string> pool = new List<string>();
        foreach (var name in colorMap.Keys)
            for (int i = 0; i < 4; i++) pool.Add(name); // 6종 * 4 = 24

        // 무작위 셔플
        for (int i = 0; i < pool.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        emptyPos = new Vector2Int(2, 2); // 중앙 비움

        int index = 0;
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (x == emptyPos.x && y == emptyPos.y) continue;

                GameObject go = Instantiate(tilePrefab, boardParent);
                go.transform.localPosition = new Vector3((x - 2) * tileSpacing.x, (2 - y) * tileSpacing.y, 0);

                Tile tile = go.GetComponent<Tile>();
                string name = pool[index++];
                Color color = colorMap[name];

                tile.Init(this, x, y, name, color);
                tiles[x, y] = tile;
            }
        }
    }

    public void OnTileClicked(Tile clicked)
    {
        Vector2Int pos = new Vector2Int(clicked.x, clicked.y);

        if (pos.x != emptyPos.x && pos.y != emptyPos.y) return;

        // 슬라이딩 방향 계산
        int dx = Math.Sign(emptyPos.x - pos.x);
        int dy = Math.Sign(emptyPos.y - pos.y);

        if (dx != 0)
        {
            int start = dx > 0 ? pos.x : emptyPos.x + 1;
            for (int x = emptyPos.x - dx; x != pos.x - dx; x -= dx)
                SlideTile(new Vector2Int(x, pos.y), new Vector2Int(x + dx, pos.y));
        }
        else if (dy != 0)
        {
            int start = dy > 0 ? pos.y : emptyPos.y + 1;
            for (int y = emptyPos.y - dy; y != pos.y - dy; y -= dy)
                SlideTile(new Vector2Int(pos.x, y), new Vector2Int(pos.x, y + dy));
        }

        emptyPos = pos;
        OnTileMoved?.Invoke();

        if (CheckClear())
        {
            OnClearMatched?.Invoke();
        }
    }

    private void SlideTile(Vector2Int from, Vector2Int to)
    {
        Tile tile = tiles[from.x, from.y];
        if (tile == null) return;

        tile.x = to.x;
        tile.y = to.y;
        tile.transform.localPosition = new Vector3((to.x - 2) * tileSpacing.x, (2 - to.y) * tileSpacing.y, 0);

        tiles[to.x, to.y] = tile;
        tiles[from.x, from.y] = null;
    }

    private bool CheckClear()
    {
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
            {
                Tile tile = tiles[x + 1, y + 1];
                if (tile == null || tile.tileName != clearPattern[y, x]) return false;
            }

        return true;
    }
}
