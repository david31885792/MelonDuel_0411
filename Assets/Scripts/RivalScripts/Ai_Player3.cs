using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AI_Player3 : MonoBehaviour, IAIBehavior
{
    private RivalBoardManager board;
    private RivalPatternBoard pattern;
    private RivalGaugeManager gauge;
    private RivalSkillController skillCtrl;

    private Queue<Tile> moveQueue = new Queue<Tile>();
    private List<Tile> lastMovePath = new List<Tile>();
    private HashSet<string> visitedStates = new HashSet<string>();

    private float moveTimer = 0f;
    private float moveInterval = 0.2f;

    private float[,] priorityMap;

    //private int failCount = 0; // 클래스 멤버 변수로 추가

    private RivalScoreManager scoreManager;



    public void Initialize(RivalBoardManager board, RivalPatternBoard pattern, RivalGaugeManager gauge, RivalSkillController skillCtrl, RivalScoreManager scoreManager)
    {
        this.board = board;
        this.pattern = pattern;
        this.gauge = gauge;
        this.skillCtrl = skillCtrl;
        this.scoreManager = scoreManager;
        BuildPriorityMap();
        GeneratePriorityMap();

    }

    public void UpdateAI(float deltaTime)
    {
        moveTimer += deltaTime;

        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;

            if (moveQueue.Count > 0)
            {
                Tile tile = moveQueue.Peek(); // 일단 꺼내기만 함
                if (tile != null)
                {
                    bool moved = board.TryMoveTile(tile); // 이동 시도
                    if (moved)
                    {
                        moveQueue.Dequeue(); // 이동이 실제로 성공한 경우에만 제거
                        gauge?.AddGaugeForTileMove();
                    }
                }
            }
            else
            {
                MakeMoveDecision(); // 큐가 비어 있으면 새 결정
            }

            TryUseSkill(); // 스킬 조건 체크
        }
    }


    public void TryUseSkill()
    {
        if (gauge != null && gauge.IsGaugeFull())
        {
            Debug.Log("⚡ [AI] 스킬 사용 조건 충족 → 스킬 발동");
            skillCtrl?.UseSkill();
        }
    }

    public void MakeMoveDecision()
    {
        if (!board.CanMove()) return;

        Vector2Int emptyPos = board.GetEmptyTilePosition();
        List<Vector2Int> neighborPositions = board.GetAdjacentTilePositions(emptyPos);

        if (neighborPositions == null || neighborPositions.Count == 0)
        {
            Debug.LogWarning("[AI] 이동 가능한 타일 없음");
            return;
        }

        Vector2Int target = neighborPositions[UnityEngine.Random.Range(0, neighborPositions.Count)];
        board.TryMoveTile(target, 0.3f); // 빠른 이동

        gauge?.AddGaugeForTileMove();
    }






    private List<Tile> FindBestMoveSequence(int depth)
    {
        visitedStates.Clear();
        List<Tile> bestPath = null;
        float bestScore = float.MinValue;

        BacktrackRecursive(new List<Tile>(), depth, ref bestScore, ref bestPath);
        return bestPath;
    }

    private void BacktrackRecursive(List<Tile> path, int remainingDepth, ref float bestScore, ref List<Tile> bestPath)
    {
        string hash = board.GetBoardHash();
        if (visitedStates.Contains(hash))
            return;
        visitedStates.Add(hash);

        if (remainingDepth == 0)
        {
            float score = EvaluatePath(path); // ← 여기를 바꿈

            if (score > bestScore && !IsSamePath(path, lastMovePath))
            {
                bestScore = score;
                bestPath = new List<Tile>(path);
            }
            return;
        }

        foreach (var tile in board.GetMovableTiles())
        {
            if (path.Contains(tile)) continue;

            var backup = board.SimulateSwapClone(tile);
            path.Add(tile);

            BacktrackRecursive(path, remainingDepth - 1, ref bestScore, ref bestPath);

            path.RemoveAt(path.Count - 1);
            board.RestoreBoard(backup);
        }
    }



    private float EvaluateBoardState(TileData[,] tiles)
    {
        float score = 0f;
        float pattern = ScorePatternMatch(tiles);
        float center = ScoreCenterContribution(tiles);
        float variety = ScoreTileVariety(tiles);
        float priority = ScorePriorityTiles(tiles);

        score += pattern * 1.5f;
        score += center * 1.2f;
        score += variety * 0.3f;
        score += priority * 2.0f;

        // 보드 중심에 가까울수록 보너스
        foreach (var tile in lastMovePath)
        {
            Vector2Int pos = board.GetTilePosition(tile);
            float distToCenter = Vector2.Distance(pos, new Vector2(2, 2)); // 5x5 기준 중심 (2,2)
            score += (1f / (1f + distToCenter)) * 2f;
        }

        // 동일 타일 반복 방지
        if (lastMovePath.Count > 0 && lastMovePath.Count > 0 && lastMovePath[0] == lastMovePath[0])
        {
            score -= 3f; // 연속 같은 타일이면 패널티
        }


        Debug.Log($"[AI] 평가 점수 - 패턴:{pattern}, 중심:{center}, 다양성:{variety}, 우선순위:{priority} → 총합:{score:F2}");
        return score;
    }




    private bool IsSamePath(List<Tile> a, List<Tile> b)
    {
        if (a == null || b == null || a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    private List<Tile> GenerateMoveQueue_BFS(Vector2Int targetPosition)
    {
        var start = board.GetEmptyTilePosition();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == targetPosition)
                break;

            foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + dir;
                if (!board.IsInBounds(next) || visited.Contains(next)) continue;
                if (board.GetTileAt(next) == null) continue; // 무효 타일 제외

                queue.Enqueue(next);
                visited.Add(next);
                if (!cameFrom.ContainsKey(next))
                    cameFrom[next] = current;
            }
        }

        if (!cameFrom.ContainsKey(targetPosition)) return new List<Tile>(); // 경로 없음

        // 역추적
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int curr = targetPosition;
        while (curr != start)
        {
            path.Add(curr);
            curr = cameFrom[curr];
        }
        path.Reverse();

        return path.Select(pos => board.GetTileAt(pos)).Where(t => t != null).ToList();
    }

    private Vector2Int FindMostValuableTilePosition()
    {
        // 먼저 null 체크
        if (priorityMap == null)
        {
            Debug.LogWarning("[AI] priorityMap이 null입니다. GeneratePriorityMap() 재호출");
            GeneratePriorityMap();
        }

        if (pattern == null)
        {
            Debug.LogError("[AI] pattern이 null입니다.");
            return new Vector2Int(0, 0);
        }

        Vector2Int bestPos = Vector2Int.zero;
        float bestScore = float.MinValue;

        int h = pattern.GetHeight();
        int w = pattern.GetWidth();

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float priority = priorityMap[y, x];
                if (priority <= 0) continue;

                Vector2Int boardPos = pattern.GetBoardPositionFromPatternOffset(x, y);
                Tile current = board.GetTileAt(boardPos);
                Tile target = pattern.GetTileAtBoardPos(boardPos);

                if (current != null && target != null && current.GetTileNumber() != target.GetTileNumber())
                {
                    if (priority > bestScore)
                    {
                        bestScore = priority;
                        bestPos = boardPos;
                    }
                }
            }
        }

        return bestPos;
    }




    public void MakeGreedyDecision()
    {
        float bestScore = float.MinValue;
        Tile bestTile = null;

        foreach (var tile in board.GetMovableTiles())
        {
            var simBoard = board.SimulateSwapClone(tile);
            float score = EvaluateBoardState(simBoard);
            board.RestoreBoard(simBoard);

            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }

        if (bestTile != null)
        {
            moveQueue.Enqueue(bestTile);
            Debug.Log($"[Greedy] 선택된 타일: {bestTile.name}, 예상 점수: {bestScore:F2}");
        }
    }
    private List<Tile> FindBestMoveSequence_BacktrackingGreedy(int depthLimit)
    {
        List<Tile> path = new List<Tile>();
        float bestScore = float.MinValue;
        List<Tile> bestPath = null;

        visitedStates.Clear();
        BacktrackRecursive(path, depthLimit, ref bestScore, ref bestPath);
        return bestPath;
    }

    private int ScorePatternMatch(TileData[,] tiles)
    {
        int matchCount = 0;
        for (int y = 0; y < pattern.GetHeight(); y++)
        {
            for (int x = 0; x < pattern.GetWidth(); x++)
            {
                Vector2Int pos = pattern.GetBoardPositionFromPatternOffset(x, y);
                Tile pt = pattern.GetPatternTile(x, y);
                TileData bt = tiles[pos.y, pos.x];

                if (pt != null && pt.GetTileNumber() == bt.TileNumber)
                    matchCount++;
            }
        }
        return matchCount;
    }


    private int ScoreCenterContribution(TileData[,] tiles)
    {
        int score = 0;
        int midX = tiles.GetLength(1) / 2;
        int midY = tiles.GetLength(0) / 2;

        for (int y = midY - 1; y <= midY + 1; y++)
        {
            for (int x = midX - 1; x <= midX + 1; x++)
            {
                if (x < 0 || y < 0 || y >= tiles.GetLength(0) || x >= tiles.GetLength(1))
                    continue;

                Vector2Int pos = new Vector2Int(x, y);
                TileData tile = tiles[y, x];
                Tile pt = pattern.GetTileAtBoardPos(pos);

                if (tile.TileNumber > 0 && pt != null && tile.TileNumber == pt.GetTileNumber())
                    score++;
            }
        }

        return score;
    }


    private int ScoreTileVariety(TileData[,] tiles)
    {
        HashSet<int> unique = new HashSet<int>();
        for (int y = 0; y < tiles.GetLength(0); y++)
        {
            for (int x = 0; x < tiles.GetLength(1); x++)
            {
                TileData t = tiles[y, x];
                if (t.TileNumber > 0) unique.Add(t.TileNumber);
            }
        }
        return unique.Count;
    }


    private int ScorePriorityTiles(TileData[,] tiles)
    {
        int score = 0;
        for (int y = 0; y < priorityMap.GetLength(0); y++)
        {
            for (int x = 0; x < priorityMap.GetLength(1); x++)
            {
                float priority = priorityMap[y, x];
                Vector2Int pos = new Vector2Int(x, y);

                Tile patternTile = pattern.GetTileAtBoardPos(pos);
                TileData currentTile = tiles[y, x];

                if (patternTile != null && currentTile.TileNumber > 0 &&
                    patternTile.GetTileNumber() == currentTile.TileNumber)
                {
                    score += (int)priority;
                }
            }
        }

        return score;
    }


    private void BuildPriorityMap()
    {
        int boardWidth = board.GetWidth();
        int boardHeight = board.GetHeight();

        if (priorityMap == null || priorityMap.GetLength(0) != boardHeight || priorityMap.GetLength(1) != boardWidth)
        {
            priorityMap = new float[boardHeight, boardWidth];
        }
        else
        {
            Array.Clear(priorityMap, 0, priorityMap.Length);
        }

        Vector2Int center = new Vector2Int(pattern.GetWidth() / 2, pattern.GetHeight() / 2);

        for (int y = 0; y < pattern.GetHeight(); y++)
        {
            for (int x = 0; x < pattern.GetWidth(); x++)
            {
                Tile tile = pattern.GetPatternTile(x, y);
                if (tile == null) continue;

                int dist = Mathf.Abs(x - center.x) + Mathf.Abs(y - center.y);
                int weight = 10 - dist;

                Vector2Int boardPos = pattern.GetBoardPositionFromPatternOffset(x, y);

                if (board.IsInBounds(boardPos))
                {
                    priorityMap[boardPos.y, boardPos.x] = weight;
                }
            }
        }
    }


    private void GeneratePriorityMap()
    {
        int w = pattern.GetWidth();
        int h = pattern.GetHeight();
        priorityMap = new float[h, w];
        Vector2 center = new Vector2(w / 2f, h / 2f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Tile patternTile = pattern.GetPatternTile(x, y);
                if (patternTile == null) continue;

                float dist = Vector2.Distance(center, new Vector2(x, y));
                priorityMap[y, x] = 1f / (1f + dist);
            }
        }
    }

    private float EvaluatePath(List<Tile> path)
    {
        if (path == null || path.Count == 0) return 0f;

        float score = 0f;

        foreach (var tile in path)
        {
            if (tile == null) continue;

            Vector2Int tilePos = board.GetTilePosition(tile);
            int tileNumber = tile.GetTileNumber(); // ✅ 수정
            Tile patternTile = pattern.GetTileAtBoardPos(tilePos); // ✅ 수정

            int patternNumber = patternTile != null ? patternTile.GetTileNumber() : -1;

            // 패턴과 일치하는 타일 보너스
            if (tileNumber == patternNumber && tileNumber > 0)
                score += 5f;

            // 중심 타일일수록 보너스
            float distToCenter = Vector2.Distance(tilePos, new Vector2(2, 2));
            score += (1f / (1f + distToCenter)) * 2f;

            // 빈 칸 근처 이동 시 약간의 보너스
            if (Vector2Int.Distance(tilePos, board.GetEmptyTilePosition()) <= 1f) // ✅ 수정
                score += 1f;
        }

        // 중복 타일 선택 패널티
        if (path.Count > 0 && lastMovePath.Count > 0 && path[0] == lastMovePath[0])
            score -= 3f;

        return score;
    }


}
