using UnityEngine;

/// <summary>
/// AI 캐릭터 제어 및 루프 실행 담당
/// </summary>
public class AIController : MonoBehaviour
{
    [Header("AI 캐릭터 선택")]
    [Tooltip("0: Player1, 1: Player2, 2: Player3")]
    public int selectedCharacterId = 0;

    [Header("AI 시스템 연결")]
    public RivalBoardManager boardManager;
    public RivalPatternBoard patternBoard;
    public RivalSkillController skillController;

    private IAIBehavior currentAI;

    void Start()
    {
        // ✅ 보드 및 패턴 초기화 추가
        if (boardManager != null)
        {
            boardManager.GenerateBoardTiles();
        }
        else
        {
            Debug.LogError("AIController: RivalBoardManager가 연결되지 않았습니다.");
        }

        if (patternBoard != null)
        {
            patternBoard.GenerateNewPattern();
        }
        else
        {
            Debug.LogError("AIController: RivalPatternBoard가 연결되지 않았습니다.");
        }

        // 캐릭터 ID에 따라 AI 인스턴스 생성
        switch (selectedCharacterId)
        {
            case 0:
                currentAI = new AI_Player1();
                break;
            case 1:
                currentAI = new AI_Player2();
                break;
            case 2:
                currentAI = new AI_Player3();
                break;
            default:
                Debug.LogError("AIController: 유효하지 않은 캐릭터 ID");
                return;
        }

        if (boardManager == null || patternBoard == null || skillController == null)
        {
            Debug.LogError("AIController: 시스템 연결 누락");
            return;
        }

        currentAI.Initialize(boardManager, patternBoard, skillController);
        Debug.Log($"🤖 AIController: Player{selectedCharacterId + 1} 초기화 완료");
    }

    void Update()
    {
        currentAI?.UpdateAI(Time.deltaTime);
    }
}
