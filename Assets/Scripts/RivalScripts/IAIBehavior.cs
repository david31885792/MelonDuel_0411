using UnityEngine;

/// <summary>
/// 모든 AI 캐릭터가 따라야 할 공통 행동 규약
/// </summary>
public interface IAIBehavior
{
    /// <summary>
    /// 현재 보드 상태와 클리어 패턴을 바탕으로 타일 이동 등 행동 결정
    /// </summary>
    void MakeMoveDecision();

    /// <summary>
    /// 게이지가 충분한 경우 스킬을 사용할지 판단
    /// </summary>
    void TryUseSkill();

    /// <summary>
    /// AI 초기화 로직 (보드, 패턴, 스킬 시스템 연결)
    /// </summary>
    void Initialize(RivalBoardManager boardManager, RivalPatternBoard patternBoard, RivalGaugeManager gaugeManager, RivalSkillController skillController, RivalScoreManager scoreManager);

    /// <summary>
    /// 매 프레임 또는 일정 주기로 호출되는 AI 행동 루프
    /// </summary>
    void UpdateAI(float deltaTime);
}
