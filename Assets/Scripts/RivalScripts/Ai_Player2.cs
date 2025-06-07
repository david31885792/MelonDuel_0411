using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 캐릭터 2 (시로코) – 블라인드 스킬 사용
/// </summary>
public class AI_Player2 : MonoBehaviour, IAIBehavior
{
    private RivalBoardManager boardManager;
    private RivalPatternBoard patternBoard;
    private RivalSkillController skillController;
    private RivalGaugeManager gaugeManager;

    private float moveDelayTimer = 0f;
    private float moveDelayMin = 0.21f;
    private float moveDelayMax = 0.21f;

    private bool isReady = false;

    public void Initialize(RivalBoardManager boardManager, RivalPatternBoard patternBoard, RivalSkillController skillController)
    {
        this.boardManager = boardManager;
        this.patternBoard = patternBoard;
        this.skillController = skillController;
        this.gaugeManager = skillController.GetGaugeManager(); // 🔌 게이지 매니저 연결

        isReady = true;
        ResetMoveTimer();
    }

    public void UpdateAI(float deltaTime)
    {
        if (!isReady) return;

        moveDelayTimer -= deltaTime;
        if (moveDelayTimer <= 0f)
        {
            MakeMoveDecision();
            ResetMoveTimer();
        }

        TryUseSkill();
    }

    private void ResetMoveTimer()
    {
        moveDelayTimer = Random.Range(moveDelayMin, moveDelayMax);
    }

    public void MakeMoveDecision()
    {
        Tile[,] tiles = boardManager.GetTiles();
        Vector2 emptyPos = boardManager.GetEmptyTilePosition();

        List<Tile> movableTiles = GetAdjacentTiles(tiles, emptyPos);
        if (movableTiles.Count == 0) return;

        Tile chosenTile;
        if (Random.value < 0.1f)
        {
            chosenTile = GetRandomNonAdjacentTile(tiles, emptyPos, movableTiles);
            Debug.Log("😵 [AI] 실수로 잘못된 타일 선택");
        }
        else
        {
            chosenTile = movableTiles[Random.Range(0, movableTiles.Count)];
            Debug.Log("🧠 [AI] 정상 타일 이동");
        }

        if (boardManager.TryMoveTile(chosenTile))
        {
            gaugeManager?.AddGaugeForTileMove(); // ✅ 게이지 충전
            Debug.Log("⚡ [AI] 타일 이동 후 게이지 충전");
        }
    }

    public void TryUseSkill()
    {
        // 현재는 자동 발동 구조이므로 생략 가능
        // 조건부 전략적 발동 시 여기에 로직 추가
    }

    private List<Tile> GetAdjacentTiles(Tile[,] tiles, Vector2 emptyPos)
    {
        List<Tile> result = new List<Tile>();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int center = Vector2Int.RoundToInt(emptyPos);

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int pos = center + dir;
            if (pos.x >= 0 && pos.x < 5 && pos.y >= 0 && pos.y < 5)
            {
                Tile t = tiles[pos.y, pos.x];
                if (t != null) result.Add(t);
            }
        }

        return result;
    }

    private Tile GetRandomNonAdjacentTile(Tile[,] tiles, Vector2 emptyPos, List<Tile> exclude)
    {
        List<Tile> all = new List<Tile>();

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Tile t = tiles[y, x];
                if (t != null && !exclude.Contains(t))
                {
                    all.Add(t);
                }
            }
        }

        return all.Count > 0 ? all[Random.Range(0, all.Count)] : exclude[0];
    }
}
