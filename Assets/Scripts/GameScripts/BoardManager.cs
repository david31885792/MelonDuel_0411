using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("보드 설정")]
    public Transform boardParent;            // 타일이 들어갈 부모
    public GameObject tilePrefab;            // 타일 프리팹

    [Header("크기 및 간격")]
    public float tileSize = 160f;            // 타일 한 변 크기
    public float spacing = 10f;              // 타일 간격

    private int boardSize = 5;
    private Tile[,] tiles = new Tile[5, 5];
    private Vector2 startPos;

    private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    private int maxPerColor = 4;

    void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        // 시작 위치 계산: 중앙 정렬 기준 좌상단
        float boardWidth = boardSize * tileSize + (boardSize - 1) * spacing;
        startPos = new Vector2(-boardWidth / 2 + tileSize / 2, boardWidth / 2 - tileSize / 2);

        // 타일 이름 리스트 만들기
        List<string> tileNames = GenerateTileNames();

        // 빈 칸 위치 결정
        int emptyX = Random.Range(0, boardSize);
        int emptyY = Random.Range(0, boardSize);

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                // 빈칸이면 건너뜀
                if (row == emptyY && col == emptyX)
                    continue;

                // 타일 이름 가져오기
                string name = tileNames[0];
                tileNames.RemoveAt(0);

                // 생성 및 위치 지정
                GameObject go = Instantiate(tilePrefab, boardParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = GetTilePosition(row, col);
                rt.sizeDelta = new Vector2(tileSize, tileSize);

                Tile tile = go.GetComponent<Tile>();
                tile.Initialize(row, col, name);

                tiles[row, col] = tile;
            }
        }

        Debug.Log($"✅ Board generated with empty at ({emptyX}, {emptyY})");
    }

    private List<string> GenerateTileNames()
    {
        List<string> names = new List<string>();
        foreach (string color in colorNames)
        {
            for (int i = 0; i < maxPerColor; i++)
                names.Add(color);
        }

        // 랜덤 셔플
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
}
