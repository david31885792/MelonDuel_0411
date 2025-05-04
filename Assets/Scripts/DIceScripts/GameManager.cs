using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private DiceManager diceManager;

    [Header("UI")]
    [SerializeField] private Button startButton;

    private bool patternReady = false;

    void Start()
    {
        if (diceManager == null)
        {
            Debug.LogError("❌ DiceManager 연결 필요");
            return;
        }

        // 초기 패턴 생성 및 주사위 배치
        diceManager.OnDiceBoardReady += HandlePatternReady;
        diceManager.InitializeDiceBoard();

        if (startButton != null)
            startButton.onClick.AddListener(OnStartGamePressed);
    }

    private void HandlePatternReady()
    {
        patternReady = true;
        Debug.Log("✅ 주사위 패턴 준비 완료");
    }

    private void OnStartGamePressed()
    {
        if (!patternReady)
        {
            Debug.LogWarning("⚠️ 아직 패턴 준비되지 않음");
            return;
        }

        // 버튼 비활성화 및 숨기기
        startButton.interactable = false;
        startButton.gameObject.SetActive(false);

        // 주사위 굴리고 완료되면 씬 전환
        diceManager.RollAllDicesAndThen(() =>
        {
            Debug.Log("✅ 주사위 굴림 완료 → GameScene 이동");
            SceneLoader.LoadGameScene();
        });
    }
}
