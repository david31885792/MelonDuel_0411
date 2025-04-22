using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("연결 대상")]
    public DiceManager diceManager;
    public ShakeDetector shakeDetector;

    [Header("UI 오브젝트")]
    public GameObject shakeText;
    public TextMeshProUGUI gameStartText;

    private bool hasStarted = false;

    void Start()
    {
        Debug.Log("🟢 [GameManager] Start() 실행됨");

        if (diceManager != null)
        {
            diceManager.SpawnDice();
            Debug.Log("🎲 [GameManager] diceManager.SpawnDice() 호출 완료");
        }
        else
        {
            Debug.LogWarning("❌ [GameManager] diceManager 연결 안됨");
        }

        if (shakeText != null)
            shakeText.SetActive(true);

        if (gameStartText != null)
            gameStartText.text = "Shake to Start!";

        if (shakeDetector != null)
        {
            shakeDetector.OnShakeDetected += OnShakeStart;
            Debug.Log("📡 [GameManager] ShakeDetector 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("❌ [GameManager] shakeDetector 연결 안됨");
        }
    }

    void Update()
    {
        if (!hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("⌨️ [GameManager] Space 입력 감지 → OnShakeStart() 호출");
            OnShakeStart(); // 디버깅용 강제 시작
        }
    }

    private void OnShakeStart()
    {
        if (hasStarted) return;

        hasStarted = true;
        Debug.Log("✅ [GameManager] OnShakeStart() 실행됨");

        if (shakeText != null)
            shakeText.SetActive(false);

        if (gameStartText != null)
            gameStartText.text = "Rolling...";

        if (diceManager != null)
        {
            diceManager.StartRoll();
            Debug.Log("▶️ [GameManager] diceManager.StartRoll() 호출");
        }
    }

    public void OnDiceComplete()
    {
        Debug.Log("🎯 [GameManager] 모든 주사위 감지 및 정렬 완료");

        if (gameStartText != null)
            gameStartText.text = "Game Start!";

        Invoke("LoadGameScene", 1.5f);
    }

    private void LoadGameScene()
    {
        Debug.Log("🚪 [GameManager] GameScene 로딩 시작");

        SceneManager.LoadScene("GameScene");
        // 또는 SceneLoader.LoadGameScene(); 을 사용하려면 연결 필요
    }
}
