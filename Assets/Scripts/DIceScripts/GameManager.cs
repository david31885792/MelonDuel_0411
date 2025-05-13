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


        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameStartClip); // ✅ 효과음만 재생
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager.Instance is NULL");
            return;
        }

        if (AudioManager.Instance.gameStartClip == null)
        {
            Debug.LogError("❌ gameStartClip is NULL");
            return;
        }

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

        // 수정 방식: 컴포넌트는 그대로 두고, 시각적으로만 보이게 처리
        foreach (var dice in FindObjectsByType<Dice>(FindObjectsSortMode.None))
        {
            foreach (var renderer in dice.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = true;
            }
        }




        // 주사위 굴리고 완료되면 씬 전환
        diceManager.RollAllDicesAndThen(() =>
        {
            Debug.Log("✅ 주사위 굴림 완료 → GameScene 이동");
            SceneLoader.LoadGameScene();
        });
    }

}
