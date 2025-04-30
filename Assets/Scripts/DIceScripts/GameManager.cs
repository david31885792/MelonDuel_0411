using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("참조")]
    public DiceManager diceManager;
    public Button startButton;

    private bool patternReady = false;

    void Start()
    {
        Debug.Log("🎮 [GameManager] Start (비물리 기반)");

        // 패턴 및 주사위 세팅
        InitDice();

        // 버튼 이벤트 연결
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGamePressed);
    }

    void InitDice()
    {
        if (diceManager == null)
        {
            Debug.LogError("❌ DiceManager가 연결되지 않았습니다.");
            return;
        }

        Debug.Log("🎲 주사위 초기화 중...");

        // DiceManager 내부에서 패턴 생성 및 주사위 배치 수행
        diceManager.InitializeDiceBoard(); // DiceManager.cs에서 이 메서드 정의해야 함

        patternReady = true;
    }

    void OnStartGamePressed()
    {
        if (!patternReady)
        {
            Debug.LogWarning("⚠️ 패턴이 아직 준비되지 않았습니다.");
            return;
        }

        Debug.Log("🚀 GameScene으로 이동합니다...");
        SceneLoader.LoadGameScene(); // 씬 로딩 함수 (SceneLoader.cs 필요)
    }
}
