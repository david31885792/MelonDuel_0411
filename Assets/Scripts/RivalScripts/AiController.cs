using UnityEngine;

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
    private RivalGaugeManager gaugeManager;

    private void Awake()
    {
        if (skillController == null)
        {
            Debug.LogError("AIController: RivalSkillController가 연결되지 않았습니다.");
            return;
        }

        // 스킬 자동 할당: GetComponents<MonoBehaviour>()에서 ICharacterSkill 구현체 찾기
        ICharacterSkill foundSkill = null;
        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (comp is ICharacterSkill)
            {
                foundSkill = (ICharacterSkill)comp;
                Debug.Log($"[AI DEBUG] 스킬 자동 감지됨: {foundSkill.GetType().Name}");
                break;
            }
        }

        // 없으면 AddComponent 시도
        if (foundSkill == null)
        {
            switch (selectedCharacterId)
            {
                case 0:
                    foundSkill = gameObject.AddComponent<Player1Skill_SlideBooster>();
                    break;
                case 1:
                    foundSkill = gameObject.AddComponent<Player2Skill_Blind>();
                    break;
                case 2:
                    foundSkill = gameObject.AddComponent<Player3Skill_WildTiles>();
                    break;
            }

            Debug.LogWarning($"[AI DEBUG] 스킬이 존재하지 않아 AddComponent로 추가됨: {foundSkill?.GetType().Name}");
        }

        skillController.SetSkill(foundSkill);
        Debug.Log($"[RivalSkillController] 스킬 세팅됨: {foundSkill?.GetType().Name ?? "NULL"}");
        Debug.Log($"[AI DEBUG] 스킬 할당됨? => {skillController.HasSkill()}");

        // 게이지 매니저에 컨트롤러 연결
        gaugeManager = skillController.GetComponent<RivalGaugeManager>();
        if (gaugeManager != null)
        {
            gaugeManager.SetSkillController(skillController);
            Debug.Log("[AI DEBUG] RivalGaugeManager에 스킬 컨트롤러 연결 완료");
        }
        else
        {
            Debug.LogError("[AI DEBUG] RivalGaugeManager가 연결되지 않았습니다.");
        }
    }

    private void Start()
    {
        if (boardManager != null)
            boardManager.GenerateBoardTiles();
        else
            Debug.LogError("AIController: RivalBoardManager가 연결되지 않았습니다.");

        if (patternBoard != null)
            patternBoard.GenerateNewPattern();
        else
            Debug.LogError("AIController: RivalPatternBoard가 연결되지 않았습니다.");

        // AI 스크립트 전부 가져오기
        AI_Player1 ai1 = GetComponent<AI_Player1>();
        AI_Player2 ai2 = GetComponent<AI_Player2>();
        AI_Player3 ai3 = GetComponent<AI_Player3>();

        // 모두 비활성화
        if (ai1 != null) ai1.enabled = false;
        if (ai2 != null) ai2.enabled = false;
        if (ai3 != null) ai3.enabled = false;

        // 선택된 AI만 활성화 및 초기화
        switch (selectedCharacterId)
        {
            case 0:
                currentAI = ai1;
                if (ai1 != null) ai1.enabled = true;
                break;
            case 1:
                currentAI = ai2;
                if (ai2 != null) ai2.enabled = true;
                break;
            case 2:
                currentAI = ai3;
                if (ai3 != null) ai3.enabled = true;
                break;
        }

        currentAI?.Initialize(boardManager, patternBoard, gaugeManager, skillController);
        Debug.Log($"🤖 AIController: Player{selectedCharacterId + 1} 초기화 완료");
        Debug.Log($"[AI DEBUG] boardManager: {(boardManager == null ? "NULL" : "OK")}");
        Debug.Log($"[AI DEBUG] patternBoard: {(patternBoard == null ? "NULL" : "OK")}");
        Debug.Log($"[AI DEBUG] gaugeManager: {(gaugeManager == null ? "NULL" : "OK")}");
        Debug.Log($"[AI DEBUG] skillController: {(skillController == null ? "NULL" : "OK")}");

    }

    void Update()
    {
        currentAI?.UpdateAI(Time.deltaTime);
    }
}
