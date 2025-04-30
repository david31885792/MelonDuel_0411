using UnityEngine;
using System;

public class ShakeDetector : MonoBehaviour
{
    [Header("Shake 감지 민감도")]
    public float shakeThreshold = 2.0f;

    [Header("쿨타임 (초)")]
    public float shakeCooldown = 1.0f;

    [Header("디버그 모드 (PC 테스트용)")]
    public bool enableKeyboardTest = true;

    public event Action OnShakeDetected;

    private float lastShakeTime = -999f;
    private Vector3 lastAcceleration;

    void Start()
    {
        lastAcceleration = Input.acceleration;
        Debug.Log("📡 [ShakeDetector] Start() - 감지 시작됨");
    }

    void Update()
    {
#if UNITY_EDITOR
        // PC 테스트: 스페이스바 입력
        if (enableKeyboardTest && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("⌨️ [ShakeDetector] Spacebar 감지 (에디터 테스트)");
            TryInvokeShake();
        }
#endif

        Vector3 acceleration = Input.acceleration;
        float delta = (acceleration - lastAcceleration).magnitude;

        if (delta > shakeThreshold)
        {
            if (Time.time - lastShakeTime > shakeCooldown)
            {
                Debug.Log($"📳 [ShakeDetector] 흔들림 감지됨 (delta: {delta:F2})");
                TryInvokeShake();
            }
        }

        lastAcceleration = acceleration;
    }

    private void TryInvokeShake()
    {
        lastShakeTime = Time.time;

        if (OnShakeDetected != null)
        {
            OnShakeDetected.Invoke();
        }
        else
        {
            Debug.LogWarning("⚠️ [ShakeDetector] OnShakeDetected에 연결된 이벤트 없음");
        }
    }
}
